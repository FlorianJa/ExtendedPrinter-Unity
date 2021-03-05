using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterControlsController : MonoBehaviour
{
    public GameObject DialogPrefab;


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
    }

    public void OnPrinterPause()
    {
        
    }
}
