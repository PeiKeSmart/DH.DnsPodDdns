namespace DH.DnsPodDdns.JsonData;

public class DdnsData
{
    /// <summary>
    /// 
    /// </summary>
    public Status status { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Record record { get; set; }
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
    public string name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string value { get; set; }
}