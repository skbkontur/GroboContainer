namespace GroboContainer.Algorithms.DataStructures
{
    public class Matching
    {
        private readonly int[] matching;

        public Matching(int size)
        {
            matching = new int[size];
            for (int i = 0; i < size; ++i )
            {
                matching[i] = -1;
            }
        }

        public void AddPair(int i, int j)
        {
            matching[i] = j;
        }

        public int GetPair(int i)
        {
            return matching[i];
        }

        public bool HasPair(int i)
        {
            return matching[i] != -1;
        }

        public void RemovePair(int i)
        {
            matching[i] = -1;
        }
    }
}