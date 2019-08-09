using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SetLabel : MonoBehaviour
{
    public string[] Texts;

    public TextMesh label;

    public int StepCounter = 0;

    public int StepForextrusion;

    public Dictionary<int, VideoClip> clips;
    public VideoPlayer videoPlayer;
    public MeshRenderer videoMeshRenderer;

    public OctoPrintConnector OctoPrintConnector;


    public void SetLabelText(int index)
    {
        if(index < Texts.Length)
        {
            label.text = Texts[index];
            if (clips.ContainsKey(index))
            {
                /*videoMeshRenderer.enabled = true;
                videoPlayer.clip = clips[index];
                videoPlayer.Play();*/

            }
            else
            {
                //videoMeshRenderer.enabled = false;
            }
        }

        if(index >= StepForextrusion)
        {
            OctoPrintConnector.FilamentExtrusion();
        }
    }

    public void Next()
    {
        StepCounter++;
        SetLabelText(StepCounter);
    }


}
