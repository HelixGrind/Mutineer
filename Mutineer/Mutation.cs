using Microsoft.CodeAnalysis;
using Mutineer.Interface;

namespace Mutineer
{
    public class Mutation : IMutation
    {
        public ISourceFile File { get; }
        public MutationState Result { get; set; }
        public INodePair NodePair { get; }
        public SyntaxTree[] SyntaxTrees { get; set; }

        public Mutation(ISourceFile file, INodePair nodePair)
        {
            File     = file;
            NodePair = nodePair;
        }
    }
}
