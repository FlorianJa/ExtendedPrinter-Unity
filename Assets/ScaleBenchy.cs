using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleBenchy : MonoBehaviour
{
    

    public void ChangeScale(SliderEventData data)
    {
        int scaleIndex = (int)Math.Round(data.NewValue * 3f);

        float scale = (100f + scaleIndex * 50)/100f;

        transform.localScale = new Vector3(scale, scale, scale);
    }
}
