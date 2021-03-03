using OctoPrintLib;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrintInformationBoxController : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro FileName;

    [SerializeField]
    private TextMeshPro PrintTime;

    [SerializeField]
    private TextMeshPro TimeLeft;

    [SerializeField]
    private TextMeshPro PrintedLayers;


    public void OnDataREcieved(CurrentMessage data)
    {

    }
}
