using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;

#if UNITY_EDITOR
using WebSocketSharp;
#endif

#if UNITY_WSA && !UNITY_EDITOR
using Windows.Web.Http;
using System.Net.Http;
#else

#endif

namespace OctoprintClient

{
    /// <summary>
    /// is the base Class connecting your project to different parts of Octoprint.
    /// </summary>
    public class OctoprintConnection
    {
        /// <summary>
        /// The end point URL like https://192.168.1.2/
        /// </summary>
        public string EndPoint { get; set; }

        /// <summary>
        /// The end point Api Key like "ABCDE12345"
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the position. of the 3D printer, guesses it if necessary from the GCODE
        /// </summary>
        public OctoprintPos Position { get; set; }
        /// <summary>
        /// Gets or sets files in the Folders of the Octoprint Server
        /// </summary>
        public OctoprintFileTracker Files { get; set; }
        /// <summary>
        /// Starts Jobs or reads progress of the Octoprint Server
        /// </summary>
        public OctoprintJobTracker Jobs { get; set; }

        /// <summary>
        /// Reads the Hardware state, Temperatures and other information.
        /// </summary>
        public OctoprintPrinter Printer { get; set; }

        /// <summary>
        /// Creates a <see cref="T:OctoprintClient.OctoprintConnection"/> 
        /// </summary>
        /// <param name="eP">The endpoint Address like "http://192.168.1.2/"</param>
        /// <param name="aK">The Api Key of the User account you want to use. You can get this in the user settings</param>
        public OctoprintConnection(string eP, string aK)
        {
            SetEndPointDirty(eP);
            ApiKey = aK;
        }


        /// <summary>
        /// Sets the end point from dirty input, checks for common faults.
        /// </summary>
        private void SetEndPointDirty(string eP)
        {
            if (eP.EndsWith("/", StringComparison.Ordinal))
            {
                if (eP.StartsWith("http", StringComparison.Ordinal))
                    EndPoint = eP;
                else
                    EndPoint = "http://" + eP;
            }
            else
            {
                if (eP.StartsWith("http", StringComparison.Ordinal))
                    EndPoint = eP + "/";
                else
                    EndPoint = "http://" + eP + "/";
            }
        }

        /// <summary>
        /// Gets the websocketUrl.
        /// </summary>
        /// <returns>The websocket Url.</returns>
        protected string GetWebsocketurl()
        {
            string result = EndPoint;

            result = result.Replace("http://", "");
            result = result.Replace("https://", "");
            result = "ws://" + result + "sockjs/websocket";

            return result;
        }

        protected void ParseData(string input)
        {
            JObject obj = null;
            try
            {
                obj = JObject.Parse(input);
            }
            catch
            {
                Debug.WriteLine("Couldent parse data. Not enough data?");
            }
            if (obj != null)
            {
                JToken current = obj.Value<JToken>("current");

                if (current != null)
                {
                    JToken progress = current.Value<JToken>("progress");
                    if (progress != null && Jobs.ProgressListens())
                    {
                        OctoprintJobProgress jobprogress = new OctoprintJobProgress(progress);
                        Jobs.CallProgress(jobprogress);
                    }

                    JToken job = current.Value<JToken>("job");
                    if (job != null && Jobs.JobListens())
                    {
                        OctoprintJobInfo jobInfo = new OctoprintJobInfo(job);
                        Jobs.CallJob(jobInfo);
                    }

                    JToken printerinfo = current.Value<JToken>("state");
                    if (printerinfo != null && Printer.StateListens())
                    {
                        OctoprintPrinterState opstate = new OctoprintPrinterState(printerinfo);
                        Printer.CallPrinterState(opstate);
                    }

                    float? currentz = current.Value<float?>("currentZ");
                    if (currentz != null && Printer.ZListens())
                    {
                        Printer.CallCurrentZ((float)currentz);
                    }
                    JToken offsets = current.Value<JToken>("offsets");
                    if (offsets != null && Printer.OffsetListens())
                    {
                        List<int> offsetList = new List<int>();
                        for (int i = 0; i < 256; i++)
                        {
                            int? tooloffset = offsets.Value<int?>("tool" + i);
                            if (tooloffset != null)
                            {
                                offsetList.Add((int)tooloffset);
                            }
                            else
                            {
                                break;
                            }
                        }
                        int? offsetBed = offsets.Value<int?>("bed");
                        if (offsetBed != null)
                        {
                            offsetList.Add((int)offsetBed);
                        }
                        Printer.CallOffset(offsetList);
                    }

                    JToken temps = current.Value<JToken>("temps");
                    if (temps != null && temps.HasValues && Printer.TempsListens())
                    {
                        temps = temps.Value<JToken>(0);
                        Printer.CallTemp(new OctoprintHistoricTemperatureState(temps));

                    }
                }
            }
        }

        internal virtual string Get(string v)
        {
            throw new NotImplementedException();
        }

        internal virtual string Delete(string v)
        {
            throw new NotImplementedException();
        }
        internal virtual string PostJson(string v, JObject data)
        {
            throw new NotImplementedException();
        }

        internal virtual string PostString(string v, string data)
        {
            throw new NotImplementedException();
        }

        internal virtual string PostMultipart(string packagestring, string v)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// The base class for the different Trackers
    /// </summary>
    public class OctoprintBase
    {
        protected OctoprintConnection Connection { get; set; }
        public OctoprintBase(OctoprintConnection con)
        {
            Connection = con;
        }
    }
}