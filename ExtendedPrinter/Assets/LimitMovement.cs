using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitMovement : MonoBehaviour
{

    //absoult min:    X:-20.00 Y:-18.5 Z:0.00 <- printer coordinatsystem
    public float MinX = -0.02f;
    public float MinY = -0.0185f;
    public float MinZ = 0.0f;

    //max: X:293.90 Y:303.00 Z:256.01
    public float MaxX = 0.2939f;
    public float MaxY = 0.303f;
    public float MaxZ = 0.256f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tmp = transform.localPosition;
        if(transform.localPosition.x >MaxX)
        {
            tmp.x = MaxX;
        }
        if (transform.localPosition.x < MinX)
        {
            tmp.x = MinX;
        }
        if (transform.localPosition.y > MaxY)
        {
            tmp.y = MaxY;
        }
        if (transform.localPosition.y < MinY)
        {
            tmp.y = MinY;
        }
        if (transform.localPosition.z > MaxZ)
        {
            tmp.z = MaxZ;
        }
        if (transform.localPosition.z < MinZ)
        {
            tmp.z = MinZ;
        }

        transform.localPosition = tmp;
    }
}
