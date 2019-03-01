using ChartAndGraph;
using OctoprintClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctoPrintConnectior : MonoBehaviour {

    OctoprintConnection octoprintConnection;
    public string Ip;
    public string ApiKey;
    public GraphChart Graph;

    // Use this for initialization
    void Start ()
    {
#if UNITY_WSA && !UNITY_EDITOR
        octoprintConnection = new OctoprintConnectionUWP(Ip, ApiKey);
#else
        octoprintConnection = new OctoprintConnectionEditor(Ip, ApiKey);
#endif

        octoprintConnection.Printers.TempHandlers += Printers_TempHandlers;
        octoprintConnection.Printers.PrinterstateHandlers += Printers_PrinterstateHandlers;
    }

    private void Printers_PrinterstateHandlers(OctoprintPrinterState obj)
    {

    }

    private void Printers_TempHandlers(OctoprintHistoricTemperatureState obj)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Graph.DataSource.AddPointToCategoryRealtime("ToolTarget", System.DateTime.Now, obj.Tools[0].Target, 1f); 
            Graph.DataSource.AddPointToCategoryRealtime("BedTarget", System.DateTime.Now, obj.Bed.Target, 1f); 
            Graph.DataSource.AddPointToCategoryRealtime("Tool", System.DateTime.Now, obj.Tools[0].Actual, 1f); 
            Graph.DataSource.AddPointToCategoryRealtime("Bed", System.DateTime.Now, obj.Bed.Actual, 1f);
        });
    }
}
