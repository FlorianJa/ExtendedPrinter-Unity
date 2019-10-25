using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.XR.WSA.WebCam;
using System.IO;
using System;
using TMPro;
using UnityEngine.Events;

public class VideoRecord : MonoBehaviour
{
    private VideoCapture _videoCapture = null;
    private Resolution _cameraResolution;
    private float _cameraFramerate;
    private DateTime _startRecordingTime;
    private float m_stopRecordingTimer = float.MaxValue;

    private string _mediaPath;
    private string _fileNameFullPath;

    private string _fileName;

    public float MaxRecordingTime = 300.0f;

    public VideoFinishedEvent OnVideoFinished;
    public UnityEvent OnVideoRecordingStarted;



    // Use this for initialization
    void Start()
    {
        _cameraResolution = VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        _cameraFramerate = VideoCapture.GetSupportedFrameRatesForResolution(_cameraResolution).OrderByDescending((fps) => fps).First();
        //_cameraResolution = new Resolution() { width = 896, height = 504 };
        //try
        //{
        //    _cameraFramerate = VideoCapture.GetSupportedFrameRatesForResolution(_cameraResolution).OrderByDescending((fps) => fps).First();
        //}
        //catch (Exception)
        //{
        //    _cameraFramerate = 30f;
        //}

        _mediaPath = Application.persistentDataPath;/* Path.Combine(Application.persistentDataPath, MediaPath);*/
    }

    void Update()
    {
        if (_videoCapture != null)
        {
            if (_videoCapture.IsRecording)
            {
                if (Time.time > m_stopRecordingTimer)
                {
                    _videoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
                }
            }
        }
    }


    public void OnTapped()
    {
        if (_videoCapture != null)
        {
            if (_videoCapture.IsRecording)
            {
                StopRecording();
            }
        }
        else
        {
            StartVideoCapture();
        }
    }

    public void StopRecording()
    {
        if (_videoCapture != null)
        {
            try
            {
                _videoCapture.StopRecordingAsync(OnStoppedRecordingVideo);

            }
            catch (Exception)
            {
                Debug.Log("Error on stopping recording");
            }
        }
    }

    private void StartVideoCapture()
    {
        VideoCapture.CreateAsync(true, OnVideoCaptureCreated);
    }

    private void OnVideoCaptureCreated(VideoCapture videoCapture)
    {
        if (videoCapture != null)
        {
            _videoCapture = videoCapture;
            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 1.0f;
            cameraParameters.frameRate = _cameraFramerate;
            cameraParameters.cameraResolutionWidth = _cameraResolution.width;
            cameraParameters.cameraResolutionHeight = _cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            _startRecordingTime = DateTime.Now;

            if (!Directory.Exists(_mediaPath))
            {
                Directory.CreateDirectory(_mediaPath);
            }


            _videoCapture.StartVideoModeAsync(cameraParameters,
                                               VideoCapture.AudioState.ApplicationAndMicAudio,
                                               OnStartedVideoCaptureMode);
        }
        else
        {
            Debug.LogError("Failed to create VideoCapture Instance!");
            OnVideoFinished?.Invoke("Error");
        }
    }

    void OnStartedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        if (result.success)
        {
            _fileName = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".mp4";
            _fileNameFullPath = Path.Combine(_mediaPath, _fileName);
            _videoCapture.StartRecordingAsync(_fileNameFullPath, OnStartedRecordingVideo);
        }
        else
        {
            OnVideoFinished?.Invoke("Error");
        }
    }

    void OnStoppedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        _videoCapture = null;

        OnVideoFinished?.Invoke(_fileName);
    }

    void OnStartedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        m_stopRecordingTimer = Time.time + MaxRecordingTime;
        OnVideoRecordingStarted?.Invoke();
    }

    void OnStoppedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        _videoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
    }
}


[System.Serializable]
public class VideoFinishedEvent : UnityEvent<string>
{
}