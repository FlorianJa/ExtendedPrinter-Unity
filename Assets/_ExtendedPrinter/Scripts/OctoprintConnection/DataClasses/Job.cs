using System;
using System.Collections.Generic;
using System.Text;

namespace OctoPrintLib.DataClasses
{
    public class JobBase
    {
        public double? estimatedPrintTime { get; set; }
        public double? averagePrintTime { get; set; }
        public double? lastPrintTime { get; set; }
        public Filament filament { get; set; }

    }

    public class JobInCurrentMessage:JobBase
    {
        public FileInCurrentMessage file { get; set; }
        public string user { get; set; }
    }

    public class JobInHistoryMessage:JobBase
    {
        public FileInHistoryMessage file { get; set; }
    }
}
