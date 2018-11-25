using GroboContainer.Algorithms.DataStructures;

namespace GroboContainer.Algorithms.Builders
{
    public class MatchingBuilder : IMatchingBuilder
    {
        public bool TryBuild(BipartiteGraph graph, out Matching matching)
        {
            matching = new Matching(graph.SecondPartSize);
            for (var i = 0; i < graph.FirstPartSize; ++i)
            {
                var used = new bool[graph.FirstPartSize];
                if (!TryKuhn(i, used, graph, matching))
                    return false;
            }
            return true;
        }

        public bool HasAnotherMatching(BipartiteGraph graph, Matching matching)
        {
            for (var i = 0; i < graph.SecondPartSize; i++)
            {
                if (matching.HasPair(i))
                {
                    var pair = matching.GetPair(i);
                    matching.RemovePair(i);
                    graph.RemoveEdge(pair, i);
                    var used = new bool[graph.FirstPartSize];
                    if (TryKuhn(pair, used, graph, matching))
                    {
                        matching.AddPair(i, pair);
                        graph.AddEdge(pair, i);
                        return true;
                    }
                    matching.AddPair(i, pair);
                    graph.AddEdge(pair, i);
                }
            }
            return false;
        }

        private bool TryKuhn(int v, bool[] used, BipartiteGraph bipartiteGraph, Matching matching)
        {
            if (used[v]) return false;
            used[v] = true;
            for (var i = 0; i < bipartiteGraph.SecondPartSize; ++i)
            {
                if (bipartiteGraph.IsConnected(v, i))
                {
                    var to = i;
                    if (!matching.HasPair(to) || TryKuhn(matching.GetPair(to), used, bipartiteGraph, matching))
                    {
                        matching.AddPair(to, v);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}