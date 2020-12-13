using Dummiesman;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Assets._ExtendedPrinter.Scripts.Helper;

namespace Assets._ExtendedPrinter.Scripts.ModelLoader
{
    public class SlicedModelLoader : MonoBehaviour
    {

        public StringEvent FileDownloadCompleted;
        public IntEvent DownloadProgres;

        [SerializeField]
        [Tooltip("The loaded models will be attached (as child) to this transform")]
        private Transform parent;

        [SerializeField]
        [Tooltip("Material of the loaded models")]
        private Material material;

        [SerializeField]
        [Tooltip("Should the loaded model be filtered by component names?")]
        private bool useFilter = true;

        [SerializeField]
        [Tooltip("List the names of componets which should be displayed. useFilter need to be true.")]
        public List<string> FilterList;

        /// <summary>
        /// Downloads the model as zip from the slicing servive server.
        /// </summary>
        /// <param name="Uri">URL of the model to download.</param>
        public async void LoadObjTest(Uri Uri)
        {
            ClearParent();

            var fileName = Uri.Segments[Uri.Segments.Length - 1];
            await DownloadFileAsync(Uri, Path.Combine(Application.persistentDataPath, fileName + ".zip"));
            var files = await Unzip(Path.Combine(Application.persistentDataPath, fileName + ".zip"));

            foreach (var file in files)
            {
                if (useFilter)
                {
                    foreach (var filter in FilterList)
                    {
                        if (file.Contains(filter))
                        {
                            await LoadObjFromFileAsync(file, parent);
                            break;
                        }
                    }
                }
                else
                {
                    await LoadObjFromFileAsync(file, parent);
                }
            }

            var renderers = parent.GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                renderer.material = material;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
        }

        private void ClearParent()
        {
            while (parent.childCount > 0)
            {
                DestroyImmediate(parent.GetChild(0).gameObject);
            }
        }

        private async Task DownloadFileAsync(Uri fileUri, string localFullPath)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(fileUri))
            {
                await request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    using (var fs = new FileStream(localFullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, request.downloadHandler.data.Length, true))
                    {
                        await fs.WriteAsync(request.downloadHandler.data, 0, request.downloadHandler.data.Length);
                    }
                }
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgres.Invoke(e.ProgressPercentage);
            Debug.Log(e.ProgressPercentage);
        }

        private AsyncCompletedEventHandler DownloadFileCompleted(string filename)
        {
            Action<object, AsyncCompletedEventArgs> action = (sender, e) =>
            {
                var _filename = filename;

                if (e.Error != null)
                {
                    throw e.Error;
                }
                FileDownloadCompleted.Invoke(_filename);
            };
            return new AsyncCompletedEventHandler(action);
        }



        public async Task<List<string>> Unzip(string zipPath, string outputPath = "")
        {
            if (outputPath == string.Empty)
            {
                outputPath = Path.Combine(Application.persistentDataPath, Path.GetFileNameWithoutExtension(zipPath));
            }

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var paths = new List<string>();
            await Task.Run(() =>
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        var fileName = entry.Name;
                        try
                        {
                            var path = Path.Combine(outputPath, fileName);
                            paths.Add(path);
                            entry.ExtractToFile(path, true);
                        }
                        catch (Exception ex)
                        {
                            Debug.Log(ex.ToString());
                        }
                    }
                }
            });

            return paths;
        }

        public async Task LoadObjFromFileAsync(string objFullPath, Transform parent, bool WorldPositionStays = false)
        {
            var loader = new OBJLoader();
            loader.defaultMaterial = material;
            var tmp = await loader.LoadAsync(objFullPath);
            tmp.transform.SetParent(parent, WorldPositionStays);
            tmp.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        }
    }
}