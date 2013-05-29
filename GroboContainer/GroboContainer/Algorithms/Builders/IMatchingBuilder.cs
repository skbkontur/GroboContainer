using GroboContainer.Algorithms.DataStructures;

namespace GroboContainer.Algorithms.Builders
{
    public interface IMatchingBuilder
    {
        bool TryBuild(BipartiteGraph graph, out Matching matching);
        bool HasAnotherMatching(BipartiteGraph graph, Matching matching);
    }
}