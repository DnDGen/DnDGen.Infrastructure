using DnDGen.RollGen;

namespace DnDGen.Infrastructure.Selectors.Collections
{
    internal static class RollConstants
    {
        public const int CommonThreshold = 1;
        public const int UncommonThreshold = 61;
        public const int RareThreshold = 91;
        public const int VeryRareThreshold = 100;

        public static readonly string CommonRoll = RollHelper.GetRollWithMostEvenDistribution(CommonThreshold, 100, true);
        public static readonly string UncommonRoll = RollHelper.GetRollWithMostEvenDistribution(UncommonThreshold, 100, true);
        public static readonly string RareRoll = RollHelper.GetRollWithMostEvenDistribution(RareThreshold, 100, true);
        public static readonly string VeryRareRoll = RollHelper.GetRollWithMostEvenDistribution(VeryRareThreshold, 100, true);
    }
}
