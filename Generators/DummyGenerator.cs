using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators
{
    [Generator]
    public class DummyGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            string dummySource = $@"using System;

namespace Repro.Generators.VariableRenaming
{{ 
    public static class DummyMethods
    {{
        public static void {((SyntaxReceiver)context.SyntaxReceiver).MethodName}(string message){{
            Console.Write(message);
        }}
    }}
}}
";

            context.AddSource("DummyMethods.Generated.cs", dummySource);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        private class SyntaxReceiver : ISyntaxReceiver
        {
            public string MethodName;

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is MemberAccessExpressionSyntax memberAccessExpressionSyntax &&
                    memberAccessExpressionSyntax.Expression is IdentifierNameSyntax identifierNameSyntax &&
                    identifierNameSyntax.Identifier.ValueText == "DummyMethods")
                {
                    MethodName = memberAccessExpressionSyntax.Name.Identifier.ValueText;
                }
            }
        }
    }
}