using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrintingUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject HeatUP1;
    
    [SerializeField]
    private GameObject Home;
    
    [SerializeField]
    private GameObject Calibration;
    
    [SerializeField]
    private GameObject HeatUP2;

    [SerializeField]
    private GameObject Start;


    public void OnHome()
    {
        HeatUP1.SetActive(false);
        Home.SetActive(true);
    }

    public void OnCalibration()
    {
        Home.SetActive(false);
        Calibration.SetActive(true);
    }

    public void OnHeatUp2()
    {
        Calibration.SetActive(false);
        HeatUP2.SetActive(true);
    }

    public void OnStart()
    {
        HeatUP2.SetActive(false);
        Start.SetActive(true);
    }

}
