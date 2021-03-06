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
    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
        if (LocalizationSettings.InitializationOperation.IsDone)
        {
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
            {
                var locale = LocalizationSettings.AvailableLocales.Locales[i];
                print("L :" + locale.name);
            }
        }
    }

    public void OnOpenDialog()
    {
        result = new DialogResult();
        var settings = PlayerSettings.GetAllSettings();

        Debug.Log(settings.OctoprintIP);
        Debug.Log(settings.APIKey);
        Debug.Log(settings.SlicingServiceIP);
        Debug.Log(settings.Language.Identifier.Code);
        Debug.Log(settings.ShowWelcomeScreen);

        Dialog myDialog = SettingsDialog.Open(SettingsDialogPrefab, result, settings);

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
