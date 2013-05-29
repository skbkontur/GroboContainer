using GroboContainer.Algorithms.Builders;
using GroboContainer.Algorithms.DataStructures;
using NUnit.Framework;

namespace Tests.AlgorithmsTests
{
    public class MatchingBuilderTest : TestBase
    {
        private MatchingBuilder builder;

        public override void SetUp()
        {
            base.SetUp();
            builder = new MatchingBuilder();
        }

        [Test]
        public void TestTryBuildWithEmptyGraph()
        {
            var graph = new BipartiteGraph(1000, 2000);
            Matching matching;
            Assert.IsFalse(builder.TryBuild(graph, out matching));
        }

        [Test]
        public void TestSimpleTryBuild()
        {
            var graph = new BipartiteGraph(1, 2);
            graph.AddEdge(0, 1);
            Matching matching;
            Assert.IsTrue(builder.TryBuild(graph, out matching));
        }

        [Test]
        public void TestTryBuildWithCycleGraph()
        {
            var graph = new BipartiteGraph(2, 2);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 0);
            graph.AddEdge(0, 0);
            graph.AddEdge(1, 1);
            Matching matching;
            Assert.IsTrue(builder.TryBuild(graph, out matching));
        }

        [Test]
        public void TestTryBuild()
        {
            var graph = new BipartiteGraph(2, 2);
            graph.AddEdge(0, 0);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 1);
            Matching matching;
            Assert.IsTrue(builder.TryBuild(graph, out matching));
        }

        [Test]
        public void TestHasAnotherMatching()
        {
            var graph = new BipartiteGraph(2, 2);
            graph.AddEdge(0, 0);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 1);
            var matching = new Matching(2);
            matching.AddPair(0, 0);
            matching.AddPair(1, 1);
            Assert.IsFalse(builder.HasAnotherMatching(graph, matching));
        }

        [Test]
        public void TestHasAnotherMatchingWithCycleGraph()
        {
            var graph = new BipartiteGraph(2, 2);
            graph.AddEdge(0, 0);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 0);
            graph.AddEdge(1, 1);
            var matching = new Matching(2);
            matching.AddPair(0, 0);
            matching.AddPair(1, 1);
            Assert.IsTrue(builder.HasAnotherMatching(graph, matching));
        }

        [Test]
        public void ComplexTest()
        {
            var graph = new BipartiteGraph(5, 5);
            graph.AddEdge(0, 0);
            graph.AddEdge(0, 3);
            graph.AddEdge(4, 0);
            graph.AddEdge(1, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 1);
            graph.AddEdge(4, 2);
            graph.AddEdge(3, 3);
            graph.AddEdge(4, 4);
            Matching matching;
            Assert.IsTrue(builder.TryBuild(graph, out matching));
            
            Assert.IsFalse(builder.HasAnotherMatching(graph, matching));
            graph.AddEdge(1, 4);
            Assert.IsTrue(builder.HasAnotherMatching(graph, matching));
        }
    }
}