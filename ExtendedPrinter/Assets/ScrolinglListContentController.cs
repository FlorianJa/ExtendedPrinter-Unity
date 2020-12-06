using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrolinglListContentController : MonoBehaviour
{
    [SerializeField]
    private ScrollingObjectCollection scrollingObjectCollection;

    [SerializeField]
    private GridObjectCollection gridObjectCollection;

    private GameObject ItemPrefab;

    public void AddContent(GameObject item)
    {

        item.transform.SetParent(gridObjectCollection.transform, false);

        gridObjectCollection.UpdateCollection();

        scrollingObjectCollection.UpdateContent();

    }
}
