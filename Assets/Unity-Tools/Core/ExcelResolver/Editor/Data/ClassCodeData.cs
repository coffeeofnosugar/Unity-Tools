using System;
using System.Linq;
using Sirenix.Utilities;

namespace Tools.ExcelResolver.Editor
{
    internal class ClassCodeData : IDisposable
    {
        public readonly TableType tableType;
        public readonly string className;
        public readonly FieldData[] fields;
        public readonly int[] keyIndex;
        public readonly FieldData[] keyField;

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
            fields.ForEach(field => field.Dispose());
        }
    }
}