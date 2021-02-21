using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class LocalizeTextMesh : MonoBehaviour
{
    [SerializeField]
    private LocalizedString stringRef;
    [SerializeField]
    private TextMesh textMesh;


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

    LocalizedString.ChangeHandler UpdateString(TextMesh textMesh)
    {
        Action<string> action = (translatedValue) =>
        {
            var _textMesh = textMesh;
            _textMesh.text = translatedValue;
        };

        return new LocalizedString.ChangeHandler(action);
    }
}
