using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class octoListener : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        OctoPrintConnector.Instance.MoveCompleted += DeactiveManipluationHandler;
    }

    private void DeactiveManipluationHandler(object sender, EventArgs e)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GetComponent<ManipulationHandler>().enabled = true;
        });
    }

    
}
