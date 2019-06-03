using ChartAndGraph;
using OctoprintClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctoPrintConnector : MonoBehaviour
{

    private OctoprintConnection octoprintConnection;
    public string Ip;
    public string ApiKey;
    public GraphChart Graph;

    public bool Connected
    {
        get; private set;
    }

    public event EventHandler<HomedEventArgs> PositionChanged;

    // Use this for initialization
    void Start()
    {
        Connected = false;
#if UNITY_WSA && !UNITY_EDITOR
        octoprintConnection = new OctoprintConnectionUWP(Ip, ApiKey);
#else
        octoprintConnection = new OctoprintConnectionEditor(Ip, ApiKey);
        Connected = true;
#endif

        octoprintConnection.Printer.TempHandlers += Printers_TempHandlers;
        octoprintConnection.Printer.PrinterstateHandlers += Printers_PrinterstateHandlers;
        octoprintConnection.Printer.Homed += Printer_Homed;
    }

    /// <summary>
    /// returns all files on octoprint server in folder hierarchy format
    /// </summary>
    /// <returns></returns>
    public OctoprintFolder GetAllFiles()
    {
        return octoprintConnection.Files.GetFiles();
    }

    private void Printer_Homed(object sender, HomedEventArgs e)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            PositionChanged?.Invoke(this, e);
        });
    }

    public void MoveAxis(Axis Axis, float position, bool absolute, int? speed)
    {
        if (Axis == Axis.X)
        {
            octoprintConnection.Printer.MakePrintheadJog(position, null, null, absolute, speed);
        }
        if (Axis == Axis.Y)
        {
            octoprintConnection.Printer.MakePrintheadJog(null, position, null, absolute, speed);
        }
        if (Axis == Axis.Z)
        {
            octoprintConnection.Printer.MakePrintheadJog(null, null, position, absolute, speed);
        }
    }


    private void Printers_PrinterstateHandlers(OctoprintPrinterState obj)
    {

    }

    private void Printers_TempHandlers(OctoprintHistoricTemperatureState obj)
    {
        if (UnityMainThreadDispatcher.Instance() != null)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (Graph != null)
                {
                    Graph.DataSource.AddPointToCategoryRealtime("ToolTarget", System.DateTime.Now, obj.Tools[0].Target, 1f);
                    Graph.DataSource.AddPointToCategoryRealtime("BedTarget", System.DateTime.Now, obj.Bed.Target, 1f);
                    Graph.DataSource.AddPointToCategoryRealtime("Tool", System.DateTime.Now, obj.Tools[0].Actual, 1f);
                    Graph.DataSource.AddPointToCategoryRealtime("Bed", System.DateTime.Now, obj.Bed.Actual, 1f);
                }
            });
        }
    }


    public void HomePrinter()
    {
        octoprintConnection.Printer.HomePrinter();

    }
}
