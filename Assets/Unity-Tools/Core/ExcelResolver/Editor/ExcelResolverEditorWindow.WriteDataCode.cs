using System;
using System.CodeDom;

/* 使用CodeCompileUnit需要将Edit->Project Settings->Player->Other Settings->Api Compatibility Level改为.NET 4.x（或.NET Framework） */


namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow
    {
        private void WriteDataCode(ClassCodeData classCodeData)
        {
            string path = $"{excelResolverConfig.CodePathRoot}/{classCodeData.className}.cs";
            CodeCompileUnit compileUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace(excelResolverConfig.GenerateDataClassNameSpace);
            compileUnit.Namespaces.Add(codeNamespace);
            
            string[] classImports = new string[]
            {
                "System",
                "System.Collections",
                "System.Collections.Generic",
                "UnityEngine",
            };
            foreach (var import in classImports)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(import));
            }
            
            CodeTypeDeclaration classType = new CodeTypeDeclaration(classCodeData.className)
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public,
                CustomAttributes = new CodeAttributeDeclarationCollection()
                {
                    new CodeAttributeDeclaration("Serializable")
                }
            };

            // switch (classCodeData.tableType)
            // {
            //     case TableType.SingleKeyTable:
            //         classType.BaseTypes.Add(new CodeTypeReference("Dictionary<int, " + classCodeData.className + ">"));
            //         break;
            //     case TableType.UnionMultiKeyTable:
            //         classType.BaseTypes.Add(new CodeTypeReference("Dictionary<string, " + classCodeData.className + ">"));
            //         break;
            //     case TableType.MultiKeyTable:
            //         classType.BaseTypes.Add(new CodeTypeReference("Dictionary<int, " + classCodeData.className + ">"));
            //         break;
            //     case TableType.NotKetTable:
            //         classType.BaseTypes.Add(new CodeTypeReference("List<" + classCodeData.className + ">"));
            //         break;
            //     case TableType.ColumnTable:
            //         classType.BaseTypes.Add(new CodeTypeReference("Dictionary<int, " + classCodeData.className + ">"));
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
        }
    }
}