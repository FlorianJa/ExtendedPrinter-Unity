using System.Collections;
using System.Collections.Generic;

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class ButtonGeneratorShowHideMenu : MonoBehaviour
{
    public GameObject CheckboxPrefab;
    public GameObject GCodeRoot;
    public MeshCreator MeshCreator;
    public List<GameObject> checkBoxes = new List<GameObject>();
    // Use this for initialization
    void Start()
    {

    }
    public void Rebuild()
    {
        //foreach (GameObject button in Checkboxes)
        //{
        //    Destroy(button);
        //}
        //List<string> names = new List<string>();
        //foreach (Transform part in GCodeRoot.transform.GetChild(0))
        //{
        //    if (!names.Contains(part.gameObject.name))
        //        names.Add(part.gameObject.name);
        //}

        //foreach (Transform child in transform)
        //{
        //    Destroy(child.gameObject);
        //}
        //GetComponent<CheckBoxShowHideMenuInteractionReceiver>().Interactables.Clear();
        //GetComponent<GridObjectCollection>().NodeList.RemoveRange(0, GetComponent<ObjectCollection>().NodeList.Count);
        //GetComponent<ObjectCollection>().NodeList = new List<CollectionNode>();

        foreach (var box in checkBoxes)
        {
            DestroyImmediate(box);
        }
        checkBoxes.Clear();

        foreach (Transform child in GCodeRoot.transform.GetChild(0).transform)
        {
            var name = child.gameObject.name;
            GameObject checkBox = Instantiate(CheckboxPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            checkBoxes.Add(checkBox);
            checkBox.name = name;
            checkBox.transform.parent = transform;
            
            //meshcreator.TogglePartActive(obj.name);
            checkBox.GetComponentInChildren<TextMesh>().text = name;
            //checkBox.GetComponent<Interactable>().IsGlobal = false;
            checkBox.GetComponent<Interactable>().IncreaseDimension();
            checkBox.GetComponent<Interactable>().OnClick.AddListener(() =>
            {
                Debug.Log("click in checkbox");
                MeshCreator.TogglePartActive(name);
            });


            
        }
        GetComponent<GridObjectCollection>().UpdateCollection();

        foreach (var checkBox in checkBoxes)
        {
            checkBox.transform.localRotation = Quaternion.identity;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
