namespace DH.DnsPodDdns.JsonData;

/// <summary>
/// DDNS配置数据
/// </summary>
public class DdnsConfig
{
    /// <summary>
    /// DNSPod API Token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 域名
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// 子域名
    /// </summary>
    public string SubDomain { get; set; } = string.Empty;

    /// <summary>
    /// 记录线路，默认为"默认"
    /// </summary>
    public string RecordLine { get; set; } = "默认";

    /// <summary>
    /// 记录类型，默认为A记录
    /// </summary>
    public string RecordType { get; set; } = "A";

    /// <summary>
    /// TTL值，默认600秒
    /// </summary>
    public int Ttl { get; set; } = 600;

    /// <summary>
    /// 更新间隔时间（秒），默认300秒（5分钟）
    /// </summary>
    public int IntervalSeconds { get; set; } = 300;

    /// <summary>
    /// 自定义IP获取URL列表
    /// </summary>
    public List<string> IpUrls { get; set; } = new();

    /// <summary>
    /// 是否启用日志
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// 验证配置是否有效
    /// </summary>
    /// <returns>验证结果</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Token) &&
               !string.IsNullOrWhiteSpace(Domain) &&
               !string.IsNullOrWhiteSpace(SubDomain) &&
               IntervalSeconds > 0 &&
               Ttl > 0;
    }
}

/// <summary>
/// 旧的用户数据类（保持向后兼容）
/// </summary>
[Obsolete("请使用DdnsConfig类替代")]
public class UserData
{
    public string Token { get; set; } = string.Empty;

    public string Domain { get; set; } = string.Empty;

    public int IntervalTime { get; set; }

    public string GetIPUrl { get; set; } = string.Empty;
}