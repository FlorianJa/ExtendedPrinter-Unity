using System;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using OctoPrintLib.Operations;
using OctoPrintLib.Messages;
using UnityEngine;
using Assets._ExtendedPrinter.Scripts.Helper;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace OctoPrintLib

{
    /// <summary>
    /// is the base Class connecting your project to different parts of Octoprint.
    /// </summary>
    public class OctoprintServer
    {
        /// <summary>
        /// The base URL like https://192.168.1.2/
        /// </summary>
        public string DomainNmaeOrIp { get; private set; }

        /// <summary>
        /// The end point Api Key like "ABCDE12345"
        /// </summary>
        public string ApplicationKey { get; private set; }

        /// <summary>
        /// The Websocket Client
        /// </summary>
        private ClientWebSocket webSocket { get; set; }
        /// <summary>
        /// Defines if the WebsocketClient is listening and the Tread is running
        /// </summary>
        public volatile bool listening;
        /// <summary>
        /// The size of the web socket buffer. Should work just fine, if the Websocket sends more, it will be split in 4096 Byte and reassembled in this class.
        /// </summary>
        public int WebSocketBufferSize = 4096;

        public event EventHandler<FileAddedEventArgs> FileAdded;
        public event EventHandler WebSocketConnectedEvent;

        public event EventHandler<CurrentMessageEventArgs> CurrentDataReceived;

        public event EventHandler<PrintDoneEventArgs> PrintDone;

        public bool WebSocketConnected
        {
            get
            {
                if (webSocket != null && webSocket.State == WebSocketState.Open)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public OctoprintFileOperation FileOperations { get; private set; }

        public OctoprintGeneral GeneralOperations { get; private set; }


        public OctoprintJobOperation JobOperations { get; private set; }

        private string username;
        
        private string sessionID;

        /// <summary>
        /// Creates a <see cref="T:OctoprintClient.OctoprintConnection"/> 
        /// </summary>
        /// <param name="domainNameOrIP">The endpoint Address like "http://192.168.1.2/"</param>
        /// <param name="aPIKey">The Api Key of the User account you want to use. You can get this in the user settings</param>
        public OctoprintServer(string domainNameOrIP, string aPIKey)
        {
            DomainNmaeOrIp = domainNameOrIP;
            ApplicationKey = aPIKey;
            FileOperations = new OctoprintFileOperation(this);
            GeneralOperations = new OctoprintGeneral(this);
            JobOperations = new OctoprintJobOperation(this);
        }

        public async Task ShutdownAsync()
        {
            listening = false;
            var cancellationToken = new CancellationToken();
            await webSocket.CloseAsync( WebSocketCloseStatus.NormalClosure, "", cancellationToken);

        }

        /// <summary>
        /// Starts the Websocket Thread.
        /// </summary>
        public async Task StartWebsocketAsync(string user, string sessionID)
        {
            if (!listening)
            {
                listening = true;
                this.username = user;
                this.sessionID = sessionID;
                await ConnectWebsocket();
                Task.Run(WebsocketDataReceiverHandler);
            }
        }

        private async Task ConnectWebsocket()
        {
            var canceltoken = CancellationToken.None;
            webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(GetWebsocketURI(),
                canceltoken);

            if(webSocket.State == WebSocketState.Open)
            {
                WebSocketConnectedEvent?.Invoke(this, null);
            }
        }

        private async Task AuthenticateWebsocketAsync(string user, string sessionID)
        {
            
            var json = JsonConvert.SerializeObject(new WebsocketAuthMessage() { auth = user + ":" + sessionID });
            var tmp = Encoding.ASCII.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(tmp, 0, json.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private Uri GetWebsocketURI()
        {
            return new Uri("ws://" + DomainNmaeOrIp + "/sockjs/websocket");
        }

        private async Task WebsocketDataReceiverHandler()
        {
            var buffer = new byte[8096];
            StringBuilder stringbuilder = new StringBuilder();
            while (!webSocket.CloseStatus.HasValue && listening)
            {
                WebSocketReceiveResult received = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string text = Encoding.UTF8.GetString(buffer, 0, received.Count);
                stringbuilder.Append(text);
                if (received.EndOfMessage)
                {
                    await HandleWebSocketDataAsync(stringbuilder.ToString());
                    stringbuilder.Clear();
                }
            }
        }

        private async Task HandleWebSocketDataAsync(string data)
        {
            var messageType = GetMessageType(data);
            //Debug.Log(data);
            switch (messageType)
            {
                case MessageType.Connected:
                    {
                        try
                        {
                            var tmp = JsonUtility.FromJson<WebSocketConnectedMessage>(data);
                        }
                        catch (Exception)
                        {

                            break;
                        }
                        await AuthenticateWebsocketAsync(username, sessionID);
                        break;
                    }
                case MessageType.History:
                    {
                        try
                        {
                            var tmp = JsonUtility.FromJson<HistoryMessage>(data);
                        }
                        catch (Exception)
                        {
                            
                        }
                        break;
                    }
                case MessageType.Event:
                    {
                        try
                        {
                            JObject obj = null;
                            try
                            {
                                obj = JObject.Parse(data);
                            }
                            catch
                            {
                            }

                            JToken _event = obj.Value<JToken>("event");

                            if (_event != null)
                            {
                                string eventtype = string.Empty;
                                try
                                {
                                    eventtype = _event.Value<string>("type");
                                    Debug.Log("Received event: " + eventtype);
                                }
                                catch (Exception)
                                {

                                }

                                if (eventtype == "FileAdded")
                                {
                                    
                                    JToken payloadJtoken = null;
                                    FileAddedEventPayload fileAddedEventPayload;
                                    try
                                    {
                                        payloadJtoken = _event.Value<JToken>("payload");
                                        fileAddedEventPayload = payloadJtoken.ToObject<FileAddedEventPayload>();
                                        UnityThread.executeInUpdate(() =>
                                        {
                                            FileAdded?.Invoke(this, new FileAddedEventArgs(fileAddedEventPayload));
                                        });
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                }
                                else if(eventtype == "PrintDone")
                                {

                                    JToken payloadJtoken = null;
                                    PrintDoneEventPayload printDoneEventPayload;
                                    UnityThread.executeInUpdate(() =>
                                    {
                                        payloadJtoken = _event.Value<JToken>("payload");
                                        printDoneEventPayload = payloadJtoken.ToObject<PrintDoneEventPayload>();
                                        PrintDone?.Invoke(this, new PrintDoneEventArgs(printDoneEventPayload.name));
                                    });
                                }
                            }
                            
                        }
                        catch (Exception)
                        {

                        }
                        break;
                    }
                case MessageType.Current:
                    {
                        try
                        {
                            var tmp = JsonConvert.DeserializeObject<CurrentMessage>(data);
                            UnityThread.executeInUpdate(() =>
                            {
                                CurrentDataReceived?.Invoke(this, new CurrentMessageEventArgs(tmp));
                            }); 

                            //var tmp = JsonUtility.FromJson<CurrentMessage>(data);
                        }
                        catch (Exception e)
                        {

                        }
                        break;
                    }
                case MessageType.Unknown:
                    break;
                default:
                    break;
            }
        }

        private MessageType GetMessageType(string data)
        {
            //Messages start with {"MESSAGETYPE":  -> start index: 2, length: index of : minus 3
            var type = data.Substring(2, data.IndexOf(':') - 3);

            switch (type)
            {
                case "connected": return MessageType.Connected;
                case "event": return MessageType.Event;
                case "history": return MessageType.History;
                case "current":return MessageType.Current;
                default: return MessageType.Unknown;
            }
        }
    }

    public enum MessageType
    {
        Connected,
        Event,
        History,
        Current,
        Unknown
    }
}
