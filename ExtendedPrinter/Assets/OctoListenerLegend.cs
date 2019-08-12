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
        OctoPrintConnector.Instance.NewTemperatureDataRecieved += OctoPrintConnector_OnNewTemperatureDataRecieved;
    }
    private void OctoPrintConnector_OnNewTemperatureDataRecieved(object s,OctoprintHistoricTemperatureState obj)
    {

        if (UnityMainThreadDispatcher.Instance() != null)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                transform.GetChild(0).GetComponentInChildren<Text>().text = "Düse IST: " + obj.Tools[0].Actual.ToString("F0") + "°C";
                transform.GetChild(1).GetComponentInChildren<Text>().text = "Düse SOLL: " + obj.Tools[0].Target.ToString("F0") + "°C";
                transform.GetChild(2).GetComponentInChildren<Text>().text = "Buildplate IST: " + obj.Bed.Actual.ToString("F0") + "°C";
                transform.GetChild(3).GetComponentInChildren<Text>().text = "Buildplate SOLL: " + obj.Bed.Target.ToString("F0") + "°C";
            });
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
