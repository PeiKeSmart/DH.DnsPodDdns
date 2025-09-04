using NewLife.Log;

namespace DH.DnsPodDdns.Services;

/// <summary>
/// IP地址获取服务
/// </summary>
public class IpAddressService : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILog _logger = XTrace.Log;
    private readonly string[] _ipServiceUrls = {
        "https://ipv4.icanhazip.com",
        "https://api.ipify.org",
        "https://ipv4.ident.me",
        "https://ip.3322.net",
        "https://myip.ipip.net"
    };
    private bool _disposed = false;

    /// <summary>
    /// 初始化IP地址获取服务
    /// </summary>
    /// <param name="httpClient">HTTP客户端，如果为null则创建新实例</param>
    public IpAddressService(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    /// <summary>
    /// 获取当前公网IP地址
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>公网IP地址</returns>
    public async Task<string?> GetPublicIpAsync(CancellationToken cancellationToken = default)
    {
        foreach (var url in _ipServiceUrls)
        {
            try
            {
                _logger.Debug($"尝试从 {url} 获取公网IP");
                
                var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                
                if (response.IsSuccessStatusCode)
                {
                    var ip = (await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)).Trim();
                    
                    if (IsValidIpAddress(ip))
                    {
                        _logger.Info($"成功获取公网IP: {ip}");
                        return ip;
                    }
                    else
                    {
                        _logger.Warn($"从 {url} 获取的IP格式无效: {ip}");
                    }
                }
                else
                {
                    _logger.Warn($"从 {url} 获取IP失败，状态码: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"从 {url} 获取IP时发生异常: {ex.Message}");
            }
        }

        _logger.Error("所有IP获取服务都失败了");
        return null;
    }

    /// <summary>
    /// 获取当前公网IP地址（同步方法）
    /// </summary>
    /// <returns>公网IP地址</returns>
    public string? GetPublicIp()
    {
        return GetPublicIpAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// 验证IP地址格式是否有效
    /// </summary>
    /// <param name="ip">IP地址字符串</param>
    /// <returns>是否有效</returns>
    private static bool IsValidIpAddress(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            return false;

        return System.Net.IPAddress.TryParse(ip, out var address) && 
               address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
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
