﻿using Assets._ExtendedPrinter.Scripts.Helper;
using Assets._ExtendedPrinter.Scripts.ModelLoader;
using Microsoft.MixedReality.Toolkit.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._ExtendedPrinter.Scripts.SlicingService
{
    public class SlicingServiceConnection : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The base URL like 192.168.1.2. Do not add protocol like http.")]
        private string DomainNmaeOrIp;

        /// <summary>
        /// The Websocket client which is connected to the slicing service
        /// </summary>
        private ClientWebSocket webSocket { get; set; }

        [Tooltip("OBJLoader to load the objs")]
        public SlicedModelLoader objLoader;

        public List<string> AvailableProfiles { get; private set; }

        public FileSlicedEvent FileSliced;

        private string selectedSlicingConfigFile;
        private float infill = 0.25f;
        private SupportType support = SupportType.None;
        private AdhesionType adhesion = AdhesionType.None;

        async void Start()
        {
            await ConnectWebsocket();
            _ = Task.Run(WebsocketDataReceiverHandler);
        }

        private async Task ConnectWebsocket()
        {
            var canceltoken = CancellationToken.None;
            webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(GetWebsocketURI(), canceltoken);
        }

        /// <summary>
        /// Generates the URL for the websocket connection
        /// </summary>
        /// <returns></returns>
        private Uri GetWebsocketURI()
        {
            return new Uri("ws://" + DomainNmaeOrIp + "/ws");
        }

        [ContextMenu("test")]
        public void ManualTrigger()
        {
            if (Application.isPlaying)
            {
                var cliCommand = new PrusaSlicerCLICommands();
                cliCommand.File = "3DBenchy.stl";
                cliCommand.ExportGCode = true;
                cliCommand.GcodeComments = true;

                WebSocketSend(cliCommand);
            }
        }

        public void SliceModelWithDefaultParameters(string modelName)
        {   
            var cliCommand = PrusaSlicerCLICommands.Default;
            cliCommand.File = modelName;
            cliCommand.ExportGCode = true;
            cliCommand.GcodeComments = true;

            WebSocketSend(cliCommand);
        }

        public void SliceModel(string modelName, Vector2 center, float scale, Vector3 rotation)
        {
            var cliCommand = PrusaSlicerCLICommands.Default;
            cliCommand.File = modelName;
            cliCommand.ExportGCode = true;
            cliCommand.GcodeComments = true;
            cliCommand.Center = center;
            cliCommand.Scale = scale;
            cliCommand.Rotate = rotation.z;
            cliCommand.RotateX = rotation.x;
            cliCommand.RotateY = rotation.y;
            cliCommand.LoadConfigFile = selectedSlicingConfigFile;
            cliCommand.FillDensity = infill;
            cliCommand.SupportMaterial = support == SupportType.Buildeplate || support == SupportType.Everywhere;
            cliCommand.SupportMaterialBuildeplateOnly = support == SupportType.Buildeplate;
            cliCommand.Raft = adhesion == AdhesionType.Raft ? 2 : 0;
            cliCommand.Brim = adhesion == AdhesionType.Brim ? 5 : 0;
            WebSocketSend(cliCommand);
        }
        
        public void SetSlicingConfig(string configName)
        {
            selectedSlicingConfigFile = configName;
        }

        public void SetInfill(SliderEventData sliderData)
        {
            infill = sliderData.NewValue;
        }

        public void SetInfill(float percentage)
        {
            infill = percentage;
        }

        public void SetSupport(SupportTypeSo supportType)
        {
            support = supportType.SupportType;
        }

        public void SetAdhesion(AdhesionTypeSo adhesionType)
        {
            adhesion = adhesionType.AdhesionType;
        }

        /// <summary>
        /// Starts a slicing process with the given commands
        /// </summary>
        /// <param name="prusaSlicerCLICommands"></param>
        public async void WebSocketSend(PrusaSlicerCLICommands prusaSlicerCLICommands)//TODO: rename method
        {
            var json = JsonConvert.SerializeObject(prusaSlicerCLICommands, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var tmp = Encoding.ASCII.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(tmp, 0, json.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// Method to handle incomming data on the websocket
        /// </summary>
        /// <returns></returns>
        private async Task WebsocketDataReceiverHandler()
        {
            var buffer = new byte[8096];
            StringBuilder stringbuilder = new StringBuilder();
            while (!webSocket.CloseStatus.HasValue)
            {
                WebSocketReceiveResult received = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string text = Encoding.UTF8.GetString(buffer, 0, received.Count);

                stringbuilder.Append(text);
                if (received.EndOfMessage)
                {
                    ParseWebsocketData(stringbuilder.ToString());
                    stringbuilder.Clear();
                }
            }
        }

        /// <summary>
        /// Parses the incomming data
        /// </summary>
        /// <param name="data">Data in Json</param>
        private void ParseWebsocketData(string data)
        {
            var _type = GetMessageType(data);
            if (_type == typeof(FileSlicedMessage))
            {
                var tmp = (FileSlicedMessage)JsonUtility.FromJson(data, _type);

                Uri url = new Uri("http://" + DomainNmaeOrIp + tmp.Payload.File);

                UnityThread.executeInUpdate(() => {
                    var gcodeName = url.Segments[url.Segments.Length - 1] + ".gcode";
                    FileSliced?.Invoke(new FileSlicedMessageArgs() {File = gcodeName, FilamentLength = tmp.Payload.FilamentLength, PrintTime = tmp.Payload.PrintTime });
                    objLoader.LoadObjTest(url);
                });

                
            }
            else if (_type == typeof(ProfileListMessage))
            {
                AvailableProfiles = ((ProfileListMessage)JsonUtility.FromJson(data, _type)).Payload;
                selectedSlicingConfigFile = AvailableProfiles[AvailableProfiles.Count-1]; 
            }
        }

        /// <summary>
        /// Gets the Messagetype in the json string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Type GetMessageType(string data)
        {
            var start = data.IndexOf(':') + 2;
            var end = data.IndexOf(',') - 1;
            var length = end - start;
            var type = data.Substring(start, length);

            switch (type)
            {
                case "FileSliced": return typeof(FileSlicedMessage);
                case "Profiles": return typeof(ProfileListMessage);
                default: return null;
            }
        }
    }
}

