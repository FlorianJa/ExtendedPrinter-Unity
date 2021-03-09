using System;

namespace OctoPrintLib
{
    public class PrintDoneEventArgs: EventArgs
    {
        public readonly string File;

        public PrintDoneEventArgs(string file)
        {
            File = file;
        }
    }
}