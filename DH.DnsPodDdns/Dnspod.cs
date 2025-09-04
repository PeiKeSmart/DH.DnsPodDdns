using System.Text.Json;

using DH.DnsPodDdns.JsonData;

namespace DH.DnsPodDdns;

/// <summary>
/// DNSPod API客户端
/// </summary>
public class Dnspod : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://dnsapi.cn";
    private bool _disposed = false;

    /// <summary>
    /// 初始化DNSPod客户端
    /// </summary>
    /// <param name="httpClient">HTTP客户端，如果为null则创建新实例</param>
    public Dnspod(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        
        // 设置User-Agent
        if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DH.DnsPodDdns/4.14 (https://github.com/PeiKeSmart/DH.DnsPodDdns)");
        }
    }

    /// <summary>
    /// 获取域名的DNS记录列表
    /// </summary>
    /// <param name="token">DNSPod API Token</param>
    /// <param name="domain">域名</param>
    /// <param name="recordType">记录类型，默认为A记录</param>
    /// <param name="subDomain">子域名，可选</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>记录数据</returns>
    public async Task<RecordData?> RecordListAsync(string token, string domain, string recordType = "A", 
        string? subDomain = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token不能为空", nameof(token));
        if (string.IsNullOrWhiteSpace(domain))
            throw new ArgumentException("Domain不能为空", nameof(domain));

        var parameters = new Dictionary<string, string>
        {
            { "login_token", token },
            { "record_type", recordType },
            { "format", "json" },
            { "domain", domain }
        };

        if (!string.IsNullOrWhiteSpace(subDomain))
        {
            parameters.Add("sub_domain", subDomain);
        }

        try
        {
            var httpContent = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync($"{_baseUrl}/Record.List", httpContent, cancellationToken).ConfigureAwait(false);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API请求失败: {response.StatusCode} - {response.ReasonPhrase}");
            }

            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var recordData = JsonSerializer.Deserialize<RecordData>(jsonString);

            // 检查API返回的状态
            if (recordData?.status?.code != "1")
            {
                throw new InvalidOperationException($"DNSPod API错误: {recordData?.status?.message}");
            }

            return recordData;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            throw new TimeoutException("请求超时", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("JSON反序列化失败", ex);
        }
    }

    /// <summary>
    /// 更新DNS记录（DDNS）
    /// </summary>
    /// <param name="token">DNSPod API Token</param>
    /// <param name="domain">域名</param>
    /// <param name="recordId">记录ID</param>
    /// <param name="subDomain">子域名</param>
    /// <param name="recordLine">记录线路</param>
    /// <param name="value">记录值（IP地址）</param>
    /// <param name="recordType">记录类型，默认为A记录</param>
    /// <param name="ttl">TTL值，默认600秒</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>DDNS更新结果</returns>
    public async Task<DdnsData?> DdnsAsync(string token, string domain, string recordId, string subDomain, 
        string recordLine, string value, string recordType = "A", int ttl = 600, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token不能为空", nameof(token));
        if (string.IsNullOrWhiteSpace(domain))
            throw new ArgumentException("Domain不能为空", nameof(domain));
        if (string.IsNullOrWhiteSpace(recordId))
            throw new ArgumentException("RecordId不能为空", nameof(recordId));
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value不能为空", nameof(value));

        var parameters = new Dictionary<string, string>
        {
            { "login_token", token },
            { "format", "json" },
            { "domain", domain },
            { "record_id", recordId },
            { "sub_domain", subDomain },
            { "record_line", recordLine },
            { "record_type", recordType },
            { "value", value },
            { "ttl", ttl.ToString() }
        };

        try
        {
            var httpContent = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync($"{_baseUrl}/Record.Ddns", httpContent, cancellationToken).ConfigureAwait(false);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API请求失败: {response.StatusCode} - {response.ReasonPhrase}");
            }

            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var ddnsData = JsonSerializer.Deserialize<DdnsData>(jsonString);

            // 检查API返回的状态
            if (ddnsData?.status?.code != "1")
            {
                throw new InvalidOperationException($"DNSPod API错误: {ddnsData?.status?.message}");
            }

            return ddnsData;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            throw new TimeoutException("请求超时", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("JSON反序列化失败", ex);
        }
    }

    /// <summary>
    /// 同步版本 - 获取域名的DNS记录列表
    /// </summary>
    public RecordData? RecordList(string token, string domain, string recordType = "A", string? subDomain = null)
    {
        return RecordListAsync(token, domain, recordType, subDomain).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 同步版本 - 更新DNS记录（DDNS）
    /// </summary>
    public DdnsData? Ddns(string token, string domain, string recordId, string subDomain, 
        string recordLine, string value, string recordType = "A", int ttl = 600)
    {
        return DdnsAsync(token, domain, recordId, subDomain, recordLine, value, recordType, ttl).GetAwaiter().GetResult();
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
            _httpClient?.Dispose();
            _disposed = true;
        }
    }
}