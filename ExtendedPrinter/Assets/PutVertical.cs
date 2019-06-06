using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutVertical : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,0);
    }
}
    