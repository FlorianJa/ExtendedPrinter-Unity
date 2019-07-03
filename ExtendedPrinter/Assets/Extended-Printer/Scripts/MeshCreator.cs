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
    MeshLoader loader = new MeshLoader();

    public string[] names;
    [Serializable]
    public struct MaterialPreference
    {
        public string name;
        public Material mat;
    }


    public MaterialPreference[] materialDictionary;
    public int layersvisible = 0;
    public ButtonGeneratorShowHideMenu ShowHideCheckBoxes;
    public PinchSlider slider;
    public bool loading = false;
    private int _layersvisible = 0;
    internal Dictionary<string, Dictionary<int, GameObject>> allLayerObjects = new Dictionary<string, Dictionary<int, GameObject>>();
    internal Dictionary<string, GameObject> parentObjects = new Dictionary<string, GameObject>();
    internal Dictionary<string, bool> parentvisible = new Dictionary<string, bool>();

    internal GameObject RootForObject;

    internal bool newloaded;

    void Start()
    {
        if (slider)
        {
            slider.OnValueUpdated.AddListener(Updateslider);
        }
        loader.Start();
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
        loader.Clear();
    }

    internal void createmesh(string meshname, Vector3[] newVertices, Vector3[] newNormals, Vector2[] newUV, int[] newTriangles, Transform objectParent)
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
            loader.meshSimplifierQueue.Enqueue(msc);
        }

        loader.simplifypossible = true;
    }

    internal IEnumerator LoadObject(string refs_download)
    {
        return loader.LoadObjectFromNet(refs_download, this);
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
        loader.Update(this);
    }

    internal KeyValuePair<string,int> createLayerObjects(KeyValuePair<String,Mesh> KeyValuepPairLayer)
    {

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
        int layernum=Convert.ToInt32(layer.name.Substring(layer.name.LastIndexOf(" ") + 1));
        allLayerObjects[parent].Add(layernum, layer);
        return new KeyValuePair<string, int>(parent,layernum);
    }

    internal void endloading(int layernum)
    {
        RootForObject.transform.localPosition = new Vector3(1, 1, 1);
        RootForObject.transform.localScale = new Vector3(1, 1, 1);
        

        layersvisible = layernum;
        _layersvisible = layernum;
        slider.enabled = true;
        slider.gameObject.SetActive(true);
        if (layernum > 0)
        {
            slider.MaxValue = layernum;
            slider.SliderValue = layernum;
        }
        ShowHideCheckBoxes.Rebuild();
        ShowHideCheckBoxes.gameObject.SetActive(true);
        StartCoroutine(loader.closeProgress());
        loading = false;
    }

    internal void disableUI()
    {
        slider.enabled = false;
        slider.gameObject.SetActive(false);
        ShowHideCheckBoxes.gameObject.SetActive(false);

        clearchildren();
    }

    internal void createBlankObject(string savePath)
    {
        //create a parent for the objects we create now
        RootForObject = new GameObject(loader.GetObjectNameFromPath(savePath));
        RootForObject.transform.SetParent(transform);
        RootForObject.transform.localPosition = Vector3.zero;
        RootForObject.transform.localScale = Vector3.one;
        RootForObject.transform.localRotation = Quaternion.identity;
    }

    internal void CreateTypeObject(string typeString)
    {

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

    }

    internal int createLayerObject(string layername, string typeString, Mesh mesh)
    {


        Material typeMaterial = null;
        typeMaterial = GetMeshTypeMaterial(typeString);
        var layer = new GameObject(layername);
        var meshFilter = layer.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        var renderer = layer.AddComponent<MeshRenderer>();
        renderer.material = typeMaterial;
        renderer.receiveShadows = false;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        layer.transform.SetParent(parentObjects[typeString].transform);
        layer.transform.localScale = Vector3.one;
        layer.transform.localPosition = Vector3.zero;
        layer.transform.localRotation = Quaternion.identity;
        var l = Convert.ToInt32(layer.name.Substring(layer.name.LastIndexOf(" ") + 1));
        allLayerObjects[typeString].Add(l, layer);
        return l;
    }
}
