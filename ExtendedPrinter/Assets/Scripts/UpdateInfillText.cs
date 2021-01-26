using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class UpdateInfillText : MonoBehaviour
{

    private TextMeshPro textMeshPro;

    private void Start()
    {
        textMeshPro = GetComponent<TextMeshPro>();
    }
    public void UpdateText(SliderEventData data)
    {
        textMeshPro.text = ((int)(data.NewValue*100)).ToString() + "%";
    }
}
