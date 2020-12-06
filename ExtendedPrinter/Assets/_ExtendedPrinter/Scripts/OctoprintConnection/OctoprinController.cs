using OctoPrintLib;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class OctoprinController : MonoBehaviour
{
    [SerializeField] private string DomainNameOrIP;
    [SerializeField] private string APIKey;
    
    private OctoprintServer octoprintServer;

    public UnityEvent FileAdded;
    public UnityEvent WebsocketConnectedEvent;

    public bool WebsocketConnected
    {
        get
        {
            if(octoprintServer != null)
            {
                return octoprintServer.WebSocketConnected;
            }
            return false;
        }
    }

    void Start()
    {
        octoprintServer = new OctoprintServer(DomainNameOrIP, APIKey);
        octoprintServer.FileAdded += OctoprintServer_FileAdded;
        octoprintServer.WebSocketConnectedEvent += OctoprintServer_WebSocketConnectedEvent;
        var x = octoprintServer.GeneralOperations.Login();
        octoprintServer.StartWebsocketAsync(x.name, x.session);
    }

    private void OctoprintServer_WebSocketConnectedEvent(object sender, System.EventArgs e)
    {
        WebsocketConnectedEvent.Invoke();
    }

    private void OctoprintServer_FileAdded(object sender, FileAddedEventArgs e)
    {
        FileAdded.Invoke();
    }

    private async void OnDisable()
    {
        await octoprintServer.ShutdownAsync();
    }

    [ContextMenu("Fetch all files")]
    public async Task<FolderInformation> FetchAllFilesAsync()
    {
        return await octoprintServer.FileOperations.GetFileInfosInFolderAsync("local", "");
    }
}
