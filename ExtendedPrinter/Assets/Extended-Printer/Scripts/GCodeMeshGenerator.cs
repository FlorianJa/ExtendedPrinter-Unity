using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GCodeMeshGenerator
{

    private float plasticwidth = 0.6f;
    public float rotationclustersize = 180.0f;
    public float distanceclustersize = 3.5f;
    public int layercluster = 1;
    internal int createdLayers;

    private Queue<MeshCreatorInput> meshCreatorInputQueue = new Queue<MeshCreatorInput>();
    internal void CreateObjectFromGCode(string[] Lines, MeshLoader loader, MeshCreator mc)//takes ages and munches on all that juicy cpu, only use if absolutely necessary
    {


        //Read the text from directly from the test.txt file
        //StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open));
        mc.loading = true;
        loader.filesLoadingfinished = false;
        //mc.print("loading " + filename);
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
                    createlayer(tmpmove[namepart], namepart, loader);
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
        mc.layersvisible = layernum;
        //loading = false;
        loader.filesLoadingfinished = true;
        mc.newloaded = true;
    }

    void createlayer(List<List<Vector3>> tmpmoves, string meshname, MeshLoader loader)
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
        MeshCreatorInput mci = new MeshCreatorInput
        {
            meshname = meshname,
            newUV = newUV.ToArray(),
            newNormals = newNormals.ToArray(),
            newVertices = newVertices.ToArray(),
            newTriangles = newTriangles.ToArray()
        };
        meshCreatorInputQueue.Enqueue(mci);
        createdLayers++;
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

    internal void Update(MeshCreator source, MeshLoader loader)
    {

        if (meshCreatorInputQueue.Count > 0)
        {
            MeshCreatorInput mci = meshCreatorInputQueue.Dequeue();
            source.createmesh(mci.meshname, mci.newVertices, mci.newNormals, mci.newUV, mci.newTriangles, source.RootForObject.transform);
        }

    }
}