using System.CodeDom;
using System.CodeDom.Compiler;
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
            
            CodeTypeDeclaration classType = new CodeTypeDeclaration($"{classCodeData.className}SO")
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public,
                BaseTypes =
                {
                    new CodeTypeReference("ScriptableObject"),
                    new CodeTypeReference("IExcelSO"),
                }
            };
            codeNamespace.Types.Add(classType);
            
            CodeGeneratorOptions options = new CodeGeneratorOptions
            {
                BracingStyle = "C",
                BlankLinesBetweenMembers = false,
                VerbatimOrder = true,
            };

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
            }
        }
    }
}