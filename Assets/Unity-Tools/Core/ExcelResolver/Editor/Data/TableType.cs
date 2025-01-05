namespace Tools.ExcelResolver.Editor
{
    internal enum TableType
    {
        SingleKeyTable,         // 单主键表
        UnionMultiKeyTable,     // 多主键表（联合索引）
        MultiKeyTable,          // 多主键表（独立索引）
        NotKetTable,            // 无主键表
        ColumnTable,            // 纵表
    }
}