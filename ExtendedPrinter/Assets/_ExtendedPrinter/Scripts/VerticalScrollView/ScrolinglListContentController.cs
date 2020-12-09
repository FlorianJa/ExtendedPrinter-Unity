using Assets._ExtendedPrinter.Scripts.Helper;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ScrolinglListContentController : MonoBehaviour
{
    [SerializeField]
    private ScrollingObjectCollection scrollingObjectCollection;

    [SerializeField]
    private GridObjectCollection gridObjectCollection;

    [SerializeField]
    private OctoprinController octoprintConnector;

    [SerializeField]
    private GameObject ListItemPrefab;

    public StringEvent ModelSelected;


    private void OnEnable()
    {
        if (octoprintConnector != null)
        {
            octoprintConnector.FileAdded.AddListener(Refresh);
            if (octoprintConnector.WebsocketConnected)
            {
                Refresh();
            }
            else
            {
                octoprintConnector.WebsocketConnectedEvent.AddListener(Refresh);
            }
        }
    }

    public void AddContent(Interactable item)
    {
        var modelName = item.gameObject.GetComponent<ButtonConfigHelper>().MainLabelText;
        item.OnClick.AddListener(() => ItemSelected(modelName));
        item.transform.SetParent(gridObjectCollection.transform, false);

        gridObjectCollection.UpdateCollection();

        scrollingObjectCollection.UpdateContent();

    }

    private void ItemSelected(string name)
    {
        ModelSelected?.Invoke(name);
    }

    public void ClearContent()
    {
        while (gridObjectCollection.transform.childCount > 0)
        {
            DestroyImmediate(gridObjectCollection.transform.GetChild(0).gameObject);
        }

        scrollingObjectCollection.UpdateContent();
    }

    private async void Refresh()
    {
        var tmp = await octoprintConnector.FetchAllFilesAsync();

        if (tmp.files.Count > 0)
        {
            var files = tmp.files.OrderBy(x => x.name);

            ClearContent();
            foreach (var file in files)
            {
                if (file.display.EndsWith(".stl"))
                {
                    var listItem = Instantiate(ListItemPrefab);
                    var bch = listItem.GetComponent<ButtonConfigHelper>();
                    bch.MainLabelText = file.name;
                    AddContent(listItem.GetComponent<Interactable>());
                }
            }
        }
    }


}
