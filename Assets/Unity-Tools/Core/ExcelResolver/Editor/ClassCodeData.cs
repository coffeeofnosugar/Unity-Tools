using System;
using System.Collections.Generic;
using Sirenix.Utilities;

namespace Tools.ExcelResolver.Editor
{
    internal class ClassCodeData : IDisposable
    {
        public TableType tableType;
        public string className;
        public FieldData[] fields;
        public int[] keyIndex;

        public ClassCodeData(TableType tableType, string className)
        {
            this.tableType = tableType;
            this.className = $"{char.ToUpper(className[0])}{className.Substring(1)}";
        }

        public void Dispose()
        {
            className = null;
            fields.ForEach(field => field.Dispose());
            fields = null;
            keyIndex = null;
        }
    }
    
    internal class FieldData : IDisposable
    {
        public string name;
        public Type type;
        public string info;
        public string description;
        public string path;
        
        public void Dispose()
        {
            name = null;
            type = null;
            info = null;
            description = null;
            path = null;
        }
    }
    
    internal enum TableType
    {
        SingleKeyTable,         // 单主键表
        UnionMultiKeyTable,     // 多主键表（联合索引）
        MultiKeyTable,          // 多主键表（独立索引）
        NotKetTable,            // 无主键表
        ColumnTable,            // 纵表
    }
}