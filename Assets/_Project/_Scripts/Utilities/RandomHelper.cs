namespace MJM.HG
{
    public static class RandomHelper
    {
        public static int RandomRange(int minInclusive, int maxInclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxInclusive + 1);
        }
    }
}
