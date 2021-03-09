using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Buildplate2Controller : MonoBehaviour
{
    [SerializeField]
    private Interactable NextButton, BackButton, DemoButton;

    [SerializeField]
    private GameObject DialogPrefab;

    public UnityEvent StartDemo;

    public void EnableButtons(string file)
    {
        if (file == "moveBuildplate.gcode")
        {
            NextButton.IsEnabled = true;
            BackButton.IsEnabled = true;
            DemoButton.IsEnabled = true;
        }
    }

    public void OnStartDemo()
    {
        Dialog myDialog = Dialog.Open(DialogPrefab, DialogButtonType.OK, "Vorsicht", "Nicht erschrecken! Die echte Buildplate wird sich gleich bewegen.", true);
        if (myDialog != null)
        {
            myDialog.OnClosed += OnClosedDialogEvent;
        }
    }

    private void OnClosedDialogEvent(DialogResult obj)
    {
        StartDemo.Invoke();
    }
}
