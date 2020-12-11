using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class STLPreviewController : MonoBehaviour
{

    [SerializeField]
    private OctoprinController octoprinController;

    [SerializeField]
    private STLImporter importer;

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
}
