using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OctoPrintLib
{
    public class OctoprintJobOperation: OctoprintConnection
    {

        public OctoprintJobOperation(OctoprintServer server) : base(server)
        {

        }

        private async Task<bool> PostAsync(JObject data)
        {
            try
            {
                _ = await PostJsonAsync("api/job", data);
                return true;
            }
            catch (WebException e)
            {
                return false;

            }
        }

        public async Task<bool> PausePrintAsync()
        {
            JObject data = new JObject
            {
                { "command", "pause" },
                { "print", "pause"}
            };
            return await PostAsync(data);
        }

        

        public async Task<bool> ResumePrintAsync()
        {
            JObject data = new JObject
            {
                { "command", "pause" },
                { "print", "resume"}
            };

            return await PostAsync(data);
        }

        public async Task<bool> StopPrintAsync()
        {
            JObject data = new JObject
            {
                { "command", "cancel" }
            };

            return await PostAsync(data);
        }
        public async Task<bool> StartPrintAsync()
        {
            JObject data = new JObject
            {
                { "command", "start" }
            };

            return await PostAsync(data);
        }
    }
}