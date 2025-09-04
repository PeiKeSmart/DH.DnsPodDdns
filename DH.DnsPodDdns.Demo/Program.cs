using DH.DnsPodDdns.Services;
using NewLife.Log;

namespace DH.DnsPodDdns.Demo;

class Program
{
    static async Task Main(string[] args)
    {
        // 配置日志
        XTrace.UseConsole();

        Console.WriteLine("=== DH.DnsPodDdns 演示程序 ===");
        Console.WriteLine();

        // 演示1：获取公网IP
        await DemoGetPublicIp();

        Console.WriteLine();
        Console.WriteLine("按任意键继续...");
        Console.ReadKey();

        // 演示2：DDNS配置和更新
        await DemoDdnsUpdate();

        Console.WriteLine();
        Console.WriteLine("程序结束，按任意键退出...");
        Console.ReadKey();
    }

    /// <summary>
    /// 演示获取公网IP
    /// </summary>
    static async Task DemoGetPublicIp()
    {
        Console.WriteLine("=== 演示1：获取公网IP ===");

        try
        {
            using var ipService = new IpAddressService();
            
            Console.WriteLine("正在获取公网IP地址...");
            var ip = await ipService.GetPublicIpAsync();
            
            if (!string.IsNullOrEmpty(ip))
            {
                Console.WriteLine($"✅ 当前公网IP: {ip}");
            }
            else
            {
                Console.WriteLine("❌ 无法获取公网IP地址");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 获取IP时发生异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 演示DDNS更新
    /// </summary>
    static async Task DemoDdnsUpdate()
    {
        Console.WriteLine("=== 演示2：DDNS配置和更新 ===");
        Console.WriteLine("注意：这是演示代码，请替换为你的真实配置");
        Console.WriteLine();

        // 示例配置（请替换为你的真实配置）
        var config = DdnsSetting.Current;
        
        // 设置演示配置（如果尚未配置）
        if (string.IsNullOrEmpty(config.Token))
            config.Token = "你的DNSPod_Token";           // 格式：ID,Token
        if (string.IsNullOrEmpty(config.Domain))
            config.Domain = "example.com";               // 你的域名  
        if (string.IsNullOrEmpty(config.SubDomain))
            config.SubDomain = "home";                   // 子域名
        if (string.IsNullOrEmpty(config.RecordLine))
            config.RecordLine = "默认";                  // 解析线路
        if (config.Ttl <= 0) 
            config.Ttl = 600;                           // TTL值
        if (config.UpdateInterval <= 0) 
            config.UpdateInterval = 5;                  // 更新间隔（分钟）

        Console.WriteLine("配置信息:");
        Console.WriteLine($"  域名: {config.SubDomain}.{config.Domain}");
        Console.WriteLine($"  线路: {config.RecordLine}");
        Console.WriteLine($"  TTL: {config.Ttl}秒");
        Console.WriteLine();

        // 验证配置
        var validation = config.Validate();
        if (!validation.IsValid)
        {
            Console.WriteLine("❌ 配置验证失败:");
            foreach (var error in validation.Errors)
            {
                Console.WriteLine($"   - {error}");
            }
            return;
        }
        Console.WriteLine("✅ 配置验证通过");
        Console.WriteLine();

        // 检查是否为演示配置
        if (config.Token == "你的DNSPod_Token" || config.Domain == "example.com")
        {
            Console.WriteLine("⚠️  这是演示配置，请替换为你的真实DNSPod Token和域名");
            Console.WriteLine("   DNSPod Token格式：ID,Token");
            Console.WriteLine("   例如：12345,abcdef123456789");
            return;
        }

        try
        {
            using var ddnsService = new DdnsService();

            Console.WriteLine("正在执行DDNS更新...");
            var result = await ddnsService.UpdateAsync();

            if (result.Success)
            {
                Console.WriteLine($"✅ {result.Message}");
                Console.WriteLine($"   当前IP: {result.IpAddress}");
                Console.WriteLine($"   更新时间: {result.UpdateTime:yyyy-MM-dd HH:mm:ss}");
                
                if (result.Changed)
                {
                    Console.WriteLine($"   IP已从 {result.OldIpAddress} 更新为 {result.IpAddress}");
                }
            }
            else
            {
                Console.WriteLine($"❌ DDNS更新失败: {result.Message}");
            }

            // 演示获取DNS记录信息
            Console.WriteLine();
            Console.WriteLine("正在获取DNS记录信息...");
            var record = await ddnsService.GetDnsRecordAsync();
            
            if (record != null)
            {
                Console.WriteLine("✅ DNS记录信息:");
                Console.WriteLine($"   记录ID: {record.id}");
                Console.WriteLine($"   名称: {record.name}");
                Console.WriteLine($"   类型: {record.type}");
                Console.WriteLine($"   值: {record.value}");
                Console.WriteLine($"   线路: {record.line}");
                Console.WriteLine($"   TTL: {record.ttl}");
                Console.WriteLine($"   状态: {(record.enabled == "1" ? "启用" : "禁用")}");
                Console.WriteLine($"   更新时间: {record.updated_on}");
            }
            else
            {
                Console.WriteLine("❌ 未找到匹配的DNS记录");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 操作失败: {ex.Message}");
        }
    }
}
