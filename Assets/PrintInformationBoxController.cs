using OctoPrintLib;
using System;
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
        if (FileName.text != data.current.job.file.name) FileName.text = data.current.job.file.name;
        PrintTime.text = GetTimeStringFromInt(data.current.progress.printTime);
        TimeLeft.text = GetTimeStringFromInt(data.current.progress.printTimeLeft);

    }

    private string GetTimeStringFromInt(int? seconds)
    {
        if (!seconds.HasValue)
        {
            return string.Empty;
        }

        TimeSpan time = TimeSpan.FromSeconds(seconds.Value);

        //here backslash is must to tell that colon is
        //not the part of format, it just a character that we want in output
        return time.ToString(@"hh\:mm\:ss");
    }
}
