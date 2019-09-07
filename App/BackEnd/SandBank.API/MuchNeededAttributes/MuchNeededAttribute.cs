using System;

namespace MuchNeededAttributes
{
    public abstract class MuchNeededAttribute : Attribute
    {
        public string Description { get; private set; }

        protected MuchNeededAttribute() : this("") {}

        protected MuchNeededAttribute(string description)
        {
            Description = description;
        }
    }
}