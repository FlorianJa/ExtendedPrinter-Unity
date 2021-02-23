using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class OpenDialogController : MonoBehaviour
{
    public GameObject SelectLanguageDialogPrefab;

    public DialogResult result;

    // Start is called before the first frame update
    void Start()
    {
        OnOpenDialog();
    }

    public void OnOpenDialog()
    {
        result = new DialogResult();

        Dialog myDialog = SelectLanguageDialog.Open(SelectLanguageDialogPrefab, result);

        if (myDialog != null)
        {
            myDialog.OnClosed += OnClosedDialogEvent;
        }
    }

    private void OnClosedDialogEvent(DialogResult obj)
    {
        LocalizationSettings.SelectedLocale = (Locale)obj.Variable;
    }
}
