namespace Temperature.Models
{
    public class DriveParameter
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        public string RegisterNo { get; set; }
        public ParameterValueRange ValueRange { get; set; }
        public string Unit { get; set; }
        public string Value { get; set; }
    }

    public class ParameterValueRange
    {
        public double minValue { get; set; }
        public double maxValue { get; set; }
    }
}