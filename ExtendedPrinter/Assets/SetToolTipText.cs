using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ToolTip))]
public class SetToolTipText : MonoBehaviour
{
    public string[] Texts;
    public int StepCounter = 0;
    private ToolTip toolTip;
    public void Start()
    {
        toolTip = GetComponent<ToolTip>();
    }


    public void SetText(int index)
    {
        if(index < Texts.Length)
        {
            toolTip.ToolTipText = Texts[index];
        }
    }

    public void Next()
    {
        StepCounter++;
        SetText(StepCounter);
    }
}
