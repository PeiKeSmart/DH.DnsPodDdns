namespace DH.DnsPodDdns.JsonData;

public class RecordData
{
    /// <summary>
    /// 
    /// </summary>
    public Status status { get; set; } = new();
    /// <summary>
    /// 
    /// </summary>
    public Domain domain { get; set; } = new();
    /// <summary>
    /// 
    /// </summary>
    public Info info { get; set; } = new();
    /// <summary>
    /// 
    /// </summary>
    public List<RecordsItem> records { get; set; } = new();
}

public class Domain
{
    /// <summary>
    /// 
    /// </summary>
    public string id { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string name { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string punycode { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string grade { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string owner { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string ext_status { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public int ttl { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<string> dnspod_ns { get; set; } = new();
}

public class Info
{
    /// <summary>
    /// 
    /// </summary>
    public string sub_domains { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string record_total { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string records_num { get; set; } = string.Empty;
}

public class RecordsItem
{
    /// <summary>
    /// 
    /// </summary>
    public string id { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string name { get; set; } = string.Empty;
    /// <summary>
    /// 电信
    /// </summary>
    public string line { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string line_id { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string type { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string ttl { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string value { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string weight { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string mx { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string enabled { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string status { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string monitor_status { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string remark { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string updated_on { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string use_aqb { get; set; } = string.Empty;
}