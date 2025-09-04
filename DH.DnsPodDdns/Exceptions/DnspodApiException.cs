namespace DH.DnsPodDdns.Exceptions;

/// <summary>
/// DNSPod API异常
/// </summary>
public class DnspodApiException : Exception
{
    /// <summary>
    /// API错误代码
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// 初始化DNSPod API异常
    /// </summary>
    /// <param name="message">错误消息</param>
    public DnspodApiException(string message) : base(message)
    {
    }

    /// <summary>
    /// 初始化DNSPod API异常
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="errorCode">错误代码</param>
    public DnspodApiException(string message, string? errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// 初始化DNSPod API异常
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="innerException">内部异常</param>
    public DnspodApiException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// 初始化DNSPod API异常
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="errorCode">错误代码</param>
    /// <param name="innerException">内部异常</param>
    public DnspodApiException(string message, string? errorCode, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
