using Microsoft.MixedReality.Toolkit.UI;
using OctoprintClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoundingBox : MonoBehaviour
{
    
    public float X = 0;
    public float Y = 0;
    public float Z = 0;

    //after home: X:-19.00 Y:258.00 Z:10.00
    public float HomeX = 0;
    public float HomeY = 0;
    public float HomeZ = 0;

    //absoult min:    X:-20.00 Y:-18.5 Z:0.00
    private float MinX = 0f;
    private float MinY = 0;
    private float MinZ = 0.05f;

    //max: X:293.90 Y:303.00 Z:256.01
    private float MaxX = 0.21f;
    private float MaxY = 0.21f;
    private float MaxZ = 0.21f;

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
    private Vector3 printheadHomePosition = new Vector3(0f, 0f, 0.295f);
    private Vector3 buildePlateHomePosition = new Vector3(0, 0, 0f);
    public ToolTip toolTip;

    public void Start()
    {
        OctoPrintConnector.MoveCompleted += OctoPrintConnector_MoveCompleted;
/*      PrinterHead.GetComponent<ManipulationHandler>().OnManipulationStarted.AddListener((med) => DisableArrows());
        PrinterHead.GetComponent<ManipulationHandler>().OnManipulationEnded.AddListener((med) => MovePrinterHeadToPosition(med));

        BuildPlate.GetComponent<ManipulationHandler>().OnManipulationStarted.AddListener((med) => DisableArrows());
        BuildPlate.GetComponent<ManipulationHandler>().OnManipulationEnded.AddListener((med) => MoveBuildplateToPosition(med));*/

    }

    public void DisableArrows()
    {
        manuallyDisabled = true;
    }

    public void MoveBuildplateToPosition(ManipulationEventData med)
    {
        var tmp = new Vector3(med.ManipulationSource.transform.localPosition.x * 1000f,
                                (med.ManipulationSource.transform.localPosition.z) * 1000f,
                                (med.ManipulationSource.transform.localPosition.y) * 1000f);

        //Y = tmp.y / 1000f;
        Z = tmp.z / 1000f;
        OctoPrintConnector.MovePrinter(tmp, true, false, false, true);
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

    public void MovePrinterHeadToPosition(ManipulationEventData med)
    {
        var tmp = new Vector3(med.ManipulationSource.transform.localPosition.x * 1000f,
                                med.ManipulationSource.transform.localPosition.z * 1000f, 
                                med.ManipulationSource.transform.localPosition.y * 1000f);

        X = tmp.x / 1000f;
        Y = tmp.y / 1000f;
        //Z = tmp.z / 1000f;
        OctoPrintConnector.MovePrinter(tmp, true,true,true,false);
        ShowPosition();
    }

    private void OctoPrintConnector_MoveCompleted(object sender, System.EventArgs e)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(EnableArrows);
    }

    public void MoveUp()
    {
        if (!manuallyDisabled)
        {
            manuallyDisabled = true;
            ArrowTop.Enabled = false;
            MoveBpundingBoxZAxis(distance);
            OctoPrintConnector.MovePrintHeadUp();
        }
    }
    public void MoveDown()
    {
        if (!manuallyDisabled)
        {
            manuallyDisabled = true;
            ArrowBottom.Enabled = false;
            MoveBpundingBoxZAxis(-distance);
            OctoPrintConnector.MovePrintHeadDown();
        }
    }
    public void MoveRight()
    {
        if (!manuallyDisabled)
        {
            manuallyDisabled = true;
            ArrowRight.Enabled = false;
            MoveBoundingBoxXAxis(distance);
            OctoPrintConnector.MovePrintHeadRight();
        }
    }
    public void MoveLeft()
    {
        if (!manuallyDisabled)
        {
            manuallyDisabled = true;
            ArrowLeft.Enabled = false;
            MoveBoundingBoxXAxis(-distance);
            OctoPrintConnector.MovePrintHeadLeft();
        }
    }
    public void MoveFront()
    {
        if (!manuallyDisabled)
        {
            manuallyDisabled = true;
            ArrowFront.Enabled = false;
            MoveBoundingBoxYAxis(distance);
            OctoPrintConnector.MoveBuildplateFront();
        }
    }
    public void MoveBack()
    {
        if (!manuallyDisabled)
        {
            manuallyDisabled = true;
            ArrowBack.Enabled = false;
            MoveBoundingBoxYAxis(-distance);
            OctoPrintConnector.MoveBuildplateBack();
        }
    }

    public void EnableArrows()
    {
        manuallyDisabled = false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="distance">in Meter</param>
    public void MoveBoundingBoxXAxis(float distance)
    {
        var tmp = PrinterHead.transform.localPosition;
        if (X + distance > MinX && X + distance < MaxX)
        {
            X += distance;
            PrinterHead.transform.localPosition = new Vector3(tmp.x + distance, tmp.y, tmp.z);
        }
        ShowPosition();
    }

    public void MoveBpundingBoxZAxis(float distance)
    {
        var tmp = PrinterHead.transform.localPosition;
        if (Z + distance > MinZ && Z + distance < MaxZ)
        {
            Z += distance;
            PrinterHead.transform.localPosition = new Vector3(tmp.x, tmp.y + distance, tmp.z);
        }
        ShowPosition();
    }

    public void MoveBoundingBoxYAxis(float distance)
    {
        var tmp = BuildPlate.transform.localPosition;
        if (Y + distance > MinY && Z + distance < MaxY)
        {
            Y += distance;
            BuildPlate.transform.localPosition = new Vector3(tmp.x, tmp.y, tmp.z + distance);
        }
        ShowPosition();
    }

    public void BoundingBoxHome()
    {
        PrinterHead.transform.localPosition = printheadHomePosition;
        BuildPlate.transform.localPosition = buildePlateHomePosition;

        X = 0f;
        Y= 0.295f;
        Z= 0f;
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
        else
        {
            ArrowTop.Enabled = false;
            ArrowRight.Enabled = false;
            ArrowBottom.Enabled = false;
            ArrowLeft.Enabled = false;
            ArrowFront.Enabled = false;
            ArrowBack.Enabled = false;
        }
    }
}
