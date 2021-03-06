using OctoPrintLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class CurrentMessageEventArgs:EventArgs
    {
        public CurrentMessage Message;

        public CurrentMessageEventArgs(CurrentMessage message)
        {
            Message = message;
        }
    }


