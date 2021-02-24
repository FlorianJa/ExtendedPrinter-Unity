using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class PlayerSettings
{

    public string OctoprintIP { get; set; }
    public string APIKey { get; set; }
    public string SlicingServiceIP { get; set; }
    public Locale Language { get; set; }
    public bool ShowWelcomeScreen { get; set; }

    public static PlayerSettings GetAllSettings()
    {
        var octoprintIP = PlayerPrefs.GetString("OctoprintIP", string.Empty);
        var aPIKey = PlayerPrefs.GetString("APIKey", string.Empty);
        var slicingServiceIP = PlayerPrefs.GetString("SlicingServiceIP", string.Empty);
        var language = LocalizationSettings.AvailableLocales.GetLocale(PlayerPrefs.GetString("Language", "de"));
        var showWelcomeScreen = PlayerPrefs.GetInt("ShowWelcomeScreen", 1) == 1 ? true : false;

        return new PlayerSettings() { OctoprintIP = octoprintIP, APIKey = aPIKey, SlicingServiceIP = slicingServiceIP, Language = language, ShowWelcomeScreen = showWelcomeScreen };
    }

    public void Save()
    {
        Save(this);
    }

    public void Save(PlayerSettings settings)
    {
        PlayerPrefs.SetString("OctoprintIP", settings.OctoprintIP);
        PlayerPrefs.SetString("APIKey", settings.APIKey);
        PlayerPrefs.SetString("SlicingServiceIP", settings.SlicingServiceIP);
        PlayerPrefs.SetString("Language", LocalizationSettings.SelectedLocale.Identifier.Code);
        PlayerPrefs.SetInt("ShowWelcomeScreen", settings.ShowWelcomeScreen == true ? 1 : 0);
    }
}