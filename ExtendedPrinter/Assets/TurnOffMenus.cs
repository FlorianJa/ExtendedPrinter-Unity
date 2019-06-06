using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffMenus : MonoBehaviour
{
    public GameObject selectMenu;
    public GameObject showHideMenu;
    public GameObject slider;
    public GameObject Gcode;

    public void TurnOff()
    {
        selectMenu.SetActive(false);
        showHideMenu.SetActive(false);
        slider.SetActive(false);
        Gcode.SetActive(false);
    }
}
