using Microsoft.MixedReality.Toolkit.Experimental.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLanguageDialog : Dialog
{

    private SelectLanguageDialogButton[] Buttons;

    private GameObject[] twoButtonSet;


    protected override void FinalizeLayout()
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }
}
