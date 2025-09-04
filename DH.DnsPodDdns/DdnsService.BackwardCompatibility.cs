using DH.DnsPodDdns.Models;
using DH.DnsPodDdns.Services;

namespace DH.DnsPodDdns;

/// <summary>
/// 兼容旧命名空间的 DDNS 服务入口。内部继承新的 <see cref="Services.DdnsService"/>。
/// 建议新代码引用 <c>DH.DnsPodDdns.Services</c> 命名空间中的实现。
/// </summary>
public class DdnsService : Services.DdnsService
{
    /// <inheritdoc />
    public DdnsService(DdnsConfig config, HttpClient? httpClient = null) : base(config, httpClient)
    {
    }
}
