# DH.DnsPodDdns

基于腾讯云DNSPod的动态域名解析库，支持自动获取公网IP并更新DNS记录，适用于内网服务映射和家庭宽带动态IP场景。

## 🚀 特性

- ✅ **异步支持** - 全面的异步/同步API，提供更好的性能
- ✅ **自动IP获取** - 支持多个IP获取服务，确保高可用性
- ✅ **智能更新** - 只有IP变化时才更新DNS记录，减少API调用
- ✅ **定时更新** - 支持自动定时检查和更新
- ✅ **错误处理** - 完善的异常处理和日志记录
- ✅ **资源管理** - 正确的资源管理和IDisposable实现
- ✅ **配置验证** - 启动前验证配置的有效性
- ✅ **多线路支持** - 支持DNSPod的不同解析线路

## 📦 安装

通过NuGet包管理器安装：

```powershell
Install-Package DH.DnsPodDdns
```

或者通过.NET CLI：

```bash
dotnet add package DH.DnsPodDdns
```

## 🔧 快速开始

### 1. 基本使用

```csharp
using DH.DnsPodDdns.Models;
using DH.DnsPodDdns.Services;

// 配置DDNS参数
var config = new DdnsConfig
{
    Token = "your_dnspod_token",        // DNSPod API Token
    Domain = "example.com",             // 你的域名
    SubDomain = "home",                 // 子域名
    RecordLine = "默认",                // 解析线路
    Ttl = 600,                          // TTL值（秒）
    UpdateInterval = 5,                 // 更新间隔（分钟）
    EnableAutoUpdate = false            // 是否启用自动更新
};

// 创建DDNS服务
using var ddnsService = new DdnsService(config);

// 执行一次更新
var result = await ddnsService.UpdateAsync();

if (result.Success)
{
    Console.WriteLine($"更新成功: {result.Message}");
    if (result.Changed)
    {
        Console.WriteLine($"IP已从 {result.OldIpAddress} 更新为 {result.IpAddress}");
    }
}
else
{
    Console.WriteLine($"更新失败: {result.Message}");
}
```

### 2. 启用自动更新

```csharp
var config = new DdnsConfig
{
    Token = "your_dnspod_token",
    Domain = "example.com",
    SubDomain = "home",
    UpdateInterval = 5,                 // 每5分钟检查一次
    EnableAutoUpdate = true             // 启用自动更新
};

using var ddnsService = new DdnsService(config);

// 自动更新已在后台运行
Console.WriteLine("DDNS自动更新已启动，按任意键停止...");
Console.ReadKey();

// 停止自动更新
ddnsService.StopAutoUpdate();
```

### 3. 仅获取公网IP

```csharp
using DH.DnsPodDdns.Services;

using var ipService = new IpAddressService();
var ip = await ipService.GetPublicIpAsync();

Console.WriteLine($"当前公网IP: {ip}");
```

### 4. 直接使用DNSPod API

```csharp
using var dnspod = new Dnspod();

// 获取域名记录
var records = await dnspod.RecordListAsync("your_token", "example.com");

// 更新特定记录
var result = await dnspod.DdnsAsync(
    "your_token", 
    "example.com", 
    "record_id", 
    "subdomain", 
    "默认", 
    "1.2.3.4"
);
```

## 🔑 获取DNSPod Token

1. 登录 [DNSPod控制台](https://console.dnspod.cn/)
2. 进入 "用户中心" -> "安全设置" -> "API Token"
3. 创建新的Token，格式为：`ID,Token`
4. 例如：`12345,abcdef123456789`

## ⚙️ 配置说明

### DdnsConfig 配置项

| 属性 | 类型 | 必填 | 默认值 | 说明 |
|------|------|------|--------|------|
| Token | string | ✅ | - | DNSPod API Token |
| Domain | string | ✅ | - | 主域名 |
| SubDomain | string | ✅ | - | 子域名 |
| RecordLine | string | ❌ | "默认" | 解析线路 |
| Ttl | int | ❌ | 600 | TTL值（1-604800秒） |
| UpdateInterval | int | ❌ | 5 | 自动更新间隔（分钟） |
| EnableAutoUpdate | bool | ❌ | false | 是否启用自动更新 |

### 解析线路选项

- `默认` - 默认线路
- `电信` - 电信线路
- `联通` - 联通线路
- `移动` - 移动线路
- `教育网` - 教育网线路
- `境外` - 境外线路

## 📊 日志记录

库使用XTrace进行日志记录，你可以配置日志输出：

```csharp
// 配置XTrace日志
XTrace.UseConsole();
// 或者
XTrace.UseFile("ddns.log");

// 设置日志级别
XTrace.Level = LogLevel.Info;
```

## 🔧 高级用法

### 自定义HttpClient

```csharp
using var httpClient = new HttpClient();
httpClient.Timeout = TimeSpan.FromSeconds(30);

var config = new DdnsConfig { /* 配置 */ };
using var ddnsService = new DdnsService(config, httpClient);
```

### 检查DNS记录信息

```csharp
using var ddnsService = new DdnsService(config);

var record = await ddnsService.GetDnsRecordAsync();
if (record != null)
{
    Console.WriteLine($"当前DNS记录: {record.name} -> {record.value}");
    Console.WriteLine($"记录状态: {record.enabled}");
    Console.WriteLine($"更新时间: {record.updated_on}");
}
```

### 手动控制自动更新

```csharp
var config = new DdnsConfig
{
    // 配置...
    EnableAutoUpdate = true
};

using var ddnsService = new DdnsService(config);

// 暂停自动更新
ddnsService.StopAutoUpdate();

// 执行手动更新
await ddnsService.UpdateAsync();

// 重新启动自动更新
ddnsService.StartAutoUpdate();
```

## 🚨 错误处理

```csharp
try
{
    var result = await ddnsService.UpdateAsync();
    // 处理结果
}
catch (DnspodApiException ex)
{
    // DNSPod API错误
    Console.WriteLine($"API错误 [{ex.ErrorCode}]: {ex.Message}");
}
catch (ArgumentException ex)
{
    // 配置错误
    Console.WriteLine($"配置错误: {ex.Message}");
}
catch (Exception ex)
{
    // 其他错误
    Console.WriteLine($"未知错误: {ex.Message}");
}
```

## 📋 API参考

### DdnsService

| 方法 | 返回类型 | 说明 |
|------|----------|------|
| UpdateAsync() | Task\<DdnsUpdateResult\> | 异步执行DDNS更新 |
| Update() | DdnsUpdateResult | 同步执行DDNS更新 |
| GetDnsRecordAsync() | Task\<RecordsItem?\> | 异步获取DNS记录信息 |
| GetDnsRecord() | RecordsItem? | 同步获取DNS记录信息 |
| StartAutoUpdate() | void | 启动自动更新 |
| StopAutoUpdate() | void | 停止自动更新 |

### Dnspod

| 方法 | 返回类型 | 说明 |
|------|----------|------|
| RecordListAsync() | Task\<RecordData?\> | 异步获取域名记录列表 |
| DdnsAsync() | Task\<DdnsData?\> | 异步更新DNS记录 |
| RecordList() | RecordData? | 同步获取域名记录列表 |
| Ddns() | DdnsData? | 同步更新DNS记录 |

### IpAddressService

| 方法 | 返回类型 | 说明 |
|------|----------|------|
| GetPublicIpAsync() | Task\<string?\> | 异步获取公网IP |
| GetPublicIp() | string? | 同步获取公网IP |

## 🛠️ 故障排除

### 常见错误

1. **Token格式错误**
   - 确保Token格式为：`ID,Token`
   - 检查Token是否有效且未过期

2. **域名记录不存在**
   - 确保在DNSPod控制台已创建对应的A记录
   - 检查子域名拼写是否正确

3. **网络连接问题**
   - 检查网络连接
   - 确认防火墙未阻止HTTPS请求

4. **IP获取失败**
   - 库会尝试多个IP获取服务
   - 如果所有服务都失败，检查网络连接

### 调试技巧

1. 启用详细日志：
```csharp
XTrace.Level = LogLevel.Debug;
```

2. 检查API响应：
```csharp
// 日志中会显示完整的API请求和响应
```

## 📄 许可证

本项目基于 [MIT 许可证](LICENSE) 开源。

## 🤝 贡献

欢迎提交Issue和Pull Request来改进这个项目！

## 📞 支持

- 项目主页：https://github.com/PeiKeSmart/DH.DnsPodDdns
- 问题反馈：https://github.com/PeiKeSmart/DH.DnsPodDdns/issues
- 公司官网：https://www.haocoding.com/

---

© 2020-2025 湖北登灏科技有限公司 - DnsPod域名动态解析库

基于DnsPod的动态域名解析库，用于映射内网服务