using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mutineer
{
    public static class SyntaxNodeExtensions
    {
        public static (string Namespace, string Class, string Method, int LineNumber) GetHierarchicalLocation(this SyntaxNode node)
        {
            int lineNumber = node.GetLocation().GetMappedLineSpan().StartLinePosition.Line + 1;

            var currentNode      = node;
            string namespaceName = null;
            string className     = null;
            string methodName    = null;

            while (true)
            {
                if (currentNode is NamespaceDeclarationSyntax namespaceSyntax)
                {
                    namespaceName = namespaceSyntax.Name.ToString();
                    break;
                }

                switch (currentNode)
                {
                    case ClassDeclarationSyntax classSyntax when className == null:
                        className = classSyntax.Identifier.Text;
                        break;
                    case MethodDeclarationSyntax methodSyntax when methodName == null:
                        methodName = methodSyntax.Identifier.Text;
                        break;
                }

                currentNode = currentNode.Parent;
                if (currentNode == null) break;
            }


            return (namespaceName, className, methodName, lineNumber);
        }
    }
}
