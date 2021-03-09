using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace OctoPrintLib.Messages
{
    [Serializable]
    public class Payload
    {
        public string storage;
        public string path;
        public string name;

        public List<string> type;
        public string username;
        public string remoteAddress;
        public string state_id;
        public string state_string;
}
    [Serializable]
    public class Event
    {
        public string type;
        public Payload payload;
    }
    [Serializable]
    public class OctoprintEvent
    {
        [JsonProperty("event")]
        public Event Event;
    }
    [Serializable]
    public class FileAddedEventPayload
    {
        public string storage { get; set; }
        public string path { get; set; }
        public string name { get; set; }
        public List<string> type { get; set; }
    }
    [Serializable]
    public class FileAddedEvent
    {
        public string type { get; set; }
        public FileAddedEventPayload payload { get; set; }
    }
    [Serializable]
    public class FileAddedEventMessage
    {
        [JsonProperty("event")]
        public FileAddedEvent Event { get; set; }
    }


    [Serializable]
    public class FileUploadPayload
    {
        public string name { get; set; }
        public string path { get; set; }
        public string target { get; set; }
        public bool select { get; set; }
        public bool print { get; set; }
    }
    [Serializable]
    public class FileUploadEventMessage
    {
        public string type { get; set; }
        public FileUploadPayload payload { get; set; }
    }
    [Serializable]
    public class FileUploadEvent
    {
        [JsonProperty("event")]
        public FileUploadEventMessage Event { get; set; }
    }


    [Serializable]
    public class PrintDoneEvent
    {
        public string type { get; set; }
        public PrintDoneEventPayload payload { get; set; }
    }
    [Serializable]
    public class PrintDoneEventMessage
    {
        [JsonProperty("event")]
        public PrintDoneEvent Event { get; set; }
    }
    [Serializable]
    public class PrintDoneEventPayload
    {
        public string origin { get; set; }
        public string name { get; set; }
        public float time { get; set; }
        public string owner { get; set; }
        public string path { get; set; }
        public int size { get; set; }
    }

}
