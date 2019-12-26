using System;

namespace MuchNeededAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = true)]
    public class HardcodedValueAttribute : MuchNeededAttribute
    {
        public HardcodedValueAttribute(string hardcodedValue) : base(hardcodedValue) {}
    }
}