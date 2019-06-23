using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Documentationframe : MonoBehaviour
{
    public Documentationframe nextframe = null;
    public Vector3 movecommand = new Vector3(-1, -1, -1);
    public int tempgoal = -1;
    public OctoPrintConnector oc =null;
    public float timeout = -1.0f;

    private int speed = 200;
    private bool running = false;
    private float timeoutinternal = -1.0f;

    //public Animation[] clips;

    public void popup()
    {
        timeoutinternal = timeout;
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = true;
        }
        foreach (BoxCollider bc in GetComponentsInChildren<BoxCollider>())
        {
            bc.enabled = true;
        }
        if (oc != null){

            if (movecommand.x != -1)
            {
                oc.MoveAxis(OctoprintClient.Axis.X, movecommand.x, true, speed);
            }
            if (movecommand.y != -1)
            {
                oc.MoveAxis(OctoprintClient.Axis.Y, movecommand.y, true, speed);
            }
            if (movecommand.z != -1)
            {
                oc.MoveAxis(OctoprintClient.Axis.Z, movecommand.z, true, speed);
            }
            if (tempgoal != -1)
            {
                oc.SetExtruderTemp(tempgoal);
            }
        }
    }

    public void next()
    {
        if (oc != null||tempgoal==-1)
        {
            if (tempgoal == -1 || tempgoal > oc.GetExtruderTemp())
            {

                timeoutinternal = -1.0f;
                //print("framechange to " + nextframe.name);
                foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
                {
                    mr.enabled = false;
                }
                foreach (BoxCollider bc in GetComponentsInChildren<BoxCollider>())
                {
                    bc.enabled = false;
                }
                if (nextframe != null)
                {
                    nextframe.popup();
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (BoxCollider bc in GetComponentsInChildren<BoxCollider>())
        {
            bc.enabled = false;
        }
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = false;
        }
        if (name == "Entdecken 1")
            popup();

    }

    // Update is called once per frame
    void Update()
    {
        if (timeoutinternal > 0)
        {
            timeoutinternal -= Time.deltaTime;
            if(timeoutinternal <= 0)
            {
                next();
            }
        }
    }
}
