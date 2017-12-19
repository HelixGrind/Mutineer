using Microsoft.CodeAnalysis;

namespace Mutineer.Interface
{
    public interface ISourceFile
    {
        SyntaxTree SyntaxTree { get; }
        string Filename { get; }
        int FileIndex { get; }
    }
}
