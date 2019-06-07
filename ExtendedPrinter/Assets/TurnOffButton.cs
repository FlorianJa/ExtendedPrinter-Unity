using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffButton : MonoBehaviour
{
    public void TurnOFff()
    {
        GetComponent<Interactable>().Enabled = false;
        GetComponent<Interactable>().enabled = false;
        gameObject.SetActive(false);
    }
}
