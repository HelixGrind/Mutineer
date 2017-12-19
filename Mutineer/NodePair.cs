using Microsoft.CodeAnalysis;
using Mutineer.Interface;

namespace Mutineer
{
    public class NodePair : INodePair
    {
        public SyntaxNode Original { get; }
        public SyntaxNode Mutated { get; }

        public NodePair(SyntaxNode original, SyntaxNode mutated)
        {
            Original = original;
            Mutated  = mutated;
        }
    }
}
