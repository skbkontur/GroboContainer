namespace GroboContainer.Impl.Logging
{
    internal class LogSettings
    {
        static LogSettings()
        {
            depthChange = new int[(int)ItemType.Reused + 1];
            depthChange[(int)ItemType.Constructing] = 1;
            depthChange[(int)ItemType.Get] = 1;
            depthChange[(int)ItemType.Create] = 1;
            depthChange[(int)ItemType.GetAll] = 1;

            depthChange[(int)ItemType.EndGet] = -1;
            depthChange[(int)ItemType.EndCreate] = -1;
            depthChange[(int)ItemType.EndGetAll] = -1;
            depthChange[(int)ItemType.Constructed] = -1;

            depthChange[(int)ItemType.Reused] = 0;
        }

        public static readonly int[] depthChange;
    }
}