using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mutineer.Interface;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Mutineer.Walkers
{
    public class OperatorTokenMutagen : IMutagen
    {
        public List<INodePair> NodePairs { get; }
        public CSharpSyntaxWalker Walker { get; }

        public OperatorTokenMutagen()
        {
            NodePairs = new List<INodePair>();
            Walker    = new OperatorTokenWalker(NodePairs);
        }

        private class OperatorTokenWalker : CSharpSyntaxWalker
        {
            private readonly List<INodePair> _nodePairs;

            public OperatorTokenWalker(List<INodePair> nodePairs)
            {
                _nodePairs = nodePairs;
            }

            public override void VisitIfStatement(IfStatementSyntax node)
            {
                switch (node.Condition)
                {
                    case IdentifierNameSyntax _:
                    {
                        var mutatedCondition = PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, node.Condition);
                        var mutatedNode = node.WithCondition(mutatedCondition);
                        _nodePairs.Add(new NodePair(node, mutatedNode));
                        break;
                    }
                    case PrefixUnaryExpressionSyntax condition:
                    {
                        var mutatedNode = node.WithCondition(condition.Operand);
                        _nodePairs.Add(new NodePair(node, mutatedNode));
                        break;
                    }
                }

                base.VisitIfStatement(node);
            }
        }
    }
}