using ChartAndGraph;
using OctoprintClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctoListenerGraph : MonoBehaviour
{
    // Start is called before the first frame update
    private GraphChart Graph;
    void Start()
    {
        OctoPrintConnector.Instance.NewTemperatureDataRecieved += OctoPrintConnector_OnNewTemperatureDataRecieved;
        Graph =GetComponent<GraphChart>();
    }
    private void OctoPrintConnector_OnNewTemperatureDataRecieved(object source,OctoprintHistoricTemperatureState obj)
    {

        if (UnityMainThreadDispatcher.Instance() != null)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (Graph != null)
                {
                    Graph.DataSource.AddPointToCategoryRealtime("Nozzle actual", System.DateTime.Now, obj.Tools[0].Actual, 1f);
                    Graph.DataSource.AddPointToCategoryRealtime("Buildplate actual", System.DateTime.Now, obj.Bed.Actual, 1f);
                    Graph.DataSource.AddPointToCategoryRealtime("Nozzle target", System.DateTime.Now, obj.Tools[0].Target, 1f);
                    Graph.DataSource.AddPointToCategoryRealtime("Buildplate target", System.DateTime.Now, obj.Bed.Target, 1f);
                }

            });
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
