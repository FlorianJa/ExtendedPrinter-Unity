using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLanguageDialog : Dialog
{
    [SerializeField]
    private SelectLanguageDialogButton[] Buttons;

    public override void DismissDialog()
    {
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
    }
}
