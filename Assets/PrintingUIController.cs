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

    [SerializeField]
    private AudioSource audio;

    [SerializeField]
    private AudioClip HomeAudioClip;
    [SerializeField]
    private AudioClip Heatup1AudioClip;
    [SerializeField]
    private AudioClip Heatup2AudioClip;
    [SerializeField]
    private AudioClip CalibrationAudioClip;
    [SerializeField]
    private AudioClip StartAudioClip;

    public void OnHome()
    {
        HeatUP1.SetActive(false);
        Home.SetActive(true);
        audio.PlayOneShot(HomeAudioClip);
    }

    public void OnCalibration()
    {
        Home.SetActive(false);
        Calibration.SetActive(true);
        audio.PlayOneShot(CalibrationAudioClip);
    }

    public void OnHeatUp2()
    {
        Calibration.SetActive(false);
        HeatUP2.SetActive(true);
        audio.PlayOneShot(Heatup2AudioClip);
    }

    public void OnStart()
    {
        HeatUP2.SetActive(false);
        Start.SetActive(true);
        audio.PlayOneShot(StartAudioClip);
    }

}
