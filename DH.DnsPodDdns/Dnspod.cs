using DH.DnsPodDdns.JsonData;

namespace DH.DnsPodDdns;

public class Dnspod
{
    public RecordData RecordList(string token, string domain)
    {
        HttpContent httpContent = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"login_token", token },
                {"record_type", "A"},
                {"format", "json"},
                {"domain", domain }
            });
        var httpClient = new HttpClient();
        var content = httpClient.PostAsync("https://dnsapi.cn/Record.List", httpContent).Result;
        RecordData? recordData = null;

        if (content.IsSuccessStatusCode)
        {
            var s = content.Content.ReadAsStringAsync().Result;
            recordData = JsonSerializer.Deserialize<RecordData>(s);
        }

        return recordData;
    }

    public DdnsData Ddns(string token, string domain, string recordId, string subDomain, string recoreLine, string valueIp)
    {
        HttpContent httpContent = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "login_token",token },
                {"format","json"},
                {"domain", domain},
                {"record_id" ,recordId},
                {"sub_domain",subDomain },
                {"record_line" ,recoreLine},
                {"record_type", "A"},
                {"value IP",valueIp }
            });
        HttpClient httpClient = new HttpClient();
        var content = httpClient.PostAsync("https://dnsapi.cn/Record.Ddns", httpContent).Result;
        DdnsData ddnsData = null;

        if (content.IsSuccessStatusCode)
        {
            string s = content.Content.ReadAsStringAsync().Result;
            ddnsData = JsonSerializer.Deserialize<DdnsData>(s);
        }

        return ddnsData;
    }
}