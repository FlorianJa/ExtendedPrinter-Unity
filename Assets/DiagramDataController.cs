using ChartAndGraph;
using OctoPrintLib;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiagramDataController : MonoBehaviour
{
    [SerializeField]
    private GraphChart Graph;


    [SerializeField]
    private TextMeshProUGUI ToolTargetText;

    [SerializeField]
    private TextMeshProUGUI ToolActualText;
    [SerializeField]
    private TextMeshProUGUI BedTargetText;
    [SerializeField]
    private TextMeshProUGUI BedActualText;

    private float lastTime = 0;
    private float refreshIntervall = 2f;
    public void OnDataRecieved(CurrentMessage message)
    {
        var temps = message.current.temps;
        float time = Time.time;
        if (temps.Count > 0 && lastTime + refreshIntervall < time)
        {
            lastTime = time;

            Graph.DataSource.StartBatch();
            Graph.DataSource.AddPointToCategoryRealtime("ToolTarget", System.DateTime.Now, (double)temps[0].tool0.target, 1f); 
            Graph.DataSource.AddPointToCategoryRealtime("ToolActual", System.DateTime.Now, (double)temps[0].tool0.actual, 1f);
            Graph.DataSource.AddPointToCategoryRealtime("BedTarget", System.DateTime.Now, (double)temps[0].bed.target, 1f); 
            Graph.DataSource.AddPointToCategoryRealtime("BedActual", System.DateTime.Now, (double)temps[0].bed.actual, 1f);
            Graph.DataSource.EndBatch();
            ToolTargetText.text = temps[0].tool0.target.Value.ToString("F1") + " °C";
            ToolActualText.text = temps[0].tool0.actual.Value.ToString("F1") + " °C";
            BedTargetText.text = temps[0].bed.target.Value.ToString("F1") + " °C";
            BedActualText.text = temps[0].bed.actual.Value.ToString("F1") + " °C";
        }
    }
}
