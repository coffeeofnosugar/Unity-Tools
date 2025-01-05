using System;
using System.Linq;
using Sirenix.Utilities;

namespace Tools.ExcelResolver.Editor
{
    internal class ClassCodeData : IDisposable
    {
        public TableType tableType;
        public string className;
        public FieldData[] fields;
        public int[] keyIndex;
        public FieldData[] keyField;

        public ClassCodeData(TableType tableType, string className, FieldData[] fields, int[] keyIndex)
        {
            this.tableType = tableType;
            this.className = $"{char.ToUpper(className[0])}{className.Substring(1)}";
            this.fields = fields;
            this.keyIndex = keyIndex;
            
            keyField = new FieldData[keyIndex.Length];
            for (int i = 0; i < keyIndex.Length; i++)
            {
                keyField[i] = fields.First(f => f.colIndex == keyIndex[i]);
            }
        }

        public void Dispose()
        {
            className = null;
            fields.ForEach(field => field.Dispose());
            fields = null;
            keyIndex = null;
        }
    }
}