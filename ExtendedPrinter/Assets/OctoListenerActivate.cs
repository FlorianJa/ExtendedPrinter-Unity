using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctoListenerActivate : MonoBehaviour
{
    public bool onPrintFinished = false;
    public bool onFilamentChangeEnd = false;
    // Start is called before the first frame update
    void Start()
    {
        if (onPrintFinished)
        {
            OctoPrintConnector.Instance.PrintFinished += activate;
        }
        if (onFilamentChangeEnd)
        {
            OctoPrintConnector.Instance.FilamentChangeEnd += activate;
        }
    }
    private void activate()
    {
        gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
