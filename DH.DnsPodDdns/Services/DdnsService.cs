using DH.DnsPodDdns.JsonData;
using DH.DnsPodDdns.Models;
using DH.DnsPodDdns.Services;
using DH.DnsPodDdns.Exceptions;
using NewLife.Log;
using DdnsConfig = DH.DnsPodDdns.Models.DdnsConfig;

namespace DH.DnsPodDdns.Services;

/// <summary>
/// DDNS服务
/// </summary>
public class DdnsService : IDisposable
{
    private readonly Dnspod _dnspodClient;
    private readonly IpAddressService _ipService;
    private readonly ILog _logger = XTrace.Log;
    private readonly Timer? _updateTimer;
    private readonly DdnsConfig _config;
    private string? _lastKnownIp;
    private bool _disposed = false;

    /// <summary>
    /// 初始化DDNS服务
    /// </summary>
    /// <param name="config">DDNS配置</param>
    /// <param name="httpClient">HTTP客户端，如果为null则创建新实例</param>
    public DdnsService(DdnsConfig config, HttpClient? httpClient = null)
    {
        // 验证配置
        var validation = config.Validate();
        if (!validation.IsValid)
        {
            throw new ArgumentException($"配置无效: {validation.GetErrorMessage()}", nameof(config));
        }

        _config = config;
        _dnspodClient = new Dnspod(httpClient);
        _ipService = new IpAddressService(httpClient);

        // 如果启用自动更新，创建定时器
        if (_config.EnableAutoUpdate)
        {
            var interval = TimeSpan.FromMinutes(_config.UpdateInterval);
            _updateTimer = new Timer(OnTimerElapsed, null, TimeSpan.Zero, interval);
            _logger.Info($"已启用自动更新，间隔: {_config.UpdateInterval} 分钟");
        }
    }

    /// <summary>
    /// 手动执行一次DDNS更新
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新结果</returns>
    public async Task<DdnsUpdateResult> UpdateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Info($"开始DDNS更新: {_config.SubDomain}.{_config.Domain}");

            // 1. 获取当前公网IP
            var currentIp = await _ipService.GetPublicIpAsync(cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrEmpty(currentIp))
            {
                var error = "无法获取当前公网IP地址";
                _logger.Error(error);
                return new DdnsUpdateResult { Success = false, Message = error };
            }

            _logger.Info($"当前公网IP: {currentIp}");

            // 2. 检查IP是否发生变化
            if (_lastKnownIp == currentIp)
            {
                _logger.Info("IP地址未发生变化，跳过更新");
                return new DdnsUpdateResult 
                { 
                    Success = true, 
                    Message = "IP地址未发生变化", 
                    IpAddress = currentIp,
                    Changed = false
                };
            }

            // 3. 获取现有DNS记录
            var records = await _dnspodClient.RecordListAsync(_config.Token, _config.Domain, "A", _config.SubDomain, cancellationToken).ConfigureAwait(false);
            if (records?.records == null || records.records.Count == 0)
            {
                var error = $"未找到子域名 {_config.SubDomain} 的A记录";
                _logger.Error(error);
                return new DdnsUpdateResult { Success = false, Message = error };
            }

            // 4. 查找匹配的记录
            var targetRecord = records.records.FirstOrDefault(r => 
                r.name == _config.SubDomain && 
                r.type == "A" && 
                (string.IsNullOrEmpty(_config.RecordLine) || r.line == _config.RecordLine || _config.RecordLine == "默认"));

            if (targetRecord == null)
            {
                var error = $"未找到匹配的DNS记录: {_config.SubDomain}.{_config.Domain}";
                _logger.Error(error);
                return new DdnsUpdateResult { Success = false, Message = error };
            }

            _logger.Info($"找到目标记录: ID={targetRecord.id}, 当前值={targetRecord.value}, 线路={targetRecord.line}");

            // 5. 检查记录值是否需要更新
            if (targetRecord.value == currentIp)
            {
                _logger.Info("DNS记录值与当前IP一致，无需更新");
                _lastKnownIp = currentIp;
                return new DdnsUpdateResult 
                { 
                    Success = true, 
                    Message = "DNS记录值与当前IP一致", 
                    IpAddress = currentIp,
                    Changed = false
                };
            }

            // 6. 执行DDNS更新
            var ddnsResult = await _dnspodClient.DdnsAsync(
                _config.Token,
                _config.Domain,
                targetRecord.id,
                _config.SubDomain,
                targetRecord.line,
                currentIp,
                "A",
                _config.Ttl,
                cancellationToken).ConfigureAwait(false);

            if (ddnsResult?.status?.code == "1")
            {
                _lastKnownIp = currentIp;
                var message = $"DDNS更新成功: {_config.SubDomain}.{_config.Domain} -> {currentIp}";
                _logger.Info(message);
                return new DdnsUpdateResult 
                { 
                    Success = true, 
                    Message = message, 
                    IpAddress = currentIp,
                    Changed = true,
                    RecordId = targetRecord.id,
                    OldIpAddress = targetRecord.value
                };
            }
            else
            {
                var error = $"DDNS更新失败: {ddnsResult?.status?.message}";
                _logger.Error(error);
                return new DdnsUpdateResult { Success = false, Message = error };
            }
        }
        catch (Exception ex)
        {
            var error = $"DDNS更新过程中发生异常: {ex.Message}";
            _logger.Error(error + Environment.NewLine + ex.ToString());
            return new DdnsUpdateResult { Success = false, Message = error };
        }
    }

    /// <summary>
    /// 手动执行一次DDNS更新（同步方法）
    /// </summary>
    /// <returns>更新结果</returns>
    public DdnsUpdateResult Update()
    {
        return UpdateAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// 获取当前配置的DNS记录信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>DNS记录信息</returns>
    public async Task<RecordsItem?> GetDnsRecordAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var records = await _dnspodClient.RecordListAsync(_config.Token, _config.Domain, "A", _config.SubDomain, cancellationToken).ConfigureAwait(false);
            
            return records?.records?.FirstOrDefault(r => 
                r.name == _config.SubDomain && 
                r.type == "A" && 
                (string.IsNullOrEmpty(_config.RecordLine) || r.line == _config.RecordLine || _config.RecordLine == "默认"));
        }
        catch (Exception ex)
        {
            _logger.Error($"获取DNS记录失败: {_config.SubDomain}.{_config.Domain}" + Environment.NewLine + ex.ToString());
            return null;
        }
    }

    /// <summary>
    /// 获取当前配置的DNS记录信息（同步方法）
    /// </summary>
    /// <returns>DNS记录信息</returns>
    public RecordsItem? GetDnsRecord()
    {
        return GetDnsRecordAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// 停止自动更新
    /// </summary>
    public void StopAutoUpdate()
    {
        _updateTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        _logger.Info("已停止自动更新");
    }

    /// <summary>
    /// 启动自动更新
    /// </summary>
    public void StartAutoUpdate()
    {
        if (_updateTimer != null)
        {
            var interval = TimeSpan.FromMinutes(_config.UpdateInterval);
            _updateTimer.Change(TimeSpan.Zero, interval);
            _logger.Info($"已启动自动更新，间隔: {_config.UpdateInterval} 分钟");
        }
    }

    /// <summary>
    /// 定时器事件处理
    /// </summary>
    /// <param name="state">状态对象</param>
    private async void OnTimerElapsed(object? state)
    {
        try
        {
            await UpdateAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error("自动更新过程中发生异常" + Environment.NewLine + ex.ToString());
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing">是否正在释放</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _updateTimer?.Dispose();
            _dnspodClient?.Dispose();
            _ipService?.Dispose();
            _disposed = true;
        }
    }
}

/// <summary>
/// DDNS更新结果
/// </summary>
public class DdnsUpdateResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 结果消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 当前IP地址
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 是否发生了变化
    /// </summary>
    public bool Changed { get; set; }

    /// <summary>
    /// 记录ID
    /// </summary>
    public string? RecordId { get; set; }

    /// <summary>
    /// 旧的IP地址
    /// </summary>
    public string? OldIpAddress { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; } = DateTime.Now;
}
