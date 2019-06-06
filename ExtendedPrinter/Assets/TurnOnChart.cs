using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnChart : MonoBehaviour
{
    public GameObject Chart;

    public void TurnOn()
    {
        Chart.SetActive(true);
    }
}
