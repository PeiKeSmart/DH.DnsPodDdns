# DH.DnsPodDdns

基于腾讯云 DNSPod 的动态域名解析（DDNS）库：自动获取公网 IP，仅在变化时更新 DNS 记录，支持定时任务与自动创建缺失记录，适合家庭宽带 / 自建服务。

## ✨ 特性

- 异步 / 同步 API
- 多源公网 IP 获取与回退
- 仅在 IP 变化时更新（节省 API 调用）
- 支持自动定时更新
- 可选自动创建 A 记录
- 结构简单、易嵌入

## 📦 安装

```powershell
Install-Package DH.DnsPodDdns
```

```bash
dotnet add package DH.DnsPodDdns
```

## 🚀 快速开始（使用全局配置 DdnsSetting）

```csharp
using DH.DnsPodDdns;
using DH.DnsPodDdns.Services;

var cfg = DdnsSetting.Current;
cfg.Token = "12345,xxxxxxxxxxxxxxxx"; // 必填：格式 ID,Token
cfg.Domain = "example.com";
cfg.SubDomain = "home";
cfg.RecordLine = "默认";
cfg.Ttl = 600;
cfg.UpdateInterval = 5;      // 分钟
cfg.AutoCreateRecord = true; // 不存在则自动创建
cfg.EnableAutoUpdate = false; // 示例关闭自动更新

using var service = new DdnsService();
var r = await service.UpdateAsync();
Console.WriteLine(r.Success ? r.Message : "失败: " + r.Message);
```

## 🔁 启用自动更新

```csharp
DdnsSetting.Current.EnableAutoUpdate = true;
using var service = new DdnsService();
Console.WriteLine("自动更新中，按任意键退出...");
Console.ReadKey();
service.StopAutoUpdate();
```

## 🌐 获取公网 IP

```csharp
using DH.DnsPodDdns.Services;
var ip = await new IpAddressService().GetPublicIpAsync();
Console.WriteLine(ip);
```

## 🔧 直接调用底层 DNSPod API

```csharp
using DH.DnsPodDdns;
DdnsSetting.Current.Token = "12345,xxxxxxxx";
using var api = new Dnspod();
var list = await api.RecordListAsync("example.com", "A", "home");
```

## ⚙️ 配置项（DdnsSetting）

| 属性 | 类型 | 必填 | 默认值 | 说明 |
|------|------|------|--------|------|
| Token | string | ✅ | - | DNSPod Token（ID,Token） |
| Domain | string | ✅ | - | 主域名，如 example.com |
| SubDomain | string | ✅ | - | 子域名，如 home |
| RecordLine | string | ❌ | 默认 | 解析线路（默认/电信/联通/移动/教育网/境外） |
| Ttl | int | ❌ | 600 | TTL 秒（1-604800） |
| UpdateInterval | int | ❌ | 5 | 自动更新间隔（分钟） |
| EnableAutoUpdate | bool | ❌ | false | 是否启用定时更新 |
| AutoCreateRecord | bool | ❌ | false | 记录不存在时自动创建 |

## 🧪 错误处理示例

```csharp
try
{
    var result = await service.UpdateAsync();
    if (!result.Success) Console.WriteLine("更新失败: " + result.Message);
}
catch (ArgumentException ex) { Console.WriteLine("配置错误: " + ex.Message); }
catch (TimeoutException) { Console.WriteLine("请求超时"); }
catch (Exception ex) { Console.WriteLine("未知错误: " + ex.Message); }
```

## 📋 API 速览

### DdnsService

| 方法 | 返回 | 说明 |
|------|------|------|
| UpdateAsync() | Task&lt;DdnsUpdateResult&gt; | 执行一次更新 |
| Update() | DdnsUpdateResult | 同步更新 |
| GetDnsRecordAsync() | Task&lt;RecordsItem?&gt; | 获取当前记录 |
| GetDnsRecord() | RecordsItem? | 同步获取 |
| StartAutoUpdate() | void | 启动定时更新 |
| StopAutoUpdate() | void | 停止定时更新 |

### Dnspod

| 方法 | 返回 | 说明 |
|------|------|------|
| RecordListAsync(domain, type?, sub?) | Task&lt;RecordData?&gt; | 获取域名记录列表 |
| DdnsAsync(domain, recordId, sub, line, value, type?, ttl?) | Task&lt;DdnsData?&gt; | 动态更新 |

### IpAddressService

| 方法 | 返回 | 说明 |
|------|------|------|
| GetPublicIpAsync() | Task&lt;string?&gt; | 获取公网 IP |
| GetPublicIp() | string? | 同步获取 |

## 🧾 日志

```csharp
using NewLife.Log;
XTrace.UseConsole();
XTrace.Level = NewLife.Log.LogLevel.Info; // 或 Debug
```

## �️ 常见问题

1. 未更新：IP 可能未变化 / 线路不匹配。
2. Token 错误：确认格式 `ID,Token` 且有效。
3. 记录不存在：开启 AutoCreateRecord。
4. 超时：检查网络或调大 HttpClient 超时。

## 🔍 调试

```csharp
XTrace.Level = NewLife.Log.LogLevel.Debug; // 输出调试级别日志
```

## �📄 许可证

MIT，见 [LICENSE](LICENSE)

## 🤝 贡献

欢迎提交 Issue / PR。

## 📞 支持

- 项目主页：<https://github.com/PeiKeSmart/DH.DnsPodDdns>
- 问题反馈：<https://github.com/PeiKeSmart/DH.DnsPodDdns/issues>
- 公司官网：<https://www.haocoding.com/>

---
© 2020-2025 湖北登灏科技有限公司

基于 DNSPod 的动态域名解析库。
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