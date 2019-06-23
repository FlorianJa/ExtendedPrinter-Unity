using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class RotateVideoFiles : MonoBehaviour
{
    public VideoClip[] Videos;
    int counter = 1;
    public void Start()
    {
        GetComponent<VideoPlayer>().loopPointReached += RotateVideoFiles_loopPointReached;
    }

    private void RotateVideoFiles_loopPointReached(VideoPlayer source)
    {
        GetComponent<VideoPlayer>().clip = Videos[(counter++) % 2];
        GetComponent<VideoPlayer>().Play();
    }
}
