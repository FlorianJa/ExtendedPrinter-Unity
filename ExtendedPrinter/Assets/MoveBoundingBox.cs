﻿using Microsoft.MixedReality.Toolkit.UI;
using OctoprintClient;
using System;
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

    public GameObject PrinterHead;
    public GameObject BuildPlate;

    public OctoPrintConnector OctoPrintConnector;
    public float distance = 0.01f;
    private bool manuallyDisabled;
    private Vector3 printheadHomePosition = new Vector3(-0.019f, 0.01f, 0f);
    private Vector3 buildePlateHomePosition = new Vector3(0, 0, 0.258f);
    public ToolTip toolTip;

    public void Start()
    {
        OctoPrintConnector.MoveCompleted += OctoPrintConnector_MoveCompleted;
        PrinterHead.GetComponent<ManipulationHandler>().OnManipulationStarted.AddListener((med) => DisableArrows());
        PrinterHead.GetComponent<ManipulationHandler>().OnManipulationEnded.AddListener((med) => MovePrinterHeadToPosition(med));

        BuildPlate.GetComponent<ManipulationHandler>().OnManipulationStarted.AddListener((med) => DisableArrows());
        BuildPlate.GetComponent<ManipulationHandler>().OnManipulationEnded.AddListener((med) => MoveBuildplateToPosition(med));

    }

    private void MoveBuildplateToPosition(ManipulationEventData med)
    {
        var tmp = new Vector3(med.ManipulationSource.transform.localPosition.x * 1000f,
                                (med.ManipulationSource.transform.localPosition.z) * 1000f,
                                (med.ManipulationSource.transform.localPosition.y) * 1000f);

        Y = tmp.y / 1000f;
        OctoPrintConnector.MovePrinter(tmp, true, false, true, false);
        ShowPosition();
    }

    public void ShowPosition()
    {
        //toolTip.SetToolTipText("Aktuelle Position:\n" +
        //                                "X: " + (X * 1000).ToString() + "\n" +
        //                                "Y: " + (Y * 1000).ToString() + "\n" +
        //                                "Z: " + (Z * 1000).ToString() + "\n"
        //                                );
    }

    private void MovePrinterHeadToPosition(ManipulationEventData med)
    {
        var tmp = new Vector3(med.ManipulationSource.transform.localPosition.x * 1000f,
                                med.ManipulationSource.transform.localPosition.z * 1000f, 
                                med.ManipulationSource.transform.localPosition.y * 1000f);

        X = tmp.x / 1000f;
        Z = tmp.z / 1000f;
        OctoPrintConnector.MovePrinter(tmp, true,true,false,true);
        ShowPosition();
    }

    private void OctoPrintConnector_MoveCompleted(object sender, System.EventArgs e)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(EnableArrows);
    }

    public void MoveUp()
    {
        MoveZAxis(distance);
        OctoPrintConnector.MovePrintHeadUp();
        DisableArrows();
    }
    public void MoveDown()
    {
        MoveZAxis(-distance);
        OctoPrintConnector.MovePrintHeadDown();
        DisableArrows();
    }
    public void MoveRight()
    {
        MoveXAxis(distance);
        OctoPrintConnector.MovePrintHeadRight();
        DisableArrows();
    }
    public void MoveLeft()
    {
        MoveXAxis(-distance);
        OctoPrintConnector.MovePrintHeadLeft();
        DisableArrows();
    }
    public void MoveFront()
    {
        MoveYAxis(distance);
        OctoPrintConnector.MoveBuildplateFront();
        DisableArrows();
    }
    public void MoveBack()
    {
        MoveYAxis(-distance);
        OctoPrintConnector.MoveBuildplateBack();
        DisableArrows();
    }

    public void DisableArrows()
    {
        manuallyDisabled = true;
        ArrowTop.Enabled = false;
        ArrowRight.Enabled = false;
        ArrowBottom.Enabled = false;
        ArrowLeft.Enabled = false;
        ArrowFront.Enabled = false;
        ArrowBack.Enabled = false;
    }
    public void EnableArrows()
    {
        manuallyDisabled = false;
        ArrowTop.Enabled = true;
        ArrowRight.Enabled = true;
        ArrowBottom.Enabled = true;
        ArrowLeft.Enabled = true;
        ArrowFront.Enabled = true;
        ArrowBack.Enabled = true;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="distance">in Meter</param>
    public void MoveXAxis(float distance)
    {
        var tmp = PrinterHead.transform.localPosition;
        if (X + distance > MinX && X + distance < MaxX)
        {
            X += distance;
            PrinterHead.transform.localPosition = new Vector3(tmp.x + distance, tmp.y, tmp.z);
        }
        ShowPosition();
    }

    public void MoveZAxis(float distance)
    {
        var tmp = PrinterHead.transform.localPosition;
        if (Z + distance > MinZ && Z + distance < MaxZ)
        {
            Z += distance;
            PrinterHead.transform.localPosition = new Vector3(tmp.x, tmp.y + distance, tmp.z);
        }
        ShowPosition();
    }

    public void MoveYAxis(float distance)
    {
        var tmp = BuildPlate.transform.localPosition;
        if (Y + distance > MinY && Z + distance < MaxY)
        {
            Y += distance;
            BuildPlate.transform.localPosition = new Vector3(tmp.x, tmp.y, tmp.z + distance);
        }
        ShowPosition();
    }

    public void Home()
    {
        PrinterHead.transform.localPosition = printheadHomePosition;
        BuildPlate.transform.localPosition = buildePlateHomePosition;

        X = -0.019f;
        Y= 0.258f;
        Z= 0.010f;
        ShowPosition();
    }

    public void Update()
    {
        if (!manuallyDisabled)
        {
            if (X + 0.01f > MaxX)
            {
                ArrowRight.Enabled = false;
            }
            else
            {
                ArrowRight.Enabled = true;
            }

            if (X - 0.01f < MinX)
            {
                ArrowLeft.Enabled = false;
            }
            else
            {
                ArrowLeft.Enabled = true;
            }

            if (Z + 0.01 > MaxZ)
            {
                ArrowTop.Enabled = false;
            }
            else
            {
                ArrowTop.Enabled = true;
            }

            if (Z - 0.01 < MinZ)
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
}