using OctoprintClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OctoListenerLegend : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        OctoPrintConnector.Instance.TempHandler += Printers_TempHandler;
    }
    private void Printers_TempHandler(OctoprintHistoricTemperatureState obj)
    {

        if (UnityMainThreadDispatcher.Instance() != null)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                transform.GetChild(0).GetComponentInChildren<Text>().text = "ToolTarget: " + obj.Tools[0].Target.ToString("F0") + "°C";
                transform.GetChild(1).GetComponentInChildren<Text>().text = "BedTarget: " + obj.Bed.Target.ToString("F0") + "°C";
                transform.GetChild(2).GetComponentInChildren<Text>().text = "Tool: " + obj.Tools[0].Actual.ToString("F0") + "°C";
                transform.GetChild(3).GetComponentInChildren<Text>().text = "Bed: " + obj.Bed.Actual.ToString("F0") + "°C";
                

            });
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
