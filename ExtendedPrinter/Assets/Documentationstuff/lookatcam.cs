using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookatcam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Camera.current!=null && Camera.current.transform!=null)
            transform.LookAt(Camera.current.transform,(new Vector3(0,1,0)));
    }
}
