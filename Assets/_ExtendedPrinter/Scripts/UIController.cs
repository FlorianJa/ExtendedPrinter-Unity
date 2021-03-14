using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets._ExtendedPrinter.Scripts.ModelLoader;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject ModelSelectionUI;

    [SerializeField]
    private GameObject SlicingUI;

    [SerializeField]
    private GameObject PreviewUI;

    [SerializeField]
    private SlicedModelLoader SlicedModelLoader;

    [SerializeField]
    private GameObject offsetSetter;
    private void Awake()
    {
        if(ModelSelectionUI == null)
        {
            throw new NullReferenceException("ModellSelectionUI Gameobject is not set.");
        }
        
        if(SlicingUI == null)
        {
            throw new NullReferenceException("SlicingUI Gameobject is not set.");
        }
        
        if(PreviewUI == null)
        {
            throw new NullReferenceException("PreviewUI Gameobject is not set.");
        }

        if (SlicedModelLoader == null)
        {
            throw new NullReferenceException("SlicedModelLoader is not set.");
        }

        //ModelSelectionUI.GetComponent<ScrolinglListContentController>().ModelSelected.AddListener(onModelSelected);
        //SlicedModelLoader.ModelLoaded.AddListener(onModelLoaded);
    }

    private void onModelLoaded()
    {
        PreviewUI.SetActive(true);
    }

    private void OnDisable()
    {
        //ModelSelectionUI.GetComponent<ScrolinglListContentController>().ModelSelected.RemoveListener(onModelSelected);
    }

    public void onModelSelected(string modelName)
    {
        SlicingUI.SetActive(true);
    }


    private void Update()
    {
        this.transform.localPosition = offsetSetter.transform.localPosition;
    }
}
