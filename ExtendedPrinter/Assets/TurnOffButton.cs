using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffButton : MonoBehaviour
{
    public GameObject[] buttons;

    public void TurnOFff()
    {
        if (buttons.Length > 0)
        {
            foreach (var button in buttons)
            {
                if (button != null)
                {
                    GetComponent<Interactable>().Enabled = false;
                    GetComponent<Interactable>().enabled = false;
                    gameObject.SetActive(false);
                }
                else
                {
                    button.GetComponent<Interactable>().Enabled = false;
                    button.GetComponent<Interactable>().enabled = false;
                    button.SetActive(false);
                }
            }
        }
    }
}
