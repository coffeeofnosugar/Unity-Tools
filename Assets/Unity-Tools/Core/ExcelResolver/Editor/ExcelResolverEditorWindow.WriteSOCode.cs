using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;

namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow
    {
        private void WriteSOCode(ClassCodeData classCodeData)
        {
            string outputPath = $"{excelResolverConfig.CodePathRoot}/{classCodeData.className}SO.cs";
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            #region 命名空间
            
            CodeNamespace codeNamespace = new CodeNamespace(excelResolverConfig.GenerateDataClassNameSpace);
            compileUnit.Namespaces.Add(codeNamespace);
            
            #endregion

            #region 引用
            
            string[] classImports = new string[]
            {
                "System",
                "System.Collections",
                "System.Collections.Generic",
                "UnityEngine",
                "Sirenix.OdinInspector",
            };
            foreach (var import in classImports)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(import));
            }
            
            #endregion

            #region 类

            CodeTypeDeclaration classType = new CodeTypeDeclaration($"{classCodeData.className}SO")
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public,
                BaseTypes =
                {
                    new CodeTypeReference("SerializedScriptableObject"),
                    new CodeTypeReference("IExcelSO"),
                }
            };
            codeNamespace.Types.Add(classType);
            
            #endregion

            #region 字段
            
            List<CodeMemberField> codeFields = new List<CodeMemberField>();

            switch (classCodeData.tableType)
            {
                case TableType.SingleKeyTable:
                    FieldData keyField = classCodeData.keyField[0];
                    CodeMemberField codeField = new CodeMemberField($"Dictionary<{keyField.typeString}, {classCodeData.className}>", "Data")
                    {
                        Attributes = MemberAttributes.Public,
                    };
                    codeFields.Add(codeField);
                    break;
                case TableType.UnionMultiKeyTable:
                    break;
                case TableType.MultiKeyTable:
                    break;
                case TableType.NotKetTable:
                    break;
                case TableType.ColumnTable:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            foreach (var codeField in codeFields)
            {
                classType.Members.Add(codeField);
            }
            
            #endregion



            #region 代码风格设置
            
            CodeGeneratorOptions options = new CodeGeneratorOptions
            {
                BracingStyle = "C",
                BlankLinesBetweenMembers = true,
                VerbatimOrder = true,
            };

            #endregion

            #region 写入文件
            
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
            }
            
            #endregion
        }
    }
}