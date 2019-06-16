using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableVuforiaEngine : MonoBehaviour
{
    public GameObject MainCamera;

    public void ToogleVuforia()
    {
        MainCamera.GetComponent<VuforiaMonoBehaviour>().enabled = !MainCamera.GetComponent<VuforiaMonoBehaviour>().enabled;
    }
}
