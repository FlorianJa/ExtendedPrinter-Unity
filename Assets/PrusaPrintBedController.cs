using OctoPrintLib;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PrusaPrintBedController : MonoBehaviour
{
    public Transform BuildPlate;

    public void CurrentDataRecieved(CurrentMessage data)
    {
        //int i = data.current.logs.Count-1;
        //while (i >= 0)
        //{
        //    //Send: N3848 G1 X69.186 Y86.257 E0.01178 * 84
        //    if(data.current.logs[i].StartsWith("Send: "))
        //    {
        //        var yIndex = data.current.logs[i].IndexOf('Y');
        //        if (yIndex >= 0)
        //        {
        //            var index = data.current.logs[i].IndexOf(' ', yIndex);
        //            float yValue;
        //            if (index >=0)
        //            {
        //               yValue = float.Parse(data.current.logs[i].Substring(yIndex + 1, index - yIndex), CultureInfo.InvariantCulture.NumberFormat);
        //            }
        //            else
        //            {
        //                yValue = float.Parse(data.current.logs[i].Substring(yIndex + 1), CultureInfo.InvariantCulture.NumberFormat);
        //            }
                      

        //            float offset = 0.18f;

        //            BuildPlate.localPosition = new Vector3(BuildPlate.localPosition.x, BuildPlate.localPosition.y, offset - (yValue / 1000f));
        //            break;
        //        }
        //    }
        //    i--;
        //}
                
    }
}
