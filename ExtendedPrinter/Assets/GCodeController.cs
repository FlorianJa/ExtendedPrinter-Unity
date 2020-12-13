using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCodeController : MonoBehaviour
{
    public void RemoveChildren()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
