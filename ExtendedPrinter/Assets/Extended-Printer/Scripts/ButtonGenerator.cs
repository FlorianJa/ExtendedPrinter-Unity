using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using OctoprintClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ButtonGenerator : MonoBehaviour
{

    public GameObject ButtonPrefab;
    public GameObject CheckBoxPrefab;
    public OctoPrintConnector connector;
    public MeshCreator MeshCreator;
    public ToolTip ToolTip;

    private bool MenuCreated = false;
    private Task GetFileTaks;

    private List<OctoprintFile> AllFile;

    private bool AllFileNameReceived = false;
    private List<GameObject> allButtons = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        AllFile = new List<OctoprintFile>();
        print("start button generator");
    }

    // Update is called once per frame
    void Update()
    {
        //print("0");
        if (connector.Connected && MenuCreated == false)
        {
            //print("1");
            if (GetFileTaks == null)
            {
                //print("2");
                GetFileTaks = Task.Run(() =>
                {
                    //print("3");
                    var rootfolder = connector.GetAllFiles();
                    foreach (var file in rootfolder.octoprintFiles)
                    {
                        AllFile.Add(file);

                    }
                    AllFileNameReceived = true;
                });
            }
        }

        if (AllFileNameReceived && MenuCreated == false)
        {
            //print("4");
            foreach (var file in AllFile)
            {
                var filenameSplitted = file.Name.Split('.')[0];
                GameObject button = Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity);
                button.name = filenameSplitted;
                button.GetComponentInChildren<TextMesh>().text = filenameSplitted;
                button.transform.parent = transform;
                button.GetComponent<Interactable>().OnClick.AddListener(() => {

                    print("button clicked");// Debug.Log("clicked");
                    if (!MeshCreator.loading)
                    {
                        DateTime time = new DateTime();
                        time = time.AddSeconds((double)file.GcodeAnalysis.EstimatedPrintTime);
                        print(file.GcodeAnalysis.EstimatedPrintTime + " " + time);
                        var filamentLength = file.GcodeAnalysis.FilamentLength / 1000f;
                        ToolTip.ToolTipText = String.Format("{0}\nDruckdauer: {1} hh:mm\nFilament: {2}m", file.Name, time.ToShortTimeString(), filamentLength.ToString("F"));

                        StartCoroutine(MeshCreator.LoadObject(file.Refs_download));
                    }
                });
                button.GetComponent<Interactable>().IsGlobal = true;
                allButtons.Add(button);
            }

            GetComponent<GridObjectCollection>().UpdateCollection();

            foreach (var button in allButtons)
            {
                button.transform.localRotation = Quaternion.identity;
            }
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


