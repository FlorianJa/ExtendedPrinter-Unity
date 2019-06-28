using ChartAndGraph;
using OctoprintClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctoListenerGraph : MonoBehaviour
{
    // Start is called before the first frame update
    public GraphChart Graph;
    void Start()
    {
        OctoPrintConnector.Instance.TempHandler += Printers_TempHandler;
        Graph=GetComponent<GraphChart>();
    }
    private void Printers_TempHandler(OctoprintHistoricTemperatureState obj)
    {

        if (UnityMainThreadDispatcher.Instance() != null)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (Graph != null)
                {
                    Graph.DataSource.AddPointToCategoryRealtime("ToolTarget", System.DateTime.Now, obj.Tools[0].Target, 1f);
                    Graph.DataSource.AddPointToCategoryRealtime("BedTarget", System.DateTime.Now, obj.Bed.Target, 1f);
                    Graph.DataSource.AddPointToCategoryRealtime("Tool", System.DateTime.Now, obj.Tools[0].Actual, 1f);
                    Graph.DataSource.AddPointToCategoryRealtime("Bed", System.DateTime.Now, obj.Bed.Actual, 1f);
                }

            });
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
