using System.Collections.Generic;

namespace Mutineer.Interface
{
    public interface IProject
    {
        string Name { get; }
        ISourceFile[] Files { get; }
        void GenerateMutations(List<IMutagen> mutagens);
        void CreateMutatedAssemblies();
    }
}
