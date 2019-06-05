using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using System;
using System.Threading;
using Microsoft.MixedReality.Toolkit.UI;
using UnityMeshSimplifier;
using B83.MeshTools;
using System.Threading.Tasks;
//using HoloToolkit.UX.Progress;
using UnityEngine.Networking;

//using UnityMeshSimplifier.Scripts.UnityMeshSimplifier;

public class MeshCreator : MonoBehaviour
{
    private const string FolderToRawGCodes = "/RawGCodes/";

    public string[] names;
    [Serializable]
    public struct MaterialPreference
    {
        public string name;
        public Material mat;
    }


    public MaterialPreference[] materialDictionary;
    public float rotationclustersize = 180.0f;
    public float distanceclustersize = 3.5f;
    public float meshsimplifyquality = 0.5f;
    public int layercluster = 1;
    private int createdLayers;
    private int simplifiedLayers;
    public int layersvisible = 0;
    public ButtonGeneratorShowHideMenu ShowHideCheckBoxes;
    public PinchSlider slider;
    public bool loading = false;
    private float plasticwidth = 0.6f;
    private int _layersvisible = 0;
    private Dictionary<string, Dictionary<int, GameObject>> allLayerObjects = new Dictionary<string, Dictionary<int, GameObject>>();
    private Dictionary<string, GameObject> parentObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, bool> parentvisible = new Dictionary<string, bool>();

    private GameObject RootForObject;

    private bool newloaded;
    private struct MeshcreatorInput
    {
        public string meshname;
        public Vector3[] newVertices;
        public Vector3[] newNormals;
        public Vector2[] newUV;
        public int[] newTriangles;
    }
    private Queue<MeshcreatorInput> meshCreatorInputQueue = new Queue<MeshcreatorInput>();
    private class MeshSimplifierstruct
    {
        public MeshSimplifier MeshSimplifier;
        public MeshFilter MeshFilter;
        public bool simplified;

    }
    private bool issimplifying = false;
    private bool simplifypossible = false;
    private Queue<MeshSimplifierstruct> meshSimplifierQueue = new Queue<MeshSimplifierstruct>();
    private Queue<KeyValuePair<string, Mesh>> loadQueue = new Queue<KeyValuePair<string, Mesh>>();
    private object meshSimplifierQueueLock = new object();
    private object MeshCreatorInputQueueLock = new object();
    private string dataPath;
    private bool _regenerateModel = false;
    private bool filesLoadingfinished = false;
    private string path;
    private bool loadingFromDisk;
    private int EnqueuedMeshes;
    private int dequeuedMeshes = 0;
    private int layernum;

    void Start()
    {
        if (slider)
        {
            slider.OnValueUpdated.AddListener(Updateslider);
        }

        dataPath = Application.persistentDataPath;
        string mainpath = Application.streamingAssetsPath;
        //names = Directory.GetFiles(mainpath, "*.gcode");
        //print(names.ToString());
        //Debug.Log(names.ToString());
        ////createThread = new Task(recreate);
        ////simplify = new Task(simplyfyOne);
        //if (names.Length > 0)
        //{
        //    if (CheckForExsitingObject(names[0]) && !_regenerateModel)
        //    {
        //        filename = names[0];
        //        //create a parent for the objects we create now
        //        RootForObject = new GameObject(GetObjectNameFromFileName(filename));
        //        RootForObject.transform.SetParent(transform);
        //        LoadObjectFromDisk();
        //    }
        //    else
        //    {
        //        filename = names[0];
        //        //create a parent for the objects we create now
        //        RootForObject = new GameObject(GetObjectNameFromFileName(filename));
        //        RootForObject.transform.SetParent(transform);
        //        Task.Run(() => CreateObjectFromGCode());
        //    }
        //}
    }

    internal IEnumerator LoadObject(string urlToFile)
    {
        if (!loading)
        {
            loading = true;
            print("click in meshcreator");
            //UnityMainThreadDispatcher.Instance().Enqueue(() =>
            //{
            //ProgressIndicator.Instance.SetMessage("Lade Objekt...");
            //ProgressIndicator.Instance.SetProgress(0f);
            //});

            slider.enabled = false;
            slider.gameObject.SetActive(false);
            ShowHideCheckBoxes.gameObject.SetActive(false);

            clearchildren();

            int startindex = urlToFile.LastIndexOf("/") + 1;
            string savePath = dataPath + FolderToRawGCodes + urlToFile.Substring(startindex);

            //create a parent for the objects we create now
            RootForObject = new GameObject(GetObjectNameFromPath(savePath));
            RootForObject.transform.SetParent(transform);
            RootForObject.transform.localPosition = Vector3.zero;
            RootForObject.transform.localScale = Vector3.one;
            RootForObject.transform.localRotation = Quaternion.identity;

            if (!CheckForExsitingObject(urlToFile))
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
                        if (!Directory.Exists(dataPath + "/RawGCodes/"))
                        {
                            Directory.CreateDirectory(dataPath + "/RawGCodes/");
                        }
                        System.IO.File.WriteAllText(savePath, www.downloadHandler.text);

                    }


                }

                //if (loading == false)
                //{
                    Task.Run(() => CreateObjectFromGCode(savePath));
                //}

            }
            else
            {
                if (loadingFromDisk == false)
                {
                    path = urlToFile;
                    loadingFromDisk = true;
                }
            }
        }
    }

    //private void LoadObjectFromDisk(string path)
    //{

    //    EnqueuedMeshes = 0;
    //    //int layernum = 0;

    //    //create object to put all mesh types in
    //    var objectName = GetObjectNameFromPath(path);

    //    var rootFolderOfObject = dataPath + "/" + objectName;

    //    foreach (var folder in Directory.GetDirectories(rootFolderOfObject))
    //    {
    //        EnqueuedMeshes += Directory.GetFiles(folder).Length;
    //    }
    //    loadingFromDisk = true;
    //    foreach (var folder in Directory.GetDirectories(rootFolderOfObject))
    //    {
    //        //create object for each mesh type
    //        //var type = new GameObject(folder.Substring(folder.LastIndexOf(@"\") + 1));
    //        //type.transform.SetParent(RootForObject.transform);

    //        //allLayerObjects.Add(type.name, new Dictionary<int, GameObject>());
    //        //parentObjects.Add(type.name, type);
    //        //parentvisible.Add(type.name, true);

    //        //Material typeMaterial = null;
    //        //typeMaterial = GetMeshTypeMaterial(type.name);


    //        foreach (var file in Directory.GetFiles(folder))
    //        {
    //            //create new object for each layer and add meshfilter and renderer
    //            //var layer = new GameObject(file.Substring(file.LastIndexOf(@"\") + 1, file.LastIndexOf(".") - file.LastIndexOf(@"\") - 1));
    //            //var meshFilter = layer.AddComponent<MeshFilter>();
    //            //var renderer = layer.AddComponent<MeshRenderer>();
    //            //renderer.material = typeMaterial;

    //            //get mesh from file
    //            var mesh = MeshSerializer.DeserializeMesh(File.ReadAllBytes(file));
    //            //meshFilter.mesh = mesh;
    //            //layer.transform.SetParent(type.transform);

    //            ////get the biggest layer number
    //            //var l = Convert.ToInt32(layer.name.Substring(layer.name.LastIndexOf(" ") + 1));
    //            //if (l > layernum)
    //            //{
    //            //    layernum = l;
    //            //}

    //            //allLayerObjects[type.name].Add(l, layer);

    //            loadQueue.Enqueue(new KeyValuePair<string, Mesh>(file.Substring(file.LastIndexOf(@"\") + 1, file.LastIndexOf(".") - file.LastIndexOf(@"\") - 1), mesh));
    //        }
    //    }

    //    //RootForObject.transform.localPosition = new Vector3(1, 1, 1);
    //    //RootForObject.transform.localScale = new Vector3(1, 1, 1);


    //    //layersvisible = layernum;
    //    //_layersvisible = layernum;
    //    //slider.SetSpan(0, layernum);
    //    //slider.SetSliderValue(layernum);
    //    ShowHideCheckBoxes.Rebuild();
    //    loadingFromDisk = false;
    //}
    private IEnumerator LoadObjectFromDiskCR(string path)
    {
        int layernum = 0;

        //create object to put all mesh types in
        var objectName = GetObjectNameFromPath(path);

        var rootFolderOfObject = dataPath + "/" + objectName;

        RootForObject.transform.localPosition = Vector3.zero;
        RootForObject.transform.localScale = Vector3.one;
        RootForObject.transform.localRotation = Quaternion.identity;

        SortedDictionary<int, List<string>> sortedLayers = new SortedDictionary<int, List<string>>();

        foreach (var folder in Directory.GetDirectories(rootFolderOfObject))
        {
            var files = Directory.GetFiles(folder);
            foreach (var file in Directory.GetFiles(folder))
            {
                var l = Convert.ToInt32(file.Substring(file.LastIndexOf(@" ") + 1, file.LastIndexOf(".") - file.LastIndexOf(@" ") - 1));
                if (!sortedLayers.ContainsKey(l))
                {
                    sortedLayers.Add(l, new List<string>());
                    sortedLayers[l].Add(file);
                }
                else
                {
                    sortedLayers[l].Add(file);
                }
            }
        }


        foreach (var layers in sortedLayers)
        {
            foreach (var file in layers.Value)
            {
                var typeString = file.Substring(file.LastIndexOf(@"\") + 1, file.LastIndexOf(' ') - file.LastIndexOf(@"\") - 1);

                if (!allLayerObjects.ContainsKey(typeString))
                {
                    var type = new GameObject(typeString);
                    type.transform.SetParent(RootForObject.transform);
                    type.transform.localScale = Vector3.one;
                    type.transform.localPosition = Vector3.zero;
                    type.transform.localRotation = Quaternion.identity;
                    allLayerObjects.Add(type.name, new Dictionary<int, GameObject>());
                    parentObjects.Add(type.name, type);
                    parentvisible.Add(type.name, true);
                }

                Material typeMaterial = null;
                typeMaterial = GetMeshTypeMaterial(typeString);

                var layer = new GameObject(file.Substring(file.LastIndexOf(@"\") + 1, file.LastIndexOf(".") - file.LastIndexOf(@"\") - 1));

                var meshFilter = layer.AddComponent<MeshFilter>();
                var renderer = layer.AddComponent<MeshRenderer>();
                renderer.material = typeMaterial;
                renderer.receiveShadows = false;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                
                //get mesh from file
                var mesh = MeshSerializer.DeserializeMesh(File.ReadAllBytes(file));
                meshFilter.mesh = mesh;
                layer.transform.SetParent(parentObjects[typeString].transform);
                layer.transform.localScale = Vector3.one;
                layer.transform.localPosition = Vector3.zero;
                layer.transform.localRotation = Quaternion.identity;
                //get the biggest layer number
                var l = Convert.ToInt32(layer.name.Substring(layer.name.LastIndexOf(" ") + 1));
                if (l > layernum)
                {
                    layernum = l;
                }

                allLayerObjects[typeString].Add(l, layer);
            }
            yield return null;
        }


        layersvisible = layernum;
        _layersvisible = layernum;
        slider.enabled = true;
        slider.gameObject.SetActive(true);
        slider.MaxValue=layernum;
        slider.SliderValue=layernum;
        ShowHideCheckBoxes.Rebuild();
        ShowHideCheckBoxes.gameObject.SetActive(true);
        StartCoroutine(closeProgress());
        loading = false;
    }
    /// <summary>
    /// gets the material from dictionary by mesh type
    /// </summary>
    /// <param name="meshTypeName"></param>
    /// <returns></returns>
    private Material GetMeshTypeMaterial(string meshTypeName)
    {
        foreach (MaterialPreference mp in materialDictionary)
        {
            if (mp.name == meshTypeName)
            {
                return mp.mat;
            }
        }

        return null;
    }

    /// <summary>
    /// checks if there is already a model for the gcode
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    private bool CheckForExsitingObject(string filename)
    {

        var objectName = GetObjectNameFromPath(filename);
        if (Directory.Exists(dataPath + "/" + objectName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// extracts the name of the model
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private string GetObjectNameFromPath(string path)
    {
        int indexOfLastBackSlash = path.LastIndexOf(@"\");
        int indexOfLastForwardSlash = path.LastIndexOf(@"/");
        int indexOfFileExtention = path.LastIndexOf(".");

        if ((indexOfLastForwardSlash > 0 || indexOfLastBackSlash > 0) && indexOfFileExtention > 0)
        {
            int startIndex = indexOfLastBackSlash > 0 ? indexOfLastBackSlash : indexOfLastForwardSlash;

            return path.Substring(startIndex + 1, indexOfFileExtention - startIndex - 1);
        }
        else
        {
            return string.Empty;
        }
    }

    public void Updateslider(SliderEventData eventData)
    {
        layersvisible = (int)eventData.NewValue;
    }

    //public void load(string path)
    //{
    //    Debug.Log("in meshcreator");
    //    slider.enabled = false;
    //    ShowHideCheckBoxes.enabled = false;
    //    clearchildren();
    //    string mainpath = Application.streamingAssetsPath;

    //    names = Directory.GetFiles(mainpath, "*.gcode");
        
    //    for (int i = 0; i < names.Length; i++)
    //    {
    //        if (names[i].Contains(name) && names[i].EndsWith(".gcode"))
    //        {
    //            if (CheckForExsitingObject(names[i]) && !_regenerateModel)
    //            {
    //                //create a parent for the objects we create now
    //                RootForObject = new GameObject(GetObjectNameFromPath(names[i]));
    //                RootForObject.transform.SetParent(transform);
    //                LoadObjectFromDisk(names[i]);
    //            }
    //            else
    //            {
    //                //create a parent for the objects we create now
    //                RootForObject = new GameObject(GetObjectNameFromPath(names[i]));
    //                RootForObject.transform.SetParent(transform);
    //                Task.Run(() => CreateObjectFromGCode(names[i]));
    //                break;
    //            }
    //        }
    //    }
    //    ShowHideCheckBoxes.enabled = true;
    //    slider.enabled = true;
    //}
    /// <summary>
    /// call this before you recreate to regenerate with new clustersizes
    /// </summary>
    void clearchildren()
    {

        Destroy(RootForObject);
        allLayerObjects.Clear();
        parentObjects.Clear();
        parentvisible.Clear();
        meshSimplifierQueue.Clear();
    }
    void CreateObjectFromGCode(string filename)//takes ages and munches on all that juicy cpu, only use if absolutely necessary
    {


        //Read the text from directly from the test.txt file
        //StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open));
        loading = true;
        filesLoadingfinished = false;
        string[] Lines = File.ReadAllLines(filename);
        print("loading " + filename);
        List<string> meshnames = new List<string>();
        int currentmesh = -1;
        Dictionary<string, List<List<Vector3>>> tmpmove = new Dictionary<string, List<List<Vector3>>>();
        Vector3 currpos = new Vector3(0, 0, 0);
        float accumulateddist = 0.0f;
        Vector3 lastpointcache = new Vector3(0, 0, 0);
        int linesread = 0;
        int layernum = 0;
        bool accumulating = false;
        float lastanglecache = 0.0f;
        float accumulatedangle = 0.0f;
        bool ismesh = false;
        foreach (string line in Lines)
        {
            linesread += 1;
            //string line = reader.ReadLine();
            if (line.StartsWith(";TYPE:"))
            {
                ismesh = true;
                string namemesh = line.Substring(6) + " " + layernum.ToString("D8");
                if (line.Substring(6).Contains("WALL") || line.Substring(6).Contains("SKIN"))
                {
                    namemesh = "WALLS " + layernum.ToString("D8");
                }
                //here i change the type of 3d printed part i print next, this only works in cura-sliced stuff, slic3r doesnt have those comments
                //print("setting type");
                if (!meshnames.Contains(namemesh))
                {
                    meshnames.Add(namemesh);
                    currentmesh = meshnames.Count - 1;
                    tmpmove[namemesh] = new List<List<Vector3>>();
                    tmpmove[namemesh].Add(new List<Vector3>());
                    //print("adding: " + line + " as: " + line.Substring(6) + " Line " + layernum + " with number " + currentmesh);
                }
                else
                {
                    currentmesh = meshnames.FindIndex((namemesh).EndsWith);
                    //print("changed mesh to: " + currentmesh + " because of " + line);
                }
                //print("currentmesh" + currentmesh);
            }
            else if (line.StartsWith(";LAYER:"))
            {
                layernum = int.Parse(line.Substring(7));
                foreach (string namepart in tmpmove.Keys)
                {
                    createlayer(tmpmove[namepart], namepart);
                }
                tmpmove.Clear();
                //todo create layer
            }
            else if ((line.StartsWith("G1") || line.StartsWith("G0")) /*&& currentmesh != -1*/ && ((layernum % layercluster) == 0 || layercluster == 1))
            {
                //here i add a point to the list of visited points of the current part
                //print("Adding object");
                /*if(currentmesh!=-1 && meshnames[currentmesh].Contains("WALLS 60"))
                {
                    print(line);
                    print(currpos);
                    print("accumulating: " + accumulating);
                }*/
                string[] parts = line.Split(' ');

                if (accumulating)
                {
                    accumulateddist += Vector3.Distance(currpos, lastpointcache);
                    accumulatedangle += Mathf.Abs(lastanglecache - Vector2.Angle(new Vector2(1, 0), new Vector2((currpos - lastpointcache).x, (currpos - lastpointcache).z)));
                }
                lastpointcache = currpos;
                lastanglecache = Vector2.Angle(new Vector2(1, 0), new Vector2((currpos - lastpointcache).x, (currpos - lastpointcache).z));

                if (!accumulating &&
                    (line.Contains("X") || line.Contains("Y") || line.Contains("Z")) &&
                    line.Contains("E") &&
                    currpos != new Vector3(0, 0, 0)
                    && currentmesh != -1)
                {
                    /*if (currentmesh != -1 && meshnames[currentmesh].Contains("WALLS 60"))
                        print("first");*/
                    //print(currentmesh);
                    string meshname = meshnames[currentmesh];
                    if (tmpmove.ContainsKey(meshname))
                    {

                        tmpmove[meshname][tmpmove[meshname].Count - 1].Add(currpos);
                    }
                    /*else
                    {
                        if (currentmesh != -1 && meshnames[currentmesh].Contains("WALLS 60"))
                            print("nah");
                    };*/
                }
                foreach (string part in parts)
                {
                    if (part.StartsWith("X"))
                    {
                        currpos.x = float.Parse(part.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                    }
                    else if (part.StartsWith("Y"))
                    {
                        currpos.z = float.Parse(part.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                    }
                    else if (part.StartsWith("Z"))
                    {
                        currpos.y = float.Parse(part.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                    }
                }
                if (((!accumulating || accumulateddist > distanceclustersize || accumulatedangle > rotationclustersize) && (ismesh || line.Contains("E"))) && (line.Contains("X") || line.Contains("Y") || line.Contains("Z")) && currpos != new Vector3(0, 0, 0))
                {
                    /*if (currentmesh != -1 && meshnames[currentmesh].Contains("WALLS 60"))
                        print("second");*/
                    if (currentmesh != -1 && tmpmove.ContainsKey(meshnames[currentmesh]))
                    {
                        string meshname = meshnames[currentmesh];
                        tmpmove[meshname][tmpmove[meshname].Count - 1].Add(currpos);
                    }

                    accumulateddist = 0.0f;
                    accumulatedangle = 0.0f;
                }
                accumulating = true;
                if (line.Contains("E") &&
                    (line.Contains("X") || line.Contains("Y") || line.Contains("Z")))
                {
                    ismesh = true;
                }
                else
                {
                    ismesh = false;
                    accumulating = false;
                    if (currentmesh != -1 && tmpmove.ContainsKey(meshnames[currentmesh]) && tmpmove[meshnames[currentmesh]][tmpmove[meshnames[currentmesh]].Count - 1].Count > 1)
                    {
                        tmpmove[meshnames[currentmesh]].Add(new List<Vector3>());
                        //createlayer(tmpmove, meshnames[currentmesh]);
                    }
                    //tmpmove.Clear();
                }
            }
            else if (line.StartsWith(";MESH:"))
            {
                ismesh = false;
            }
            /*if(meshnames[currentmesh].Contains("WALLS 60"))
                {
                print(line);
            }*/
        }
        layersvisible = layernum;
        //loading = false;
        filesLoadingfinished = true;
        newloaded = true;
    }

    void createlayer(List<List<Vector3>> tmpmoves, string meshname)
    {
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector3> newNormals = new List<Vector3>();
        List<Vector2> newUV = new List<Vector2>();
        List<int> newTriangles = new List<int>();
        List<Dictionary<int, Dictionary<int, int>>> neighbours = new List<Dictionary<int, Dictionary<int, int>>>();
        /*if(meshname.Contains("WALLS 60"))
        {
            string points = "Points for WALLS 60:";
            for(int i=0; i<tmpmoves.Count; i++)
            {
                points += "\n list " + i;
                for(int j = 0; j < tmpmoves[i].Count; j++)
                {
                    points += "\n  " + tmpmoves[i][j];
                }
            }
            tmpmoves.RemoveRange(1, tmpmoves.Count - 1);
            print(points);
        }*/
        /*
        if (meshname.Contains("WALLS"))
        {
            //Test layers
            for (int i=0; i < tmpmoves.Count; i++)
            {
                Dictionary<int, Dictionary<int,int>> nb = new Dictionary<int, Dictionary<int,int>>();
                neighbours.Add(nb);
                for(int j = i + 1; j < tmpmoves.Count; j++)
                {
                    nb.Add(j, new Dictionary<int, int>());
                    for(int k = 0; k < tmpmoves[i].Count-1; k++)
                    {
                        for(int l= 0; l < tmpmoves[j].Count; l++)
                        {
                            float[] res = distanceLot(tmpmoves[i][k], tmpmoves[i][k + 1], tmpmoves[j][l]);
                            float maxfactor = (Vector2.Distance(tmpmoves[i][k], tmpmoves[i][k + 1]) + plasticwidth) / Vector2.Distance(tmpmoves[i][k], tmpmoves[i][k + 1]);
                            if (res[0] < plasticwidth * 1.2 && res[1] < maxfactor && res[1] >= 0)
                            {
                                Vector3 dv2 = tmpmoves[j][l] - tmpmoves[j][(l + 1) % tmpmoves[j].Count];
                                Vector3 dv = tmpmoves[i][k] - tmpmoves[i][k + 1];
                                float angle= Vector3.Angle(dv2, dv);
                                Vector3 dvt = dv; dvt.x = dv.z; dvt.z = -dv.x;
                                if (Vector2.Dot(tmpmoves[i][k] - tmpmoves[j][l], dvt) > 0) {
                                    if (!nb[j].ContainsKey(k))
                                    {
                                        nb[j].Add(k, l);
                                    }
                                    tmpmoves[j].RemoveAt(l);
                                    l--;
                                }
                            }
                        }
                    }
                }
            }

        }*/
        for (int tmpmvn = 0; tmpmvn < tmpmoves.Count; tmpmvn++)
        {
            List<Vector3> tmpmove = tmpmoves[tmpmvn];
            int hasleftneighbour = 1;
            for (int j = tmpmvn; j < tmpmoves.Count; j++)
            {
                if (neighbours.Count > tmpmvn && neighbours[tmpmvn].ContainsKey(j) && neighbours[tmpmvn][j].ContainsKey(0))
                {
                    hasleftneighbour++;
                }
            }
            if (tmpmove.Count > 1)
            {


                /*if (meshnames.Contains("SKIRT"))
                {
                    for (int i = 0; i < tmpmove.Count; i++)
                    {
                        print("tmpmove is: " + tmpmove[i]);
                    }
                }*/
                //here i generate the mesh from the tmpmove list, wich is a list of points the extruder goes to
                int vstart = newVertices.Count;
                Vector3 dv = tmpmove[1] - tmpmove[0];
                Vector3 dvt = dv; dvt.x = dv.z; dvt.z = -dv.x;
                dvt = -dvt.normalized;
                newVertices.Add(tmpmove[0] - dv.normalized * 0.5f + dvt * plasticwidth * (hasleftneighbour - 0.5f));
                newVertices.Add(tmpmove[0] - dv.normalized * 0.5f - dvt * 0.5f * plasticwidth);
                newVertices.Add(tmpmove[0] - dv.normalized * 0.5f - dvt * 0.5f * plasticwidth - new Vector3(0, -0.25f, 0) * layercluster);
                newVertices.Add(tmpmove[0] - dv.normalized * 0.5f + dvt * plasticwidth * (hasleftneighbour - 0.5f) - new Vector3(0, -0.25f, 0) * layercluster);
                /*                        newVertices.Add(tmpmove[0] + dvt * 0.6f);
                                        newVertices.Add(tmpmove[0] - dvt * 0.6f);
                                        newVertices.Add(tmpmove[0] - dvt * 0.6f - new Vector3(0, -0.25f, 0) * layercluster);
                                        newVertices.Add(tmpmove[0] + dvt * 0.6f - new Vector3(0, -0.25f, 0) * layercluster);*/
                newNormals.Add((dvt.normalized * plasticwidth / 2 + new Vector3(0, plasticwidth / 2, 0) - dv.normalized * plasticwidth / 2).normalized);
                newNormals.Add((dvt.normalized * -plasticwidth / 2 + new Vector3(0, plasticwidth / 2, 0) - dv.normalized * plasticwidth / 2).normalized);
                newNormals.Add((dvt.normalized * -plasticwidth / 2 + new Vector3(0, -plasticwidth / 2, 0) - dv.normalized * plasticwidth / 2).normalized);
                newNormals.Add((dvt.normalized * plasticwidth / 2 + new Vector3(0, -plasticwidth / 2, 0) - dv.normalized * plasticwidth / 2).normalized);
                /*                        newNormals.Add((dvt.normalized * 0.5f + new Vector3(0, 0.5f, 0)).normalized);
                                        newNormals.Add((dvt.normalized * -0.5f + new Vector3(0, 0.5f, 0)).normalized);
                                        newNormals.Add((dvt.normalized * -0.5f + new Vector3(0, -0.5f, 0)).normalized);
                                        newNormals.Add((dvt.normalized * 0.5f + new Vector3(0, -0.5f, 0)).normalized);
                                        newUV.Add(new Vector2(0.0f, 0.0f));
                                        newUV.Add(new Vector2(0.0f, 1.0f));
                                        newUV.Add(new Vector2(1.0f, 1.0f));
                                        newUV.Add(new Vector2(1.0f, 0.0f));*/
                newUV.Add(new Vector2(0.0f, 0.0f));
                newUV.Add(new Vector2(0.0f, 1.0f));
                newUV.Add(new Vector2(1.0f, 1.0f));
                newUV.Add(new Vector2(1.0f, 0.0f));

                newTriangles.Add(vstart + 2);
                newTriangles.Add(vstart + 1);
                newTriangles.Add(vstart + 0); //back (those need to be in clockwise orientation for culling to work right)
                newTriangles.Add(vstart + 0);
                newTriangles.Add(vstart + 3);
                newTriangles.Add(vstart + 2);
                /*
                newTriangles.Add(vstart + 0);
                newTriangles.Add(vstart + 1);
                newTriangles.Add(vstart + 5); //top
                newTriangles.Add(vstart + 0);
                newTriangles.Add(vstart + 5);
                newTriangles.Add(vstart + 4);

                newTriangles.Add(vstart + 1);
                newTriangles.Add(vstart + 2);
                newTriangles.Add(vstart + 6);//left
                newTriangles.Add(vstart + 1);
                newTriangles.Add(vstart + 6);
                newTriangles.Add(vstart + 5);

                newTriangles.Add(vstart + 0);
                newTriangles.Add(vstart + 4);
                newTriangles.Add(vstart + 3);//right
                newTriangles.Add(vstart + 3);
                newTriangles.Add(vstart + 4);
                newTriangles.Add(vstart + 7);

                newTriangles.Add(vstart + 2);
                newTriangles.Add(vstart + 3);
                newTriangles.Add(vstart + 7);//bottom
                newTriangles.Add(vstart + 2);
                newTriangles.Add(vstart + 7);
                newTriangles.Add(vstart + 6);*/
                for (int i = 1; i < tmpmove.Count - 1; i++)
                {
                    hasleftneighbour = 1;
                    for (int j = tmpmvn; j < tmpmoves.Count; j++)
                    {
                        if (neighbours.Count > tmpmvn && neighbours[tmpmvn].ContainsKey(j) && neighbours[tmpmvn][j].ContainsKey(0))
                        {
                            hasleftneighbour++;
                        }
                    }
                    //print(tmpmove[i+1]);
                    Vector3 dv1 = tmpmove[i] - tmpmove[i - 1];
                    Vector3 dvt1 = dv1; dvt1.x = dv1.z; dvt1.z = -dv1.x;
                    Vector3 dv2 = tmpmove[i + 1] - tmpmove[i];
                    Vector3 dvt2 = dv2; dvt2.x = dv2.z; dvt2.z = -dv2.x;
                    dvt = (dvt1 + dvt2).normalized * -plasticwidth;
                    newVertices.Add(tmpmove[i] + dvt * (hasleftneighbour - 0.5f));
                    newVertices.Add(tmpmove[i] - dvt * 0.5f);
                    newVertices.Add(tmpmove[i] - dvt * 0.5f - new Vector3(0, -0.25f, 0) * layercluster);
                    newVertices.Add(tmpmove[i] + dvt * (hasleftneighbour - 0.5f) - new Vector3(0, -0.25f, 0) * layercluster);
                    newNormals.Add((dvt.normalized + new Vector3(0, 0.125f, 0)).normalized);
                    newNormals.Add((dvt.normalized + new Vector3(0, 0.125f, 0)).normalized);
                    newNormals.Add((dvt.normalized + new Vector3(0, -0.125f, 0)).normalized);
                    newNormals.Add((dvt.normalized + new Vector3(0, -0.125f, 0)).normalized);
                    newUV.Add(new Vector2(0.0f, 0.0f));
                    newUV.Add(new Vector2(0.0f, 1.0f));
                    newUV.Add(new Vector2(1.0f, 1.0f));
                    newUV.Add(new Vector2(1.0f, 0.0f));

                    newTriangles.Add(vstart + 0 + 4 * (i - 1));
                    newTriangles.Add(vstart + 1 + 4 * (i - 1));
                    newTriangles.Add(vstart + 5 + 4 * (i - 1)); //top
                    newTriangles.Add(vstart + 0 + 4 * (i - 1));
                    newTriangles.Add(vstart + 5 + 4 * (i - 1));
                    newTriangles.Add(vstart + 4 + 4 * (i - 1));

                    newTriangles.Add(vstart + 1 + 4 * (i - 1));
                    newTriangles.Add(vstart + 2 + 4 * (i - 1));
                    newTriangles.Add(vstart + 6 + 4 * (i - 1));//left
                    newTriangles.Add(vstart + 1 + 4 * (i - 1));
                    newTriangles.Add(vstart + 6 + 4 * (i - 1));
                    newTriangles.Add(vstart + 5 + 4 * (i - 1));

                    newTriangles.Add(vstart + 0 + 4 * (i - 1));
                    newTriangles.Add(vstart + 4 + 4 * (i - 1));
                    newTriangles.Add(vstart + 3 + 4 * (i - 1));//right
                    newTriangles.Add(vstart + 3 + 4 * (i - 1));
                    newTriangles.Add(vstart + 4 + 4 * (i - 1));
                    newTriangles.Add(vstart + 7 + 4 * (i - 1));

                    newTriangles.Add(vstart + 2 + 4 * (i - 1));
                    newTriangles.Add(vstart + 3 + 4 * (i - 1));
                    newTriangles.Add(vstart + 7 + 4 * (i - 1));//bottom
                    newTriangles.Add(vstart + 2 + 4 * (i - 1));
                    newTriangles.Add(vstart + 7 + 4 * (i - 1));
                    newTriangles.Add(vstart + 6 + 4 * (i - 1));
                }

                hasleftneighbour = 1;
                for (int j = tmpmvn; j < tmpmoves.Count; j++)
                {
                    if (neighbours.Count > tmpmvn && neighbours[tmpmvn].ContainsKey(j) && neighbours[tmpmvn][j].ContainsKey(0))
                    {
                        hasleftneighbour++;
                    }
                }
                dv = tmpmove[tmpmove.Count - 1] - tmpmove[tmpmove.Count - 2];
                dvt = dv; dvt.x = dv.z; dvt.z = -dv.x;
                dvt = dvt.normalized * plasticwidth;
                dv = dv.normalized * plasticwidth / 2;
                int maxi = tmpmove.Count - 2;

                /*newVertices.Add(tmpmove[maxi] + dv + dvt);
                newVertices.Add(tmpmove[maxi] + dv - dvt);
                newVertices.Add(tmpmove[maxi] + dv - dvt - new Vector3(0, -0.25f, 0) * layercluster);
                newVertices.Add(tmpmove[maxi] + dv + dvt - new Vector3(0, -0.25f, 0) * layercluster);*/
                newVertices.Add(tmpmove[maxi] + dv + dvt * (hasleftneighbour - 0.5f));
                newVertices.Add(tmpmove[maxi] + dv - dvt * 0.5f);
                newVertices.Add(tmpmove[maxi] + dv - dvt * 0.5f - new Vector3(0, -0.25f, 0) * layercluster);
                newVertices.Add(tmpmove[maxi] + dv + dvt * (hasleftneighbour - 0.5f) - new Vector3(0, -0.25f, 0) * layercluster);
                /*                        newNormals.Add((dvt.normalized * 0.5f + new Vector3(0, 0.5f, 0)).normalized);
                                        newNormals.Add((dvt.normalized * -0.5f + new Vector3(0, 0.5f, 0)).normalized);
                                        newNormals.Add((dvt.normalized * -0.5f + new Vector3(0, -0.5f, 0)).normalized);
                                        newNormals.Add((dvt.normalized * 0.5f + new Vector3(0, -0.5f, 0)).normalized);*/
                newNormals.Add((dvt + new Vector3(0, plasticwidth / 2, 0) + dv).normalized);
                newNormals.Add((-dvt + new Vector3(0, plasticwidth / 2, 0) + dv).normalized);
                newNormals.Add((-dvt + new Vector3(0, -plasticwidth / 2, 0) + dv).normalized);
                newNormals.Add((dvt + new Vector3(0, -plasticwidth / 2, 0) + dv).normalized);
                /*                        newUV.Add(new Vector2(0.0f, 0.0f));
                                        newUV.Add(new Vector2(0.0f, 1.0f));
                                        newUV.Add(new Vector2(1.0f, 1.0f));
                                        newUV.Add(new Vector2(1.0f, 0.0f));*/
                newUV.Add(new Vector2(0.0f, 0.0f));
                newUV.Add(new Vector2(0.0f, 1.0f));
                newUV.Add(new Vector2(1.0f, 1.0f));
                newUV.Add(new Vector2(1.0f, 0.0f));

                newTriangles.Add(vstart + 0 + 4 * maxi);
                newTriangles.Add(vstart + 1 + 4 * maxi);
                newTriangles.Add(vstart + 5 + 4 * maxi); //top
                newTriangles.Add(vstart + 0 + 4 * maxi);
                newTriangles.Add(vstart + 5 + 4 * maxi);
                newTriangles.Add(vstart + 4 + 4 * maxi);

                newTriangles.Add(vstart + 1 + 4 * maxi);
                newTriangles.Add(vstart + 2 + 4 * maxi);
                newTriangles.Add(vstart + 6 + 4 * maxi);//left
                newTriangles.Add(vstart + 1 + 4 * maxi);
                newTriangles.Add(vstart + 6 + 4 * maxi);
                newTriangles.Add(vstart + 5 + 4 * maxi);

                newTriangles.Add(vstart + 0 + 4 * maxi);
                newTriangles.Add(vstart + 4 + 4 * maxi);
                newTriangles.Add(vstart + 3 + 4 * maxi);//right
                newTriangles.Add(vstart + 3 + 4 * maxi);
                newTriangles.Add(vstart + 4 + 4 * maxi);
                newTriangles.Add(vstart + 7 + 4 * maxi);

                newTriangles.Add(vstart + 2 + 4 * maxi);
                newTriangles.Add(vstart + 3 + 4 * maxi);
                newTriangles.Add(vstart + 7 + 4 * maxi);//bottom
                newTriangles.Add(vstart + 2 + 4 * maxi);
                newTriangles.Add(vstart + 7 + 4 * maxi);
                newTriangles.Add(vstart + 6 + 4 * maxi);
                /*
                newTriangles.Add(vstart + 4 + 4 * maxi + 1);
                newTriangles.Add(vstart + 5 + 4 * maxi + 1);
                newTriangles.Add(vstart + 9 + 4 * maxi + 1); //top
                newTriangles.Add(vstart + 4 + 4 * maxi + 1);
                newTriangles.Add(vstart + 9 + 4 * maxi + 1);
                newTriangles.Add(vstart + 8 + 4 * maxi + 1);

                newTriangles.Add(vstart + 5 + 4 * maxi + 1);
                newTriangles.Add(vstart + 6 + 4 * maxi + 1);
                newTriangles.Add(vstart + 10 + 4 * maxi + 1);//left
                newTriangles.Add(vstart + 5 + 4 * maxi + 1);
                newTriangles.Add(vstart + 10 + 4 * maxi + 1);
                newTriangles.Add(vstart + 9 + 4 * maxi + 1);

                newTriangles.Add(vstart + 4 + 4 * maxi + 1);
                newTriangles.Add(vstart + 8 + 4 * maxi + 1);
                newTriangles.Add(vstart + 7 + 4 * maxi + 1);//right
                newTriangles.Add(vstart + 7 + 4 * maxi + 1);
                newTriangles.Add(vstart + 8 + 4 * maxi + 1);
                newTriangles.Add(vstart + 11 + 4 * maxi + 1);

                newTriangles.Add(vstart + 6 + 4 * maxi + 1);
                newTriangles.Add(vstart + 7 + 4 * maxi + 1);
                newTriangles.Add(vstart + 11 + 4 * maxi + 1);//bottom
                newTriangles.Add(vstart + 6 + 4 * maxi + 1);
                newTriangles.Add(vstart + 11 + 4 * maxi + 1);
                newTriangles.Add(vstart + 10 + 4 * maxi + 1);*/

                newTriangles.Add(vstart + 4 + 4 * maxi);
                newTriangles.Add(vstart + 5 + 4 * maxi);
                newTriangles.Add(vstart + 7 + 4 * maxi);//front
                newTriangles.Add(vstart + 7 + 4 * maxi);
                newTriangles.Add(vstart + 5 + 4 * maxi);
                newTriangles.Add(vstart + 6 + 4 * maxi);

            }
        }
        MeshcreatorInput mci = new MeshcreatorInput();
        mci.meshname = meshname;
        mci.newUV = newUV.ToArray();
        mci.newNormals = newNormals.ToArray();
        mci.newVertices = newVertices.ToArray();
        mci.newTriangles = newTriangles.ToArray();
        meshCreatorInputQueue.Enqueue(mci);
        createdLayers++;
    }
    void createmesh(string meshname, Vector3[] newVertices, Vector3[] newNormals, Vector2[] newUV, int[] newTriangles, Transform objectParent)
    {

        Mesh mesh = new Mesh();
        GameObject part = new GameObject(meshname);
        part.AddComponent(typeof(MeshFilter));
        part.AddComponent(typeof(MeshRenderer));
        var renderer = part.GetComponent<MeshRenderer>();
        renderer.material = materialDictionary[0].mat;
        renderer.enabled = false;
        renderer.receiveShadows = false;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        foreach (MaterialPreference mp in materialDictionary)
        {
            if (mp.name == meshname.Split(' ')[0])
            {
                part.GetComponent<MeshRenderer>().material = mp.mat;
            }
        }
        //part.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
        
        part.name = meshname;
        part.GetComponent<MeshFilter>().mesh = mesh;
        string meshparentname = meshname.Split(' ')[0];
        if (parentObjects.ContainsKey(meshparentname))
        {
            part.transform.SetParent(parentObjects[meshparentname].transform);
            allLayerObjects[meshparentname].Add(int.Parse(meshname.Split(' ')[1]), part);
        }
        else
        {
            GameObject parentobj = new GameObject(meshparentname);
            parentObjects.Add(meshparentname, parentobj);
            parentvisible.Add(meshparentname, true);
            part.transform.SetParent(parentObjects[meshparentname].transform);
            allLayerObjects.Add(meshparentname, new Dictionary<int, GameObject>());
            allLayerObjects[meshparentname].Add(int.Parse(meshname.Split(' ')[1]), part);
            parentobj.transform.SetParent(RootForObject.transform);
            parentobj.transform.localPosition = Vector3.zero;
            parentobj.transform.localScale = Vector3.one;
            parentobj.transform.localRotation = Quaternion.identity;
        }
        part.transform.localPosition = Vector3.zero;
        part.transform.localScale = Vector3.one;
        part.transform.localRotation = Quaternion.identity;
        mesh.vertices = newVertices;
        mesh.normals = newNormals;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
        MeshSimplifier meshSimplifier = new MeshSimplifier();
        meshSimplifier.Initialize(mesh);
        MeshSimplifierstruct msc = new MeshSimplifierstruct();
        msc.MeshSimplifier = meshSimplifier;
        msc.MeshFilter = part.GetComponent<MeshFilter>();
        //lock (meshSimplifierQueueLock)
        {
            meshSimplifierQueue.Enqueue(msc);
        }

        simplifypossible = true;
    }

    void simplyfyOne()
    {
        lock (meshSimplifierQueueLock)
        {

            if (meshSimplifierQueue.Count > 0)
            {
                //for (int i = 0; i < meshSimplifierQueue.Count; i++)
                //{
                if (meshSimplifierQueue.Peek().simplified == false)
                {
                    meshSimplifierQueue.Peek().MeshSimplifier.SimplifyMesh(meshsimplifyquality);
                    meshSimplifierQueue.Peek().simplified = true;
                    simplifiedLayers++;
                }
                //}
                //if (!meshSimplifierQueue[meshSimplifierQueue.Count - 1].simplified)
                //    simplifypossible = false;
            }
            issimplifying = false;
        }

        return;
    }

    private void SaveLayerAsAsset(Mesh mesh, string folder, string fileName)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        //Write the mesh to disk again
        File.WriteAllBytes(folder + fileName, MeshSerializer.SerializeMesh(mesh));
    }



    float[] distanceLot(Vector2 lp1, Vector2 lp2, Vector2 p)
    {
        Vector2 v = lp2 - lp1;
        float z = (p.x - lp1.x - (lp1.y + p.y) * v.y / v.x) / ((v.y * v.y / v.x) + v.x);
        Vector2 distvec = lp1 + v * z - p;
        float[] result = { distvec.magnitude, z };
        return result;
        //Gerade: x(i)+z*(x(i+1)-x(i))
        //lotpunkt: y(j)
        //lotgerade y(i)+w*n(x(i+1)-x(i))
        //v=(x(i+1)-x(i))
        //daher: xi1+z*(v1)=yi1-w*(v2)
        //und: xi2+z*(v2)=yi2+w*(v1) 
        //umgeformt: (xi2+z*(v2)+yi2)*(v2)/(v1)=w*(v2)
        //addiert: (xi2+z*(v2)+yi2)*(v2)/(v1)+xi1+z*(v1)=yi1
        //auflösen nach z: (z*v2²)/v1+z*v1=yi1-xi1-(xi2+yi2)*v2/v1
        //weiter auflösen nach z: z*((v2²/v1)+v1)=yi1-xi1-(xi2+yi2)*v2/v1
        //noch weiter auflösen: z=(yi1-xi1-(xi2+yi2)*v2/v1)/((v2²/v1)+v1)
        //sqrt((xi1+zv1-yj1)²+(xi2+zv2-yj2)²)
    }
    void printbounding(Vector3[] arr)
    {
        float minx = float.MaxValue;
        float miny = float.MaxValue;
        float minz = float.MaxValue;
        float maxx = float.MinValue;
        float maxy = float.MinValue;
        float maxz = float.MinValue;
        foreach (Vector3 vec in arr)
        {
            if (vec.x < minx)
            {
                minx = vec.x;
            }
            if (vec.y < miny)
            {
                miny = vec.y;
            }
            if (vec.z < minz)
            {
                minz = vec.z;
            }
            if (vec.x > maxx)
            {
                maxx = vec.x;
            }
            if (vec.y > maxy)
            {
                maxy = vec.y;
            }
            if (vec.z > maxz)
            {
                maxz = vec.z;
            }
        }
        print("min :" + minx + "/" + miny + "/" + minz);
        print("max :" + maxx + "/" + maxy + "/" + maxz);
    }
    public void TogglePartActive(string name)
    {
        bool newvisiblestate = !parentvisible[name];
        parentObjects[name].SetActive(newvisiblestate);
        parentvisible[name] = newvisiblestate;
    }

    public void Update()
    {
        if (layersvisible != _layersvisible)
        {
            _layersvisible = layersvisible;
            foreach (KeyValuePair<string, Dictionary<int, GameObject>> parentobj in allLayerObjects)
            {
                foreach (KeyValuePair<int, GameObject> layer in parentobj.Value)
                {
                    if (layer.Key > layersvisible)
                    {
                        layer.Value.SetActive(false);
                    }
                    else
                    {
                        layer.Value.SetActive(true);
                    }
                }
            }
        }
        if (newloaded)
        {
            newloaded = false;
            slider.MaxValue= layersvisible;
            slider.SliderValue=layersvisible;
            ShowHideCheckBoxes.Rebuild();
        }

        if (loadingFromDisk == true)
        {
            loadingFromDisk = false;
            StartCoroutine(LoadObjectFromDiskCR(path));
        }

        if (loadQueue.Count > 0 && loadingFromDisk)
        {

            var KeyValuepPairLayer = loadQueue.Dequeue();
            dequeuedMeshes++;

            string parent = KeyValuepPairLayer.Key.Split(' ')[0];

            if (!allLayerObjects.ContainsKey(parent))
            {
                //create object for each mesh type
                var type = new GameObject(parent);
                type.transform.SetParent(RootForObject.transform);

                allLayerObjects.Add(type.name, new Dictionary<int, GameObject>());
                parentObjects.Add(type.name, type);
                parentvisible.Add(type.name, true);
            }

            Material typeMaterial = null;
            typeMaterial = GetMeshTypeMaterial(parent);

            //create new object for each layer and add meshfilter and renderer
            var layer = new GameObject(KeyValuepPairLayer.Key);
            var meshFilter = layer.AddComponent<MeshFilter>();
            var renderer = layer.AddComponent<MeshRenderer>();
            renderer.material = typeMaterial;

            meshFilter.mesh = KeyValuepPairLayer.Value;
            layer.transform.SetParent(parentObjects[parent].transform);

            //get the biggest layer number
            var l = Convert.ToInt32(layer.name.Substring(layer.name.LastIndexOf(" ") + 1));
            if (l > layernum)
            {
                layernum = l;
            }

            allLayerObjects[parent].Add(l, layer);
            dequeuedMeshes++;
            if (dequeuedMeshes == EnqueuedMeshes)
            {
                RootForObject.transform.localPosition = new Vector3(1, 1, 1);
                RootForObject.transform.localScale = new Vector3(1, 1, 1);


                layersvisible = layernum;
                _layersvisible = layernum;
                slider.MaxValue=layernum;
                slider.SliderValue=layernum;
                ShowHideCheckBoxes.Rebuild();
                StartCoroutine(closeProgress());
                loadingFromDisk = false;
                loading = false;
            }

        }

        //lock (meshSimplifierQueueLock)
        {


            if (meshSimplifierQueue.Count > 0)
            {
                if (meshSimplifierQueue.Peek().simplified)
                {
                    var layer = meshSimplifierQueue.Peek();
                    Mesh destMesh = layer.MeshSimplifier.ToMesh();
                    layer.MeshFilter.mesh = destMesh;
                    layer.MeshFilter.gameObject.GetComponent<MeshRenderer>().enabled = true;
                    string folder = dataPath + "/" + RootForObject.gameObject.name + "/" + layer.MeshFilter.gameObject.transform.parent.gameObject.name + "/";
                    string fileName = layer.MeshFilter.gameObject.name + ".mesh";
                    SaveLayerAsAsset(destMesh, folder, fileName);
                    lock (meshSimplifierQueueLock)
                    {
                        meshSimplifierQueue.Dequeue();
                    }
                }
                if (!issimplifying && meshSimplifierQueue.Count > 0)
                {
                    issimplifying = true;
                    Task.Run(() => simplyfyOne());

                    //if (simplify.ThreadState == ThreadState.Stopped ||simplify.ThreadState == ThreadState.Unstarted )
                    //{
                    //    try
                    //    {
                    //        simplify = new Thread(simplyfyOne);
                    //        issimplifying = true;
                    //        simplify.Start();
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //    }
                    //}
                }
            }
            else
            {
                //Debug.Log("nothing to simplify");
            }
        }
        if (meshCreatorInputQueue.Count > 0)
        {
            MeshcreatorInput mci = meshCreatorInputQueue.Dequeue();
            createmesh(mci.meshname, mci.newVertices, mci.newNormals, mci.newUV, mci.newTriangles, RootForObject.transform);
        }

        if (createdLayers == simplifiedLayers && loading && filesLoadingfinished)
        {
            StartCoroutine(closeProgress());
            filesLoadingfinished = false;
            simplifiedLayers = 0;
            createdLayers = 0;
            ShowHideCheckBoxes.gameObject.SetActive(true);
            slider.enabled = true;
            slider.gameObject.SetActive(true);
            loading = false;
        }

    }

    private IEnumerator closeProgress()
    {
        //if (ProgressIndicator.Instance.IsLoading)
        //{
        //    // Give the user a final notification that loading has finished (optional)
        //    ProgressIndicator.Instance.SetMessage("Object geladen.");
        //    ProgressIndicator.Instance.SetProgress(1f);

        //    // Close the loading dialog
        //    // ProgressIndicator.Instance.IsLoading will report true until its 'Closing' animation has ended
        //    // This typically takes about 1 second
        //    ProgressIndicator.Instance.Close();

        //    while (ProgressIndicator.Instance.IsLoading)
        //    {
        //        yield return null;
        //    }
        //}
        yield return null;
    }

    internal void ToogleReload()
    {
        _regenerateModel = !_regenerateModel;
    }
}
