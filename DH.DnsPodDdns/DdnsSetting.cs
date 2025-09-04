using System.ComponentModel;

using NewLife.Configuration;

namespace DH.DnsPodDdns;

/// <summary>配置</summary>
[Config("Ddns")]
public class DdnsSetting : Config<DdnsSetting>
{
    /// <summary>
    /// DNSPod API Token
    /// </summary>
    [Description("Ddns的Token")]
    public String? Token { get; set; }

    /// <summary>
    /// 域名
    /// </summary>
    [Description("域名")]
    public String? Domain { get; set; }

    /// <summary>
    /// 子域名
    /// </summary>
    [Description("子域名")]
    public String? SubDomain { get; set; }

    /// <summary>
    /// 记录线路，默认为"默认"
    /// </summary>
    [Description("记录线路，默认为\"默认\"")]
    public String? RecordLine { get; set; } = "默认";

    /// <summary>
    /// TTL值，默认600秒
    /// </summary>
    [Description("TTL值，默认600秒")]
    public Int32 Ttl { get; set; } = 600;

    /// <summary>
    /// 自动更新间隔（分钟），默认5分钟
    /// </summary>
    [Description("自动更新间隔（分钟），默认5分钟")]
    public Int32 UpdateInterval { get; set; } = 5;

    /// <summary>
    /// 是否启用自动更新
    /// </summary>
    [Description("是否启用自动更新")]
    public Boolean EnableAutoUpdate { get; set; } = false;

    /// <summary>
    /// 当目标子域记录不存在时是否自动创建，默认 false
    /// </summary>
    [Description("当目标子域记录不存在时是否自动创建，默认 false")]
    public Boolean AutoCreateRecord { get; set; } = false;

    /// <summary>
    /// 验证配置是否有效
    /// </summary>
    /// <returns>验证结果</returns>
    public ValidationResult Validate()
    {
        var errors = new List<String>();

        if (String.IsNullOrWhiteSpace(Token))
            errors.Add("Token不能为空");

        if (String.IsNullOrWhiteSpace(Domain))
            errors.Add("Domain不能为空");

        if (String.IsNullOrWhiteSpace(SubDomain))
            errors.Add("SubDomain不能为空");

        if (Ttl < 1 || Ttl > 604800)
            errors.Add("TTL值必须在1到604800之间");

        if (UpdateInterval < 1)
            errors.Add("更新间隔必须大于0");

        // 简单校验：Token 里至少包含一个逗号（避免只填后半段）
        if (!String.IsNullOrWhiteSpace(Token) && !Token.Contains(','))
            errors.Add("Token 需要包含前置数字ID与逗号");

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
    public List<String> Errors { get; set; } = new List<String>();

    /// <summary>
    /// 获取错误信息字符串
    /// </summary>
    /// <returns>错误信息</returns>
    public string GetErrorMessage()
    {
        return string.Join("; ", Errors);
    }
}