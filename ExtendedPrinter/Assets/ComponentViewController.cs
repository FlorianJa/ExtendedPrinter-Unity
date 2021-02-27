using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentViewController : MonoBehaviour
{
    [SerializeField]
    private GameObject GcodeContainer;

    [SerializeField]
    private GameObject PerimeterButton;
    [SerializeField]
    private GameObject InfillButton;
    [SerializeField]
    private GameObject SupportButton;

    public void Setup()
    {
        PerimeterButton.SetActive(false);
        InfillButton.SetActive(false);
        SupportButton.SetActive(false);

        PerimeterButton.GetComponent<Interactable>().OnClick.RemoveAllListeners();
        InfillButton.GetComponent<Interactable>().OnClick.RemoveAllListeners();
        SupportButton.GetComponent<Interactable>().OnClick.RemoveAllListeners();


        foreach (Transform child in GcodeContainer.transform)
        {
            if(child.gameObject.name == "Perimeter" && child.childCount > 0)
            {
                PerimeterButton.SetActive(true);
                PerimeterButton.GetComponent<Interactable>().OnClick.AddListener(() => { child.gameObject.SetActive(!child.gameObject.activeSelf); });
            }
            else if(child.gameObject.name == "Infill" && child.childCount > 0)
            {
                InfillButton.SetActive(true);
                InfillButton.GetComponent<Interactable>().OnClick.AddListener(() => { child.gameObject.SetActive(!child.gameObject.activeSelf); });
            }
            else if(child.gameObject.name == "Support" && child.childCount > 0)
            {
                SupportButton.SetActive(true);
                SupportButton.GetComponent<Interactable>().OnClick.AddListener(() => { child.gameObject.SetActive(!child.gameObject.activeSelf); });
            }
        }
    }
}
