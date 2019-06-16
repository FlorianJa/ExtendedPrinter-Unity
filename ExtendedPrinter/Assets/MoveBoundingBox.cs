using Microsoft.MixedReality.Toolkit.UI;
using OctoprintClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoundingBox : MonoBehaviour
{
    //after home: X:-19.00 Y:258.00 Z:10.00
    public float X = -0.019f;
    public float Y = 0.258f;
    public float Z = 0.010f;

    //absoult min:    X:-20.00 Y:-18.5 Z:0.00
    private float MinX = -0.02f;
    private float MinY = -0.0185f;
    private float MinZ = 0.0f;

    //max: X:293.90 Y:303.00 Z:256.01
    private float MaxX = 0.2939f;
    private float MaxY = 0.303f;
    private float MaxZ = 0.256f;

    public Interactable ArrowTop;
    public Interactable ArrowRight;
    public Interactable ArrowBottom;
    public Interactable ArrowLeft;
    public Interactable ArrowFront;
    public Interactable ArrowBack;

    public Transform PrinterHead;
    public Transform BuildPlate;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="distance">in Meter</param>
    public void MoveXAxis(float distance)
    {
        var tmp = PrinterHead.localPosition;
        if (X + distance > MinX && X + distance < MaxX)
        {
            X += distance;
            PrinterHead.localPosition = new Vector3(tmp.x + distance, tmp.y, tmp.z);
        }
    }

    public void MoveZAxis(float distance)
    {
        var tmp = PrinterHead.localPosition;
        if (Z + distance > MinZ && Z + distance < MaxZ)
        {
            Z += distance;
            PrinterHead.localPosition = new Vector3(tmp.x, tmp.y + distance, tmp.z);
        }
    }

    public void MoveYAxis(float distance)
    {
        var tmp = BuildPlate.localPosition;
        if (Y + distance > MinY && Z + distance < MaxY)
        {
            Y += distance;
            BuildPlate.localPosition = new Vector3(tmp.x, tmp.y, tmp.z - distance);
        }
    }

    public void Home()
    {
        PrinterHead.localPosition = Vector3.zero;
        BuildPlate.localPosition = Vector3.zero;

        X = -0.019f;
        Y= 0.258f;
        Z= 0.010f;
    }

    public void Update()
    {
        if(X + 0.01f > MaxX)
        {
            ArrowRight.Enabled = false;
        }
        else
        {
            ArrowRight.Enabled = true;
        }

        if(X - 0.01f < MinX)
        {
            ArrowLeft.Enabled = false;
        }
        else
        {
            ArrowLeft.Enabled = true;
        }

        if(Z + 0.01 > MaxZ)
        {
            ArrowTop.Enabled = false;
        }
        else
        {
            ArrowTop.Enabled = true;
        }

        if(Z - 0.01 < MinZ)
        {
            ArrowBottom.Enabled = false;
        }
        else
        {
            ArrowBottom.Enabled = true;
        }

        if (Y - 0.01 < MinY)
        {
            ArrowBack.Enabled = false;
        }
        else
        {
            ArrowBack.Enabled = true;
        }
        if (Y + 0.01 > MaxY)
        {
            ArrowFront.Enabled = false;
        }
        else
        {
            ArrowFront.Enabled = true;
        }

    }
}
