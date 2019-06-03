//using OctoprintClient;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MovePhysicalPrinter : MonoBehaviour {


//    public OctoPrintConnector Connection;
//    public bool IsEnabled;
//    public Axis Axis;
//    public int speed = 200;

//    private float lastPosition;
//	// Update is called once per frame
//	void Update () {
//        if (IsEnabled)
//        {
//            if (Axis == Axis.X)
//            {
//                var newPosition = transform.localPosition.x;

//                if (Mathf.Abs(newPosition - lastPosition) > 0.0001)
//                {
//                    Connection.MoveAxis(Axis.X, -(newPosition * 1000f), true, speed);
//                    lastPosition = newPosition;
//                }
//            }

//            if (Axis == Axis.Y)
//            {
//                var newPosition = transform.localPosition.y;

//                if (Mathf.Abs(newPosition - lastPosition) > 0.0001)
//                {
//                    Connection.MoveAxis(Axis.Y, -(newPosition * 1000f), true, speed);
//                    lastPosition = newPosition;
//                }
//            }

//            if (Axis == Axis.Z)
//            {
//                var newPosition = transform.localPosition.z;

//                if(Mathf.Abs(newPosition - lastPosition) > 0.0001)
//                {
//                    Connection.MoveAxis(Axis.Z, newPosition*1000f, true, speed);
//                    lastPosition = newPosition;
//                }
//            }
//        }
//	}
//}
