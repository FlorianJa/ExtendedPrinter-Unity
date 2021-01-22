using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._ExtendedPrinter.Scripts.SlicingService
{
    public class FileSlicedMessage
    {
        public string MessageType;
        public string Payload;
    }

    public class ProfileListMessage 
    {
        public string MessageType;
        public List<string> Payload;
    }
}
