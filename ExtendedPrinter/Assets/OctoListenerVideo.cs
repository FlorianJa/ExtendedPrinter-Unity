using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class OctoListenerVideo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        OctoPrintConnector.Instance.FilamentChangeBegin += StartVideo;
        OctoPrintConnector.Instance.FilamentChangeEnd += StopVideo;
    }

    private void StartVideo(object s, System.EventArgs args)
    {

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GetComponent<MeshRenderer>().enabled = true;
            GetComponentInChildren<VideoPlayer>().clip = GetComponentInChildren<RotateVideoFiles>().Videos[0];
            GetComponentInChildren<VideoPlayer>().Play();
        });
    }
    private void StopVideo(object s, System.EventArgs args)
    {

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponentInChildren<VideoPlayer>().Play();
        });
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
