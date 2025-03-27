namespace DataMigration.Dtos;

public class DataMigrationDto
{
    public string TableName { get; set; }
    public string TableDescription { get; set; }
    /// <summary>
    /// 是否同步结构
    /// </summary>
    public string? IsStructure { get; set; }
    /// <summary>
    /// 是否同步数据
    /// </summary>
    public string? IsData { get; set; }
    public long DataCount { get; set; }

    /// <summary>
    /// 结构同步状态
    /// </summary>
    public string? StructureStatus { get; set; }
    /// <summary>
    /// 数据同步状态
    /// </summary>
    public string? DataStatus { get; set; }
    public string ErrMessage { get; set; }
    //public string Sql { get; set; }
    public DateTime CreateTime { get; set; }
}
