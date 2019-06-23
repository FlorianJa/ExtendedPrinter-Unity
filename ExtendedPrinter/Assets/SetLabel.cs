using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLabel : MonoBehaviour
{
    public string[] Texts;

    public TextMesh label;

    public int StepCounter = 0;

    public int StepForextrusion;
    public OctoPrintConnector OctoPrintConnector;


    public void SetLabelText(int index)
    {
        if(index < Texts.Length)
        {
            label.text = Texts[index];
        }

        if(index == StepForextrusion)
        {
            OctoPrintConnector.FilamentExtrusion();
        }
    }

    public void Next()
    {
        StepCounter++;
        SetLabelText(StepCounter);
    }


}
