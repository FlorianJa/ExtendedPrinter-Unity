using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressController : MonoBehaviour
{
    [SerializeField]
    private ProgressIndicatorOrbsRotator progressIndicator;

    public async void ActivateProgressIndicator()
    {
        if(progressIndicator != null)
        {
            progressIndicator.gameObject.SetActive(true);
            await progressIndicator.OpenAsync();
        }
    }

    public async void CloseProgressIndicator()
    {
        if (progressIndicator != null)
        {
            await progressIndicator.CloseAsync();
            progressIndicator.gameObject.SetActive(false);
        }
    }
}
