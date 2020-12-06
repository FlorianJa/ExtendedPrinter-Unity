using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolderInformation
{
    public List<OctoprintFileInfo> files { get; set; }
    public long free { get; set; }
    public long total { get; set; }
}

public class Refs
{
    public string download { get; set; }
    public string resource { get; set; }
}

public class OctoprintFileInfo
{
    public int date { get; set; }
    public string display { get; set; }
    public string hash { get; set; }
    public string name { get; set; }
    public string origin { get; set; }
    public string path { get; set; }
    public Refs refs { get; set; }
    public int size { get; set; }
    public string type { get; set; }
    public List<string> typePath { get; set; }
}
