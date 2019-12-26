using System;

namespace MuchNeededAttributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class TechnicalDebtAttribute : MuchNeededAttribute
    {
        public TechnicalDebtAttribute(string description) : base(description) {}
    }
}