using ChartAndGraph;
using Microsoft.MixedReality.Toolkit.UI;
using OctoprintClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class OctoPrintConnector : Singleton<OctoPrintConnector>
{
    protected OctoPrintConnector() { }
    private OctoprintConnection octoprintConnection;
    public string Ip;
    public string ApiKey;

    //public ToolTip toolTip;
    
    
    private bool isPrinting = false;
    private bool isFilamentChanging;
    private bool filamentChangeBegin;
    private bool videoIsPlaying;
    private bool filamentChangeEnd;
    private bool isMovedManually;

    public event EventHandler MoveCompleted;

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
        print("octoprintConnection");
#else
        octoprintConnection = new OctoprintConnectionEditor(Ip, ApiKey);

#endif
        Connected = true;
        print("Start octoprintConnector");
        octoprintConnection.Printer.TempHandlers += Printers_TempHandlers;
        octoprintConnection.Printer.PrinterstateHandlers += Printers_PrinterstateHandlers;
        octoprintConnection.Printer.Homed += Printer_Homed;
        octoprintConnection.Jobs.ProgressinfoHandler += Jobs_Progressinfo;
        octoprintConnection.Printer.StateChanged += Printer_StateChanged;
        octoprintConnection.Printer.PrintFinished += Printer_PrintFinished;

    }


    public event EventHandler PrintFinished;
    public event EventHandler FilamentChangeBegin;
    public event EventHandler FilamentChangeEnd;

    private void Printer_PrintFinished(object sender, PrintFinishedEventArgs e)
    {
        if (isPrinting)
        {
            PrintFinished?.Invoke(this,e);
            isPrinting = false;
            
        }
        if (isFilamentChanging && filamentChangeBegin)
        {
            FilamentChangeBegin?.Invoke(this,e);
            filamentChangeBegin = false;
            videoIsPlaying = true;
        }
        if (isFilamentChanging && filamentChangeEnd)
        {
            FilamentChangeEnd?.Invoke(this,e);
            filamentChangeEnd = false;
        }
        if(isMovedManually)
        {

            isMovedManually = false;
            MoveCompleted?.Invoke(this, null);
        }
    }

    private void Printer_StateChanged(object sender, StateChangedEventArgs e)
    {

    }

    public event EventHandler<OctoprintJobProgress> ProgressInfoChanged;
    private void Jobs_Progressinfo(OctoprintJobProgress obj)
    {
        if (isPrinting)
        {
            ProgressInfoChanged?.Invoke(this, obj);
        }
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

    public void MovePrinter(Vector3 position, bool absolute, bool moveX = true, bool moveY = true, bool moveZ = true)
    {
        isMovedManually = true;
        StringBuilder sb = new StringBuilder();


        if(absolute)
        {
            sb.AppendLine("G90");
        }
        else
        {
            sb.AppendLine("G91");
        }

        sb.Append("G1 ");
        if (moveX) sb.Append("X" + position.x.ToString("F2", CultureInfo.GetCultureInfo("en-US")) + " ");
        if (moveY) sb.Append("Y" + position.y.ToString("F2", CultureInfo.GetCultureInfo("en-US")) + " ");
        if (moveZ) sb.Append("Z" + position.z.ToString("F2", CultureInfo.GetCultureInfo("en-US")) + " ");  
        sb.AppendLine("F6000");

        sb.AppendLine("M400");

        if(!absolute)
        {
            sb.AppendLine("G90");
        }
        var filename = Application.persistentDataPath + @"\customMove.gcode";
        if (File.Exists(filename))
        {

        }

        using (StreamWriter sw = File.CreateText(filename))
        {
            sw.Write(sb.ToString());
        }

        octoprintConnection.Files.UploadFile(filename, "customMove.gcode", "helper", "local", false, false);
        octoprintConnection.Files.Select("customMove.gcode", "local/helper", true);

    }
    public void MovePrintHeadUp()
    {
        if (isMovedManually == false)
        {
            isMovedManually = true;

            octoprintConnection.Files.Select("moveUp.gcode", "local/helper", true);
        }
    }
    public void MovePrintHeadDown()
    {
        if (isMovedManually == false)
        {
            isMovedManually = true;
            octoprintConnection.Files.Select("moveDown.gcode", "local/helper", true);
        }
    }
    public void MovePrintHeadRight()
    {
        if (isMovedManually == false)
        {
            isMovedManually = true;
            octoprintConnection.Files.Select("moveRight.gcode", "local/helper", true);
        }
    }
    public void MovePrintHeadLeft()
    {
        if (isMovedManually == false)
        {
            isMovedManually = true;
            octoprintConnection.Files.Select("moveLeft.gcode", "local/helper", true);
        }
    }
    public void MoveBuildplateFront()
    {

        isMovedManually = true;
        octoprintConnection.Files.Select("moveFront.gcode", "local/helper", true);
    }
    public void MoveBuildplateBack()
    {
        if (isMovedManually == false)
        {
            isMovedManually = true;
            octoprintConnection.Files.Select("moveBack.gcode", "local/helper", true);
        }
    }

    public void HomeFile()
    {

        isMovedManually = true;
        octoprintConnection.Files.Select("Home.gcode", "local/helper", true);
    }

    public void SetExtruderTemp(int to)
    {
        octoprintConnection.Printer.SetTemperatureTarget(to);
    }

    public double GetExtruderTemp()
    {
        return octoprintConnection.Printer.GetFullPrinterState().TempState.Tools[0].Actual;
    }

    private void Printers_PrinterstateHandlers(OctoprintPrinterState obj)
    {

    }


    /// <summary>
    /// Action for Eventhandling the Websocket Temperature info
    /// </summary>
    public event EventHandler<OctoprintHistoricTemperatureState> NewTemperatureDataRecieved;

    private void Printers_TempHandlers(OctoprintHistoricTemperatureState obj)
    {
        NewTemperatureDataRecieved?.Invoke(this,obj);
    }


    public void HomePrinter()
    {
        octoprintConnection.Printer.HomePrinter();
    }

    public void SelectFile(string path)
    {
        octoprintConnection.Files.Select(path);
    }

    public void StartPrint()
    {
        if (!isMovedManually)
        {
            octoprintConnection.Jobs.StartJob();
            isPrinting = true;
        }
    }

    public void StartFilamentChange()
    {
        if (!isPrinting && !isFilamentChanging)
        {
            videoIsPlaying = false;
            isFilamentChanging = true;
            //octoprintConnection.Printer.HomePrinter();
            //octoprintConnection.Printer.MakePrintheadJog(140, 30, 40, true, 6000);
            ////octoprintConnection.Printer.SelectTool("tool0");
            //octoprintConnection.Printer.SetTemperatureTarget(200);
            filamentChangeBegin = true;
            octoprintConnection.Files.Select("changeFilament.gcode", "local/helper", true);
        }
    }

    public void StopFilamentChange()
    {
        isFilamentChanging = false;
        octoprintConnection.Printer.SetTemperatureTarget(0);
        octoprintConnection.Files.SelectPreviousFile();
    }

    public void FilamentExtrusion()
    {
        if(isFilamentChanging)
        {
            filamentChangeEnd = true;
            octoprintConnection.Files.Select("extrudFliament.gcode", "local/helper", true);
        }
    }
}
