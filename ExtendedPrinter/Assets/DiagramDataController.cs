using ChartAndGraph;
using OctoPrintLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiagramDataController : MonoBehaviour
{
    [SerializeField]
    private GraphChart Graph;


    public void OnDataRecieved(CurrentMessage message)
    {
        var tmps = message.current.temps;
        if (tmps.Count > 0)
        {
            Graph.DataSource.AddPointToCategoryRealtime("ToolTarget", System.DateTime.Now, (double)tmps[0].tool0.target, 1f); 
            Graph.DataSource.AddPointToCategoryRealtime("ToolActual", System.DateTime.Now, (double)tmps[0].tool0.actual, 1f);
            Graph.DataSource.AddPointToCategoryRealtime("BedTarget", System.DateTime.Now, (double)tmps[0].bed.target, 1f); 
            Graph.DataSource.AddPointToCategoryRealtime("BedActual", System.DateTime.Now, (double)tmps[0].bed.actual, 1f);
        }
    }
}
