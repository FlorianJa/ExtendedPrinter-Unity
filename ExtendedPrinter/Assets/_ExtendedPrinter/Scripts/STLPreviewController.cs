using Assets._ExtendedPrinter.Scripts.Helper;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class STLPreviewController : MonoBehaviour
{

    [SerializeField]
    private OctoprintController octoprinController;

    [SerializeField]
    private STLImporter importer;

    [SerializeField]
    private BoundsControllVisuals boundsControlVisuals;

    public async void LoadSTLAsync(string fileName)
    {
        var _fileName = Path.Combine(Application.persistentDataPath, "Downloads", fileName);

        if (!File.Exists(_fileName))
        {
            var result = await octoprinController.DownloadFileAsync(fileName);

            if (!result)
            {
                return;
            }
        }

        if (importer != null)
        {
            await importer.ImportAsync(_fileName, this.transform);
        }
    }

    public void RemoveSTLPreview()
    {
        Destroy(transform.GetChild(0).gameObject);
    }
}
