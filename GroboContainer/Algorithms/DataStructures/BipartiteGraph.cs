namespace GroboContainer.Algorithms.DataStructures
{
    public class BipartiteGraph
    {
        public BipartiteGraph(int firstPartSize, int secondPartSize)
        {
            this.FirstPartSize = firstPartSize;
            this.SecondPartSize = secondPartSize;

            matrix = new int[FirstPartSize][];
            for (int k = 0; k < FirstPartSize; ++k)
                matrix[k] = new int[SecondPartSize];
        }

        public int FirstPartSize { get; }

        public int SecondPartSize { get; }

        public bool IsConnected(int i, int j)
        {
            return matrix[i][j] == 1;
        }

        public void AddEdge(int i, int j)
        {
            matrix[i][j] = 1;
        }

        public void RemoveEdge(int i, int j)
        {
            matrix[i][j] = 0;
        }

        private readonly int[][] matrix;
    }
}