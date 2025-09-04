namespace DH.DnsPodDdns.JsonData;

public class DdnsData
{
    /// <summary>
    /// 
    /// </summary>
    public Status status { get; set; } = new();
    /// <summary>
    /// 
    /// </summary>
    public Record record { get; set; } = new();
}

public class Record
{
    /// <summary>
    /// 
    /// </summary>
    public int id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string name { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string value { get; set; } = string.Empty;
}