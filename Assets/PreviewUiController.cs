using Assets._ExtendedPrinter.Scripts.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewUiController : MonoBehaviour
{
    private string ModelName;

    public StringEvent BackToSlicing;

    public void OnBackToSlicing()
    {
        BackToSlicing.Invoke(ModelName);
    }

    public void SetModelName(string name)
    {
        ModelName = name;
    }
}
