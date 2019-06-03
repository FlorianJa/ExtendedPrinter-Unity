using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractionReceiver : InteractionReceiver
{
    public MeshCreator MeshCreator;
    public string URL;

    protected override void InputDown(GameObject targetObject, InputEventData eventData)
    {
        if (!eventData.used)
        {
            eventData.Use();
            MeshCreator.LoadObject(URL);
        }
    }
}
