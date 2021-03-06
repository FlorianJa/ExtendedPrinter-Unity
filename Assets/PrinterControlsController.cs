using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PrinterControlsController : MonoBehaviour
{
    public GameObject DialogPrefab;


    public UnityEvent StopPrintCanceled;
    public UnityEvent StopPrint;
    public UnityEvent PausePrint;

    public void OnPrinterStop()
    {
        Dialog myDialog = Dialog.Open(DialogPrefab,  DialogButtonType.No| DialogButtonType.Yes, "Druck abbrechen", "Möchtest du den Druck wirklich abbrechen?", true);
        if (myDialog != null)
        {
            myDialog.OnClosed += OnClosedDialogEvent;
        }
    }

    private void OnClosedDialogEvent(DialogResult obj)
    {
        if (obj.Result == DialogButtonType.No)
        {
            StopPrintCanceled.Invoke();
        }
        else
        {
            StopPrint.Invoke();
        }
    }

    
}
