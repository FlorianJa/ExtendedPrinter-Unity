using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ChangeLanguage : MonoBehaviour
{
    public void OnChangeLanguage(Locale locale)
    {
        LocalizationSettings.SelectedLocale = locale;
    }
}
