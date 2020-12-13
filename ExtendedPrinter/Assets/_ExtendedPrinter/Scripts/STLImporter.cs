using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Parabox.Stl;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class STLImporter : MonoBehaviour
{

    public Material DefaultMaterial;

    public GameObject STLContainer;

    public async Task<GameObject> ImportAsync(string stlPath, Transform parent, bool smooth = true)
    {
        if (!string.IsNullOrEmpty(stlPath) && File.Exists(stlPath))
        {
            while (parent.childCount > 0)
            {
                DestroyImmediate(parent.GetChild(0).gameObject);
            }

            var stlContainer = Instantiate<GameObject>(STLContainer, this.transform);
            //stlContainer.transform.SetParent(parent, false);

            var min = new List<Vector3>();
            var max = new List<Vector3>();

            var meshes = await Importer.ImportAsync(stlPath, CoordinateSpace.Right, UpAxis.Y, smooth);

            foreach (var mesh in meshes)
            {
                var tmp = new GameObject();
                tmp.AddComponent<MeshFilter>().mesh = mesh;
                var renderer = tmp.AddComponent<MeshRenderer>();
                if(DefaultMaterial != null)
                {
                    renderer.material = DefaultMaterial;
                }
                min.Add(renderer.bounds.min);
                max.Add(renderer.bounds.max);
                Debug.Log(renderer.bounds.center);
                Debug.Log( renderer.bounds.size);
                tmp.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                tmp.transform.SetParent(stlContainer.transform);
            }

            var absMin = new Vector3();
            absMin.x = min.Min(x => x.x);
            absMin.y = min.Min(x => x.y);
            absMin.z = min.Min(x => x.z);

            var absMax = new Vector3();
            absMax.x = max.Max(x => x.x);
            absMax.y = max.Max(x => x.y);
            absMax.z = max.Max(x => x.z);

            var size = new Vector3();
            size.x = (absMax.x - absMin.x) / 1000f;
            size.y = (absMax.y - absMin.y) / 1000f;
            size.z = (absMax.z - absMin.z) / 1000f;

            var center = new Vector3();
            center.x = (absMin.x + (absMax.x - absMin.x) / 2f) / 1000f;
            center.y = (absMin.y + (absMax.y - absMin.y) / 2f) / 1000f;
            center.z = (absMin.z + (absMax.z - absMin.z) / 2f) / 1000f;

            var collider = stlContainer.GetComponent<BoxCollider>();
            collider.center += new Vector3(center.x, 0, center.z);
            collider.size = size;
                        
            stlContainer.transform.localPosition += new Vector3(0,center.y,0);
            foreach (Transform child in stlContainer.transform)
            {
                child.localPosition += new Vector3(0, -center.y, 0);
            }

            return stlContainer;
        }
        return null;
    }

}
