using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using System;

public class ButtonLocalization : MonoBehaviour
{
    [SerializeField]
    private LocalizedString stringRefButton;
    [SerializeField]
    private LocalizedString stringRefLabel;

    [SerializeField]
    private TextMeshPro textMeshButton;
    [SerializeField]
    private TextMeshPro textMeshLabel;


    void OnEnable()
    {
        if(stringRefButton != null && textMeshButton != null)
        {
            stringRefButton.StringChanged += UpdateString(textMeshButton);
        }

        if (stringRefLabel != null && textMeshLabel != null)
        {
            stringRefLabel.StringChanged += UpdateString(textMeshLabel);
        }

    }

    void OnDisable()
    {
        if (stringRefButton != null && textMeshButton != null)
        {
            stringRefButton.StringChanged -= UpdateString(textMeshButton);
        }
        if (stringRefLabel != null && textMeshLabel != null)
        {
            stringRefLabel.StringChanged -= UpdateString(textMeshLabel);
        }
    }

    LocalizedString.ChangeHandler UpdateString(TextMeshPro textMesh)
    {
        Action<string> action = (translatedValue) =>
        {
            var _textMesh = textMesh;
            _textMesh.text = translatedValue;
        };

        return new LocalizedString.ChangeHandler(action);
    }
}
