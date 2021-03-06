using Assets._ExtendedPrinter.Scripts.Helper;
using OctoPrintLib;
using OctoPrintLib.File;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class OctoprintController : MonoBehaviour
{
    [SerializeField] private string DomainNameOrIP;
    [SerializeField] private string APIKey;
    
    private OctoprintServer octoprintServer;

    public UnityEvent FileAdded;
    public UnityEvent WebsocketConnectedEvent;
    public CurrentMessageEvent CurrentMessageReceived;

    public UnityEvent PrintPaused, PrintResumed, PrintStopped, PrintStarted;

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
        octoprintServer.CurrentDataReceived += OctoprintServer_CurrentDataReceived;
        octoprintServer.WebSocketConnectedEvent += OctoprintServer_WebSocketConnectedEvent;
        var x = octoprintServer.GeneralOperations.Login();
        octoprintServer.StartWebsocketAsync(x.name, x.session);
    }

    private void OctoprintServer_CurrentDataReceived(object sender, CurrentMessageEventArgs e)
    {
        CurrentMessageReceived.Invoke(e.Message);
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
    
    public async Task<bool> DownloadFileAsync(string fileName)
    {
        var fileinfo = await octoprintServer.FileOperations.GetFileInfoAsync(fileName);

        if(fileinfo != null)
        {
            if(!Directory.Exists(Path.Combine(Application.persistentDataPath, "Downloads")))
            {
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Downloads"));
            }

            return await octoprintServer.FileOperations.DownloadFileAsync(fileName, Path.Combine(Application.persistentDataPath, "Downloads",fileName));
        }
        return false;
    }

    public async Task<OctoprintFile> GetFileInfo(string file)
    {
        return await octoprintServer.FileOperations.GetFileInfoAsync(file);
    }

    public async void PausePrintAsync()
    {
        var result = await octoprintServer.JobOperations.PausePrintAsync();

        if (result) PrintPaused.Invoke();
    }

    public async void ResumePrintAsync()
    {
        var result = await octoprintServer.JobOperations.ResumePrintAsync();

        if (result) PrintResumed.Invoke();
    }

    public async void StopPrintAsync()
    {
        var result = await octoprintServer.JobOperations.StopPrintAsync();

        if (result) PrintStopped.Invoke();
    }
    public async void StartPrintAsync()
    {
        var result = await octoprintServer.JobOperations.StartPrintAsync();

        if (result) PrintStarted.Invoke();
    }
}
