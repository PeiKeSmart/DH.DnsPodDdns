namespace DH.DnsPodDdns;

/// <summary>
/// IP地址获取服务
/// </summary>
public class IpService : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string[] _defaultUrls = 
    {
        "https://api.ipify.org",
        "https://ifconfig.me/ip", 
        "https://ip.sb",
        "https://myip.ipip.net",
        "https://api.ip.sb/ip"
    };
    private bool _disposed = false;

    /// <summary>
    /// 初始化IP服务
    /// </summary>
    /// <param name="httpClient">HTTP客户端，如果为null则创建新实例</param>
    public IpService(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
        
        if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DH.DnsPodDdns/4.14");
        }
    }

    /// <summary>
    /// 获取当前公网IP地址
    /// </summary>
    /// <param name="urls">自定义IP获取URL列表，如果为null则使用默认URL</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>IP地址</returns>
    public async Task<string?> GetPublicIpAsync(IEnumerable<string>? urls = null, CancellationToken cancellationToken = default)
    {
        var urlsToTry = urls ?? _defaultUrls;

        foreach (var url in urlsToTry)
        {
            try
            {
                var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var ip = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    ip = ip.Trim();
                    
                    // 验证IP格式
                    if (IsValidIpAddress(ip))
                    {
                        return ip;
                    }
                }
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                // 尝试下一个URL
                continue;
            }
        }

        return null;
    }

    /// <summary>
    /// 同步版本 - 获取当前公网IP地址
    /// </summary>
    /// <param name="urls">自定义IP获取URL列表，如果为null则使用默认URL</param>
    /// <returns>IP地址</returns>
    public string? GetPublicIp(IEnumerable<string>? urls = null)
    {
        return GetPublicIpAsync(urls).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 验证IP地址格式
    /// </summary>
    /// <param name="ip">IP地址字符串</param>
    /// <returns>是否为有效的IP地址</returns>
    private static bool IsValidIpAddress(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            return false;

        return System.Net.IPAddress.TryParse(ip, out var address) && 
               (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
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
