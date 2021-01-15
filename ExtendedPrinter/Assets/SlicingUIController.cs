using Assets._ExtendedPrinter.Scripts.SlicingService;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicingUIController : MonoBehaviour
{
    //[SerializeField]
    //private ButtonConfigHelper sliceButton;

    [SerializeField]
    private GameObject STLPreview;

    [SerializeField]
    private SlicingServiceConnection slicingServiceConnection;
    
    private string _modelFileName;



    public void SetupUI(string modelName)
    {
        this._modelFileName = modelName;
    }

    public void Slice()
    {
        if(slicingServiceConnection != null)
        {
            if(STLPreview == null)
            {
                return;
            }

            var stlContainer = STLPreview.transform.GetChild(0);
            var center = new Vector2(stlContainer.localPosition.x * 1000, stlContainer.localPosition.z * 1000);
            var scale = stlContainer.localScale.x;
            var rotation = new Vector3(-stlContainer.localRotation.eulerAngles.x, -stlContainer.localRotation.eulerAngles.z, - stlContainer.localRotation.eulerAngles.y);
            slicingServiceConnection.SliceModel(_modelFileName,center,scale, rotation);
        }
    }
}
