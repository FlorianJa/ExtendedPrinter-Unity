using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGlobal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Interactable>().IsGlobal = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
