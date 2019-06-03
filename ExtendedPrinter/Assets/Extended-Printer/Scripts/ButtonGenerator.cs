using HoloToolkit.Examples.InteractiveElements;
using HoloToolkit.Examples.UX;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.Collections;
using OctoprintClient;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class ButtonGenerator : MonoBehaviour {

    public GameObject ButtonPrefab;
    public GameObject CheckBoxPrefab;
    public OctoPrintConnector connector;
    public MeshCreator MeshCreator;

    private bool MenuCreated = false;
    private Task GetFileTaks;

    private List<OctoprintFile> AllFile;

    private bool AllFileNameReceived = false;

    // Use this for initialization
    void Start()
    {
        AllFile = new List<OctoprintFile>();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (connector.Connected && MenuCreated == false)
        {
            if (GetFileTaks == null || GetFileTaks.Status != TaskStatus.Running)
            {
                GetFileTaks = Task.Run(() =>
                {
                    var rootfolder = connector.GetAllFiles();
                    foreach (var file in rootfolder.octoprintFiles)
                    {
                        AllFile.Add(file);

                    }
                    AllFileNameReceived = true;
                });
            }
        }

        if(AllFileNameReceived && MenuCreated == false)
        {
            foreach (var file in AllFile)
            {
                var filenameSplitted = file.Name.Split('.')[0];
                GameObject button = Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity);
                button.name = filenameSplitted;
                button.GetComponent<CompoundButtonText>().Text = filenameSplitted;
                button.transform.parent = transform;
                var progress = button.AddComponent<ProgressLaunchButton>();
                progress.UrlToFile = file.Refs_download;
                progress.MeshCreator = MeshCreator;

                GetComponent<ButtonInteractionReceiver>().interactables.Add(button);

            }

            GetComponent<ObjectCollection>().UpdateCollection();
            MenuCreated = true;
        }
    }
}




//string mainpath = Application.streamingAssetsPath;
//string[] names = Directory.GetFiles(mainpath);

//foreach (string name in names)
//{
//    if (name.EndsWith(".gcode"))
//    {
//        GameObject buttn = Instantiate(ButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
//        string realname = name.Split('\\')[name.Split('\\').Length - 1];
//        realname = realname.Split('.')[0];
//        buttn.name = realname;
//        buttn.GetComponent<CompoundButtonText>().Text = realname;
//        buttn.transform.parent = transform;
//        GetComponent<ButtonInteractionReceiver>().interactables.Add(buttn);
//        print("added " + realname);
//    }
//}

//var checkbox = Instantiate(CheckBoxPrefab, Vector3.zero, Quaternion.identity);
//checkbox.name = "Reload";
//checkbox.GetComponent<InteractiveToggle>().SetState(false);
//checkbox.GetComponent<LabelTheme>().Default = "Regenerate";
//checkbox.transform.parent = transform;
//GetComponent<CheckBoxInteractionReceiver>().interactables.Add(checkbox);


