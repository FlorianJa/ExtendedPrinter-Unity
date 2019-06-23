//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class UpdateVirtualPrinter : MonoBehaviour {

//    public OctoPrintConnector Connection;
//    public Transform XAxis;
//    public Transform YAxis;
//    public Transform ZAxis;


//    // Use this for initialization
//    void Start () {
//		if(Connection != null)
//        {
//            Connection.PositionChanged += Connection_PositionChanged;
//        }
//	}

//    private void Connection_PositionChanged(object sender, OctoprintClient.HomedEventArgs e)
//    {
//        XAxis.localPosition = new Vector3(0, 0, 0);
//        YAxis.localPosition = new Vector3(0, 0, 0);
//        ZAxis.localPosition = new Vector3(0, 0, 0);
//    }

//    // Update is called once per frame
//    void Update () {
		
//	}
//}
