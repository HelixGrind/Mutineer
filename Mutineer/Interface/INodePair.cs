using Microsoft.CodeAnalysis;

namespace Mutineer.Interface
{
    public interface INodePair
    {
        SyntaxNode Original { get; }
        SyntaxNode Mutated { get; }
    }
}
