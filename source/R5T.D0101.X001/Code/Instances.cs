using System;

using R5T.T0042;
using R5T.T0055;
using R5T.T0060;


namespace R5T.D0101.X001
{
    public static class Instances
    {
        public static IGuidOperator GuidOperator { get; } = T0055.GuidOperator.Instance;
        public static IPredicate Predicate { get; } = T0060.Predicate.Instance;
        public static ISelector Selector { get; } = T0060.Selector.Instance;
        public static IStringOperator StringOperator { get; } = T0042.StringOperator.Instance;
    }
}
