namespace GroboContainer.Algorithms.DataStructures
{
    public class BipartiteGraph
    {
        private readonly int firstPartSize;
        private readonly int secondPartSize;
        private readonly int[][] matrix;

        public BipartiteGraph(int firstPartSize, int secondPartSize)
        {
            this.firstPartSize = firstPartSize;
            this.secondPartSize = secondPartSize;

            matrix = new int[FirstPartSize][];
            for(int k =  0; k < FirstPartSize; ++k)
                matrix[k] = new int[SecondPartSize];
        }

        public int FirstPartSize
        {
            get { return firstPartSize; }
        }

        public int SecondPartSize
        {
            get { return secondPartSize; }
        }

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
    }
}