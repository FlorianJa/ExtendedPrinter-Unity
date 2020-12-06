using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoplateListTest : MonoBehaviour
{
    public ScrolinglListContentController scrollView;

    public GameObject ListItemPrefab;

   [ContextMenu("Test")]
    void Test()
    {
        var listItem = Instantiate(ListItemPrefab);

        scrollView.AddContent(listItem);
    }

    
}
