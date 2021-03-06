using Assets._ExtendedPrinter.Scripts.Helper;
using Assets._ExtendedPrinter.Scripts.SlicingService;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class STLPreviewController : MonoBehaviour
{

    [SerializeField]
    private OctoprintController octoprintController;

    [SerializeField]
    private STLImporter importer;

    [SerializeField]
    private BoundsControllVisuals boundsControlVisuals;

    [SerializeField]
    private TransformationUIController TransformationUIController;

    private GameObject stlContainer;
    private Vector3 defaultPosition;

    public async void LoadSTLAsync(string fileName)
    {
        var _fileName = Path.Combine(Application.persistentDataPath, "Downloads", fileName);

        if (!File.Exists(_fileName))
        {
            var result = await octoprintController.DownloadFileAsync(fileName);

            if (!result)
            {
                return;
            }
        }

        if (importer != null)
        {
            stlContainer= await importer.ImportAsync(_fileName, this.transform);
            defaultPosition = stlContainer.transform.localPosition;
            TransformationUIController.Host = stlContainer.transform;
        }
    }

    public void RemoveSTLPreview()
    {
        Destroy(transform.GetChild(0).gameObject);
    }

    public void DisableSTLPreview()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    public void EnableSTLPreview()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void ResetPosition()
    {
        stlContainer.transform.localPosition = defaultPosition;
    }
    public void ResetRotation()
    {
        stlContainer.transform.localRotation = Quaternion.identity;
    }
    public void ResetScale()
    {
        stlContainer.transform.localScale = Vector3.one;
    }
}
