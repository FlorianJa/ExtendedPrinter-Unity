using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
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

    [SerializeField]
    private ClippingPlane clippingPlane;

    private float maxHeight = 0;
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

    public void AddRenderersToClippingPlane(bool clearRenderersBefore)
    {
        if(clearRenderersBefore)
        {
            clippingPlane.ClearRenderers();
        }

        var renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            clippingPlane.AddRenderer(renderer);
        }

        SetClippingPlaneToMaxYPosition();
    }

    private void SetClippingPlaneToMaxYPosition()
    {
        //TODO: check performance!
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "Perimeter")
            {
                var meshFilters = child.gameObject.GetComponentsInChildren<MeshFilter>();

                foreach (var meshFilter in meshFilters)
                {
                    foreach(var vertex in meshFilter.mesh.vertices)
                    {
                        if (maxHeight < vertex.y)
                        {
                            maxHeight = vertex.y;
                        }
                    }
                }
            }
        }
        maxHeight = maxHeight / 1000f;

        var newPosition = clippingPlane.transform.localPosition;
        newPosition.y = maxHeight;
        clippingPlane.transform.localPosition = newPosition;
        
    }

    public void OnSliderValueChanged(SliderEventData sliderEventData)
    {
        var newPosition = clippingPlane.transform.localPosition;
        newPosition.y = maxHeight * sliderEventData.NewValue;

        clippingPlane.transform.localPosition = newPosition;
    }
}
