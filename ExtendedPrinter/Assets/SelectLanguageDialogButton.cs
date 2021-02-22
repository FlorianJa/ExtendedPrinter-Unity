using Microsoft.MixedReality.Toolkit.Experimental.Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLanguageDialogButton : DialogButton
{
    [SerializeField]
    private string Locale;

    /// <summary>
    /// Event handler that runs when button is clicked.
    /// Dismisses the parent dialog.
    /// </summary>
    /// <param name="obj">Caller GameObject</param>
    public new void OnButtonClicked(GameObject obj)
    {
        if (ParentDialog != null)
        {
            ParentDialog.Result.Result = ButtonTypeEnum;
            ParentDialog.Result.Variable = Locale;
            ParentDialog.DismissDialog();
        }
    }
}
