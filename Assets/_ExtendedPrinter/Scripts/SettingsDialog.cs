using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SettingsDialog : Dialog
{
    [SerializeField]
    private DialogButton[] Buttons;

    [SerializeField]
    private MRTKTMPInputField OctoprintIPTMP;
    
    [SerializeField] 
    private MRTKTMPInputField SlicingServiceIPTMP;
    
    [SerializeField] 
    private MRTKTMPInputField APIKeyTMP;

    [SerializeField]
    private Interactable GermanButton;
    
    [SerializeField]
    private Interactable EnglishButton;

    [SerializeField]
    private Interactable ShowOnStartUpToggle;

    private static PlayerSettings Settings;
    /// <summary>
    /// Instantiates a dialog and passes it a result
    /// </summary>
    /// <param name="dialogPrefab">Dialog prefab</param>
    /// <param name="result">DialogResult class object which contains information such as title and description text</param>
    public static Dialog Open(GameObject dialogPrefab, DialogResult result, PlayerSettings settings)
    {
        Settings = settings;
        return SettingsDialog.Open(dialogPrefab, result);
    }

    public override void DismissDialog()
    {
        if (result.Result == DialogButtonType.OK)
        {
            Settings.OctoprintIP = OctoprintIPTMP.text;
            Settings.APIKey = APIKeyTMP.text;
            Settings.SlicingServiceIP = SlicingServiceIPTMP.text;
            Settings.Language = LocalizationSettings.AvailableLocales.GetLocale(GermanButton.IsToggled ? "de" : "en");
            Settings.ShowWelcomeScreen = ShowOnStartUpToggle.IsToggled;

            result.Variable = Settings;
        }

        State = DialogState.InputReceived;
    }

    protected override void FinalizeLayout()
    {
    }

    protected override void GenerateButtons()
    {
        foreach (var button in Buttons)
        {
            button.ParentDialog = this;
        }
    }

    protected override void SetTitleAndMessage()
    {
        OctoprintIPTMP.text = Settings.OctoprintIP;
        SlicingServiceIPTMP.text = Settings.SlicingServiceIP;
        APIKeyTMP.text = Settings.APIKey;
        ShowOnStartUpToggle.IsToggled = Settings.ShowWelcomeScreen;
        GermanButton.IsToggled = Settings.Language.Identifier.Code == "de";
        EnglishButton.IsToggled = Settings.Language.Identifier.Code == "en";
    }
}
