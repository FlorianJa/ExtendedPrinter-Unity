using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MeshLoader
    {

        public MeshLoaderDisk disk = new MeshLoaderDisk();
        public MeshLoaderNet net = new MeshLoaderNet();
        public GCodeMeshGenerator mc = new GCodeMeshGenerator();


        public float meshsimplifyquality = 0.5f;
        private bool issimplifying = false;
        public bool simplifypossible = false;
        public Queue<MeshSimplifierstruct> meshSimplifierQueue = new Queue<MeshSimplifierstruct>();
        private Queue<KeyValuePair<string, Mesh>> loadQueue = new Queue<KeyValuePair<string, Mesh>>();
        private object meshSimplifierQueueLock = new object();
        internal int simplifiedLayers;
        private object MeshCreatorInputQueueLock = new object();
        internal string dataPath;
        private bool _regenerateModel = false;
        internal bool filesLoadingfinished = false;
        internal string path;
        internal bool loadingFromDisk;
        private int EnqueuedMeshes;
        private int dequeuedMeshes = 0;

        private int layernum;


        public void simplyfyOne()
        {
            lock (meshSimplifierQueueLock)
            {

                if (meshSimplifierQueue.Count > 0)
                {
                    if (meshSimplifierQueue.Peek().simplified == false)
                    {
                        meshSimplifierQueue.Peek().MeshSimplifier.SimplifyMesh(meshsimplifyquality);
                        meshSimplifierQueue.Peek().simplified = true;
                        simplifiedLayers++;
                    }
                }
                issimplifying = false;
            }

            return;
        }

        internal void ToogleReload()
        {
            _regenerateModel = !_regenerateModel;
        }

        public IEnumerator LoadObjectFromDiskCR(string path, MeshCreator mc)
        {
            return disk.LoadObjectFromDiskCR(path, this, mc);
        }
        public IEnumerator LoadObjectFromNet(string urlToFile, MeshCreator source)
        {
            return net.LoadObject(urlToFile, source, this);

        }

        internal void Initialize()
        {
            dataPath = Application.persistentDataPath;
            string mainpath = Application.streamingAssetsPath;
        }

        internal void Clear()
        {
            meshSimplifierQueue.Clear();
        }

        internal void Update(MeshCreator source)
        {

            if (loadingFromDisk == true)
            {
                loadingFromDisk = false;
                source.StartCoroutine(LoadObjectFromDiskCR(path, source));
            }

            if (loadQueue.Count > 0 && loadingFromDisk)
            {

                KeyValuePair<string, Mesh> KeyValuepPairLayer = loadQueue.Dequeue();
                dequeuedMeshes++;

                KeyValuePair<String, int> LayerInfo = source.createLayerObjects(KeyValuepPairLayer);
                string parent = LayerInfo.Key;

                //get the biggest layer number
                var l = LayerInfo.Value;
                if (l > layernum)
                {
                    layernum = l;
                }

                dequeuedMeshes++;
                if (dequeuedMeshes == EnqueuedMeshes)
                {
                    source.endloading(layernum);

                    source.StartCoroutine(closeProgress());
                    loadingFromDisk = false;
                    source.loading = false;
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
                        string folder = dataPath + "/" + source.RootForObject.gameObject.name + "/" + layer.MeshFilter.gameObject.transform.parent.gameObject.name + "/";
                        string fileName = layer.MeshFilter.gameObject.name + ".mesh";
                        disk.SaveLayerAsAsset(destMesh, folder, fileName);
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

            mc.Update(source, this);


            if (mc.createdLayers == simplifiedLayers && source.loading && filesLoadingfinished)
            {
                source.StartCoroutine(closeProgress());
                filesLoadingfinished = false;
                simplifiedLayers = 0;
                mc.createdLayers = 0;
                source.endloading(layernum);
            }

        }

        internal IEnumerator closeProgress()
        {
            yield return null;
        }

        /// <summary>
        /// checks if there is already a model for the gcode
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        internal bool CheckForExsitingObject(string filename)
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
        internal string GetObjectNameFromPath(string path)
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

    }
