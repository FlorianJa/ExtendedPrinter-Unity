using OctoPrintLib.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace OctoPrintLib
{
    public class FileAddedEventArgs : EventArgs
    {
        public FileAddedEventPayload Payload {get;set;}

        public FileAddedEventArgs(FileAddedEventPayload payload)
        {
            Payload = payload;
        }

    }
}
