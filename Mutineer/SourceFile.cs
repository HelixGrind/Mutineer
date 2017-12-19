using Microsoft.CodeAnalysis;
using Mutineer.Interface;

namespace Mutineer
{
    public class SourceFile : ISourceFile
    {
        public SyntaxTree SyntaxTree { get; }
        public string Filename { get; }
        public int FileIndex { get; }

        public SourceFile(SyntaxTree syntaxTree, string filename, int fileIndex)
        {
            SyntaxTree = syntaxTree;
            Filename   = filename;
            FileIndex  = fileIndex;
        }
    }
}
