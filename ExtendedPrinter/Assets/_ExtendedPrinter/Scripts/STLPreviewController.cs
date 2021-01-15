using Assets._ExtendedPrinter.Scripts.Helper;
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
           var stlContainer= await importer.ImportAsync(_fileName, this.transform);
            TransformationUIController.Host = stlContainer.transform;
        }
    }

    public void RemoveSTLPreview()
    {
        Destroy(transform.GetChild(0).gameObject);
    }
}
