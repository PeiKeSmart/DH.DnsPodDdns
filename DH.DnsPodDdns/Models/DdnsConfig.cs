namespace DH.DnsPodDdns.Models;

/// <summary>
/// DDNS配置
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
    /// TTL值，默认600秒
    /// </summary>
    public int Ttl { get; set; } = 600;

    /// <summary>
    /// 自动更新间隔（分钟），默认5分钟
    /// </summary>
    public int UpdateInterval { get; set; } = 5;

    /// <summary>
    /// 是否启用自动更新
    /// </summary>
    public bool EnableAutoUpdate { get; set; } = false;

    /// <summary>
    /// 验证配置是否有效
    /// </summary>
    /// <returns>验证结果</returns>
    public ValidationResult Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Token))
            errors.Add("Token不能为空");

        if (string.IsNullOrWhiteSpace(Domain))
            errors.Add("Domain不能为空");

        if (string.IsNullOrWhiteSpace(SubDomain))
            errors.Add("SubDomain不能为空");

        if (Ttl < 1 || Ttl > 604800)
            errors.Add("TTL值必须在1到604800之间");

        if (UpdateInterval < 1)
            errors.Add("更新间隔必须大于0");

        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }
}

/// <summary>
/// 验证结果
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 错误信息列表
    /// </summary>
    public List<string> Errors { get; set; } = new List<string>();

    /// <summary>
    /// 获取错误信息字符串
    /// </summary>
    /// <returns>错误信息</returns>
    public string GetErrorMessage()
    {
        return string.Join("; ", Errors);
    }
}
