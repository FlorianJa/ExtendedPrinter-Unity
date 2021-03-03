namespace OctoPrintLib.DataClasses
{
    public class Tool0
    {
        public double? length { get; set; }
        public double? volume { get; set; }
    }

    public class Filament
    {
        public Tool0 tool0 { get; set; }
    }
}