using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class MeshLoaderNet
{

    private const string FolderToRawGCodes = "/RawGCodes/";
    internal IEnumerator LoadObject(string urlToFile, GCodeHandler source, MeshLoader loader)
    {
        if (!source.loading)
        {
            source.loading = true;
            source.disableUI();


            int startindex = urlToFile.LastIndexOf("/") + 1;
            string savePath = loader.dataPath + FolderToRawGCodes + urlToFile.Substring(startindex);
            source.createBlankObject(savePath);

            if (!loader.CheckForExsitingObject(urlToFile))
            {
                if (!File.Exists(savePath))
                {
                    // download gcode file from octoprint server
                    UnityWebRequest www = UnityWebRequest.Get(urlToFile);
                    yield return www.SendWebRequest();
                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {
                        if (!Directory.Exists(loader.dataPath + "/RawGCodes/"))
                        {
                            Directory.CreateDirectory(loader.dataPath + "/RawGCodes/");
                        }
                        System.IO.File.WriteAllText(savePath, www.downloadHandler.text);

                    }


                }
                
                string[] Lines = File.ReadAllLines(savePath);
                Task.Run(() => loader.gcodeMeshGenerator.CreateObjectFromGCode(Lines, loader, source));

            }
            else
            {
                if (loader.loadingFromDisk == false)
                {
                    loader.path = urlToFile;
                    loader.loadingFromDisk = true;
                }
            }
        }
    }
}