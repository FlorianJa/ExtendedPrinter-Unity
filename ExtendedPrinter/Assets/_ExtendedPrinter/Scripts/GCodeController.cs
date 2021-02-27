using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCodeController : MonoBehaviour
{
    [SerializeField]
    private Material Perimetermaterial;

    [SerializeField]
    private Material InfillMaterial;

    [SerializeField]
    private Material SupportMaterial;

    public void RemoveChildren()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }


    public void SetMeshMaterial()
    {
        foreach (Transform child in transform)
        {
            if(child.gameObject.name == "Perimeter")
            {
                foreach (Transform childChild in child)
                {
                    childChild.GetComponentInChildren<Renderer>().material = Perimetermaterial;
                }
            } 
            else if (child.gameObject.name == "Infill")
            {
                foreach (Transform childChild in child)
                {
                    childChild.GetComponentInChildren<Renderer>().material = InfillMaterial;
                }
            }
            else if (child.gameObject.name == "Support")
            {
                foreach (Transform childChild in child)
                {
                    childChild.GetComponentInChildren<Renderer>().material = SupportMaterial;
                }
            }
        }
    }
}
