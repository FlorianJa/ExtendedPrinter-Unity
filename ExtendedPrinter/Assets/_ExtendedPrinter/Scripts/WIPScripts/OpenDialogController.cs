using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class OpenDialogController : MonoBehaviour
{
    public GameObject SettingsDialogPrefab;

    public DialogResult result;

    // Start is called before the first frame update
    void Start()
    {
        OnOpenDialog();
    }

    public void OnOpenDialog()
    {
        result = new DialogResult();

        Dialog myDialog = SettingsDialog.Open(SettingsDialogPrefab, result, PlayerSettings.GetAllSettings());

        if (myDialog != null)
        {
            myDialog.OnClosed += OnClosedDialogEvent;
        }
    }

    private void OnClosedDialogEvent(DialogResult obj)
    {
        if (obj.Result == DialogButtonType.OK)
        {
            var settings = (PlayerSettings)obj.Variable;
            settings.Save();
        }
        //LocalizationSettings.SelectedLocale = (Locale)obj.Variable;
    }
}
