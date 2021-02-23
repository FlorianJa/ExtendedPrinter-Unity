using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class Localize : MonoBehaviour
{
    [SerializeField]
    private LocalizedString stringRef;
    [SerializeField]
    private TextMeshPro textMesh;


    void OnEnable()
    {
        if (stringRef != null && textMesh != null)
        {
            stringRef.StringChanged += UpdateString(textMesh);
        }
    }

    void OnDisable()
    {
        if (stringRef != null && textMesh != null)
        {
            stringRef.StringChanged -= UpdateString(textMesh);
        }
    }

    LocalizedString.ChangeHandler UpdateString(TextMeshPro textMesh)
    {
        Action<string> action = (translatedValue) =>
        {
            var _textMesh = textMesh;
            _textMesh.text = translatedValue;
        };

        return null;//new LocalizedString.ChangeHandler(action);
    }
}
