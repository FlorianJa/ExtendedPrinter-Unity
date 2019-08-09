using Microsoft.MixedReality.Toolkit.UI;
using OctoprintClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ToolTip))]
public class SetToolTipText : MonoBehaviour
{
    public string[] Texts;
    public int StepCounter = 0;
    private ToolTip toolTip;
    public void Start()
    {
        toolTip = GetComponent<ToolTip>();
        OctoPrintConnector.Instance.PrintFinished += setPrintFinishedText;
        OctoPrintConnector.Instance.FilamentChangeBegin += setFilamentChangeBeginText;
        OctoPrintConnector.Instance.FilamentChangeEnd += setFilamentChangeEndText;
        OctoPrintConnector.Instance.ProgressInfoChanged += updateProgressText;
    }

    private void setPrintFinishedText(object source, System.EventArgs args)
    {
        PrintFinishedEventArgs e=(PrintFinishedEventArgs)args;
        System.TimeSpan timePrinted = System.TimeSpan.FromSeconds(e.Time);
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
        });
    }
    private void setFilamentChangeBeginText(object source, System.EventArgs args)
    {

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            toolTip.ToolTipText = "Löse den Hebel und zieh das Filament senkrecht heraus.";
        });
    }
    private void setFilamentChangeEndText(object source, System.EventArgs args)
    {

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            toolTip.ToolTipText = "Filamentwechsel abgeschlossen";
        });
    }
    private void updateProgressText(object source, OctoprintJobProgress obj)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            System.TimeSpan timeLeft = System.TimeSpan.FromSeconds(obj.PrintTimeLeft);
            System.TimeSpan timePrinted = System.TimeSpan.FromSeconds(obj.PrintTime);
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

            toolTip.ToolTipText = System.String.Format("Dauer: {0}\n Verbleibend: {1}\n Fortschritt: {2}%", time1, time2, obj.Completion.ToString("F0"));


            //if (obj.Completion == 100)
            //{
            //    isPrinting = false;
            //}
        });
    }
    public void SetText(int index)
    {
        if(index < Texts.Length)
        {
            toolTip.ToolTipText = Texts[index];
        }
        else
        {
            index = 0;
        }
    }

    public void Next()
    {
        StepCounter++;
        SetText(StepCounter);
    }
}
