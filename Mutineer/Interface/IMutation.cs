using Microsoft.CodeAnalysis;

namespace Mutineer.Interface
{
    public interface IMutation
    {
        ISourceFile File { get; }
        MutationState Result { get; set; }
        INodePair NodePair { get; }
        SyntaxTree[] SyntaxTrees { get; set; }
    }
}
