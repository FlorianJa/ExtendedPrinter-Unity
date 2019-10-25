using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class DisableVuforiaEngine : MonoBehaviour
{
    public GameObject MainCamera;

    public void ToogleVuforia()
    {
        MainCamera.GetComponent<VuforiaMonoBehaviour>().enabled = !MainCamera.GetComponent<VuforiaMonoBehaviour>().enabled;
    }
}
