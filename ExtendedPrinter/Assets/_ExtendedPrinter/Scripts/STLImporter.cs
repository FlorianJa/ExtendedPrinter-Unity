using Parabox.Stl;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class STLImporter : MonoBehaviour
{

    public Material DefaultMaterial;

    public void Import(string stlPath, Transform parent, bool smooth = true)
    {
        if (!string.IsNullOrEmpty(stlPath) && File.Exists(stlPath))
        {


            var meshes = Importer.Import(stlPath, CoordinateSpace.Right, UpAxis.Y, true);

            foreach (var mesh in meshes)
            {
                var tmp = new GameObject();
                tmp.AddComponent<MeshFilter>().mesh = mesh;
                var renderer = tmp.AddComponent<MeshRenderer>();
                if(DefaultMaterial != null)
                {
                    renderer.material = DefaultMaterial;
                }
                tmp.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                tmp.transform.SetParent(parent);
            }
        }
    }

}
