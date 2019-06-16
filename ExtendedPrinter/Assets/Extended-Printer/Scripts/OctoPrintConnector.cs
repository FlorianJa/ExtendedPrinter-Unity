using ChartAndGraph;
using Microsoft.MixedReality.Toolkit.UI;
using OctoprintClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class OctoPrintConnector : MonoBehaviour
{

    private OctoprintConnection octoprintConnection;
    public string Ip;
    public string ApiKey;
    public GraphChart Graph;

    public ToolTip toolTip;
    public GameObject Legende;

    public GameObject Video;
    public GameObject SelectionMenu;
    public GameObject OperatingButtons;
    public GameObject ChangeFilamentNext;
    public GameObject StartPrintButton;
    public GameObject StarFilamentChangeButton;

    private bool isPrinting = false;
    private bool isFilamentChanging;
    private bool filamentChangeBegin;
    private bool videoIsPlaying;
    private bool filamentChangeEnd;

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

    private void Printer_PrintFinished(object sender, PrintFinishedEventArgs e)
    {
        if (isPrinting)
        {
            isPrinting = false;
            TimeSpan timePrinted = TimeSpan.FromSeconds(e.Time);
            string time1;
            if (timePrinted.Hours > 0)
            {
                time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        timePrinted.Hours,
                                        timePrinted.Minutes,
                                        timePrinted.Seconds);
            }
            else
            {
                time1 = string.Format("{0:D2}m:{1:D2}s",
                                        timePrinted.Minutes,
                                        timePrinted.Seconds);
            }

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                toolTip.ToolTipText = "Druck abgeschlossen.\nDauer: " + time1;
                SelectionMenu.SetActive(true);
                OperatingButtons.SetActive(true);
            });
        }
        if (isFilamentChanging && filamentChangeBegin)
        {
            filamentChangeBegin = false;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                videoIsPlaying = true;
                toolTip.ToolTipText = "Löse den Hebel und zieh das Filament senkrecht heraus.";
                Video.GetComponent<MeshRenderer>().enabled = true;
                Video.GetComponentInChildren<VideoPlayer>().clip = Video.GetComponentInChildren<RotateVideoFiles>().Videos[0];
                Video.GetComponentInChildren<VideoPlayer>().Play();
                ChangeFilamentNext.SetActive(true);
            });
        }
        if (isFilamentChanging && filamentChangeEnd)
        {
            filamentChangeEnd = false;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                toolTip.ToolTipText = "Filamentwechsel abgeschlossen";
                ChangeFilamentNext.SetActive(false);
                StartPrintButton.SetActive(true);
                StarFilamentChangeButton.SetActive(true);
                SelectionMenu.SetActive(true);
            });
        }
    }

    private void Printer_StateChanged(object sender, StateChangedEventArgs e)
    {

    }

    private void Jobs_Progressinfo(OctoprintJobProgress obj)
    {
        if (isPrinting)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                TimeSpan timeLeft = TimeSpan.FromSeconds(obj.PrintTimeLeft);
                TimeSpan timePrinted = TimeSpan.FromSeconds(obj.PrintTime);
                string time1;
                if (timePrinted.Hours > 0)
                {
                    time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                            timePrinted.Hours,
                                            timePrinted.Minutes,
                                            timePrinted.Seconds);
                }
                else
                {
                    time1 = string.Format("{0:D2}m:{1:D2}s",
                                            timePrinted.Minutes,
                                            timePrinted.Seconds);
                }

                string time2;
                if (timeLeft.Hours > 0)
                {
                    time2 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                            timeLeft.Hours,
                                            timeLeft.Minutes,
                                            timeLeft.Seconds);
                }
                else
                {
                    time2 = string.Format("{0:D2}m:{1:D2}s",
                                            timeLeft.Minutes,
                                            timeLeft.Seconds);
                }

                toolTip.ToolTipText = String.Format("Dauer: {0}\n Verbleibend: {1}\n Fortschritt: {2}%", time1, time2, obj.Completion.ToString("F0"));


                //if (obj.Completion == 100)
                //{
                //    isPrinting = false;
                //}
            });
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

    public void MoveXAxisRelativ(float distance)
    {
        MoveAxis(Axis.X, distance, false, 1000);
    }
    public void MoveZAxisRelativ(float distance)
    {
        MoveAxis(Axis.Z, distance, false, 1000);
    }

    public void MoveYAxisRelativ(float distance)
    {
        MoveAxis(Axis.Y, distance, false, 1000);
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

                    Legende.transform.GetChild(0).GetComponentInChildren<Text>().text = "ToolTarget: " + obj.Tools[0].Target.ToString("F0") + "°C";
                    Legende.transform.GetChild(1).GetComponentInChildren<Text>().text = "BedTarget: " + obj.Bed.Target.ToString("F0") + "°C";
                    Legende.transform.GetChild(2).GetComponentInChildren<Text>().text = "Tool: " + obj.Tools[0].Actual.ToString("F0") + "°C";
                    Legende.transform.GetChild(3).GetComponentInChildren<Text>().text = "Bed: " + obj.Bed.Actual.ToString("F0") + "°C";
                }

            });
        }
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
        octoprintConnection.Jobs.StartJob();
        isPrinting = true;
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
