using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._ExtendedPrinter.Scripts.SlicingService
{
    [Serializable]
    public class FileSlicedMessageArgs
    {
        public string File;// { get; set; }
        public string FilamentLength;// { get; set; }
        public string PrintTime;// { get; set; }
    }

    public class FileSlicedMessage
    {
        public string MessageType;
        public FileSlicedMessageArgs Payload;// { get; set; }
    }

    public class ProfileListMessage 
    {
        public string MessageType;
        public List<string> Payload;
    }
}
