using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mutineer.Interface;

namespace Mutineer.Walkers
{
    public class BinaryExpressionMutagen : IMutagen
    {
        public List<INodePair> NodePairs { get; }
        public CSharpSyntaxWalker Walker { get; }

        public BinaryExpressionMutagen()
        {
            NodePairs = new List<INodePair>();
            Walker    = new BinaryExpressionWalker(NodePairs);
        }

        private class BinaryExpressionWalker : CSharpSyntaxWalker
        {
            private readonly Dictionary<SyntaxKind, List<SyntaxKind>> _kindToAlternativeKinds;
            private readonly List<INodePair> _nodePairs;

            public BinaryExpressionWalker(List<INodePair> nodePairs)
            {
                _nodePairs = nodePairs;
                _kindToAlternativeKinds = new Dictionary<SyntaxKind, List<SyntaxKind>>
                {
                    { SyntaxKind.MinusToken,              new List<SyntaxKind> { SyntaxKind.PlusToken } },
                    { SyntaxKind.PlusToken,               new List<SyntaxKind> { SyntaxKind.MinusToken } },
                    { SyntaxKind.AsteriskToken,           new List<SyntaxKind> { SyntaxKind.SlashToken } },
                    { SyntaxKind.SlashToken,              new List<SyntaxKind> { SyntaxKind.AsteriskToken } },
                    { SyntaxKind.PercentToken,            new List<SyntaxKind> { SyntaxKind.AsteriskToken } },
                    { SyntaxKind.LessThanToken,           new List<SyntaxKind> { SyntaxKind.LessThanEqualsToken, SyntaxKind.GreaterThanEqualsToken} },
                    { SyntaxKind.GreaterThanToken,        new List<SyntaxKind> { SyntaxKind.LessThanEqualsToken, SyntaxKind.GreaterThanEqualsToken} },
                    { SyntaxKind.LessThanEqualsToken,     new List<SyntaxKind> { SyntaxKind.LessThanToken, SyntaxKind.GreaterThanToken } },
                    { SyntaxKind.GreaterThanEqualsToken,  new List<SyntaxKind> { SyntaxKind.LessThanToken, SyntaxKind.GreaterThanToken } },
                    { SyntaxKind.EqualsEqualsToken,       new List<SyntaxKind> { SyntaxKind.ExclamationEqualsToken } },
                    { SyntaxKind.ExclamationEqualsToken,  new List<SyntaxKind> { SyntaxKind.EqualsEqualsToken } },
                    { SyntaxKind.BarBarToken,             new List<SyntaxKind> { SyntaxKind.AmpersandAmpersandToken } },
                    { SyntaxKind.AmpersandAmpersandToken, new List<SyntaxKind> { SyntaxKind.BarBarToken } }
                };
            }

            public override void VisitBinaryExpression(BinaryExpressionSyntax node)
            {
                if (!_kindToAlternativeKinds.TryGetValue(node.OperatorToken.Kind(), out var alternativeKinds))
                    return;                

                foreach (var kind in alternativeKinds)
                {
                    var mutatedOperator = SyntaxFactory.Token(kind)
                        .WithTrailingTrivia(node.OperatorToken.TrailingTrivia);
                    var mutatedNode = node.WithOperatorToken(mutatedOperator);
                    _nodePairs.Add(new NodePair(node, mutatedNode));
                }
            }
        }
    }
}