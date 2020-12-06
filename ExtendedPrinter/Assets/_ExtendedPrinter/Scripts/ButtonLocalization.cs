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
        if(stringRefButton == null)
        {
            throw new ArgumentNullException(nameof(stringRefButton));
        }
        if (textMeshButton == null)
        {
            throw new ArgumentNullException(nameof(textMeshButton));
        }
        if (stringRefLabel == null)
        {
            throw new ArgumentNullException(nameof(stringRefLabel));
        }
        if (textMeshLabel == null)
        {
            throw new ArgumentNullException(nameof(textMeshLabel));
        }

        stringRefButton.StringChanged += UpdateString(textMeshButton);
        stringRefLabel.StringChanged += UpdateString(textMeshLabel);
    }

    void OnDisable()
    {
        stringRefButton.StringChanged -= UpdateString(textMeshButton);
        stringRefLabel.StringChanged -= UpdateString(textMeshLabel);
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
