namespace Vitastic.Web.Models.DTOs.JsDataTableHelper;

public class DataTablesRequest
{
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public DataTablesSearch Search { get; set; }
    public DataTablesOrder[] Order { get; set; }
}

public class DataTablesSearch
{
    public string Value { get; set; }
    public bool Regex { get; set; }
}

public class DataTablesOrder
{
    public int Column { get; set; }
    public string Dir { get; set; }
}

public class DataTablesResponse<T>
{
    public int Draw { get; set; }
    public long RecordsTotal { get; set; }
    public long RecordsFiltered { get; set; }
    public List<T> Data { get; set; }
}
