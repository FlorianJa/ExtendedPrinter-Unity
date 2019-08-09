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

    public VideoClip videoClipUnload;
    public VideoClip videoClipLoad;

    public GameObject endbutton;

    public OctoPrintConnector OctoPrintConnector;
    public void Start()
    {
        clips = new Dictionary<int, VideoClip>();
        clips.Add(0, videoClipUnload);
        clips.Add(5, videoClipLoad);
    }

    public void playVideo(int index)
    {

        videoMeshRenderer.enabled = true;
        videoPlayer.clip = clips[index];
        videoPlayer.Play();
    }

    public void SetLabelText(int index)
    {
        if(index < Texts.Length)
        {
            label.text = Texts[index];
            if (clips.ContainsKey(index))
            {
                playVideo(index);

            }
            else
            {
                videoMeshRenderer.enabled = false;
            }
        }
        else
        {
            endbutton.active = true;
            gameObject.active = false;
        }

        if(index == StepForextrusion)
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
