using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctoListenerActivate : MonoBehaviour
{
    public bool onPrintFinished = false;
    public bool onFilamentChangeEnd = false;
    public bool onFilamentChangeEndDeactivate = false;
    public bool onFilamentChangeBegin = false;
    // Start is called before the first frame update
    void Start()
    {
        if (onPrintFinished)
        {
            OctoPrintConnector.Instance.PrintFinished += ActivateSelf;
        }
        if (onFilamentChangeBegin)
        {
            OctoPrintConnector.Instance.FilamentChangeEnd += ActivateSelf;
        }
        if (onFilamentChangeEnd)
        {
            OctoPrintConnector.Instance.FilamentChangeEnd += ActivateSelf;
        }
        if (onFilamentChangeEndDeactivate)
        {
            OctoPrintConnector.Instance.FilamentChangeEnd += DeActivateSelf;
        }
    }
    private void ActivateSelf(object source, System.EventArgs args)
    {

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            gameObject.SetActive(true);
        });
    }
    private void DeActivateSelf(object source, System.EventArgs args)
    {

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            gameObject.SetActive(false);
        });
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
