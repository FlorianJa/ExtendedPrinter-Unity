//using System.Collections;
//using System.Collections.Generic;

//using Microsoft.MixedReality.Toolkit.UI;
//using UnityEngine;

//public class ButtonGeneratorShowHideMenu : MonoBehaviour
//{
//    public GameObject CheckboxPrefab;
//    public GameObject GCodeRoot;
//    private List<GameObject> Checkboxes = new List<GameObject>();
//    // Use this for initialization
//    void Start()
//    {

//    }
//    public void Rebuild()
//    {
//        foreach (GameObject button in Checkboxes)
//        {
//            Destroy(button);
//        }
//        List<string> names = new List<string>();
//        foreach (Transform part in GCodeRoot.transform.GetChild(0))
//        {
//            if (!names.Contains(part.gameObject.name))
//                names.Add(part.gameObject.name);
//        }

//        foreach (Transform child in transform)
//        {
//            Destroy(child.gameObject);
//        }
//        GetComponent<CheckBoxShowHideMenuInteractionReceiver>().interactables.Clear();
//        GetComponent<ObjectCollection>().NodeList.RemoveRange(0, GetComponent<ObjectCollection>().NodeList.Count);
//        //GetComponent<ObjectCollection>().NodeList = new List<CollectionNode>();
//        foreach (string name in names)
//        {
//            GameObject checkBox = Instantiate(CheckboxPrefab, new Vector3(0, 0, 0), Quaternion.identity);
//            checkBox.name = name;
//            checkBox.GetComponent<InteractiveToggle>().SetState(true);
//            checkBox.GetComponent<LabelTheme>().Default = name;
//            checkBox.transform.parent = transform;
//            Checkboxes.Add(checkBox);
//            GetComponent<CheckBoxShowHideMenuInteractionReceiver>().interactables.Add(checkBox);
//        }
//        GetComponent<ObjectCollection>().UpdateCollection();
//    }
//    // Update is called once per frame
//    void Update()
//    {

//    }
//}
