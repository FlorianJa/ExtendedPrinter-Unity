using Parabox.Stl;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class STLImporter : MonoBehaviour
{
    public void Import(string stlPath, Transform parent)
    {
        if (!string.IsNullOrEmpty(stlPath) && File.Exists(stlPath))
        {
            var meshes = Importer.Import(stlPath, CoordinateSpace.Right, UpAxis.Y, true);

            foreach (var mesh in meshes)
            {
                var tmp = new GameObject();
                tmp.AddComponent<MeshFilter>().mesh = mesh;
                tmp.AddComponent<MeshRenderer>();//.material = material;
                tmp.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                tmp.transform.SetParent(parent);
            }
        }
    }
}
