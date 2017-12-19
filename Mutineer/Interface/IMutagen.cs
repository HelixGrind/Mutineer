using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Mutineer.Interface
{
    public interface IMutagen
    {
        List<INodePair> NodePairs { get; }
        CSharpSyntaxWalker Walker { get; }
    }
}
