namespace Endpoints.Configuration
{
    public class NumberRange
    {
        public int Id { get; set; }
        public NumberRangeType RangeType { get; set; }
        public string Prefix { get; set; }
        public int RangeStart { get; set; }
        public int RangeEnd { get; set; }
        public int LastValue { get; set; }
    }
}