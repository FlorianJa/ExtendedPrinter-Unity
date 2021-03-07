using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSliderGroupController : MonoBehaviour
{

	string[,,] PrintTimes = new string[4, 5, 21]{ // scale [100,150,200,250], layer [007.01.015.02.025], layerheight [0,05,1,15,2,25,3,35,4,45,5,55,6,65,7,75,8,85,9,95,1]
		{
			{"3h:5m", "3h:22m", "3h:25m", "3h:28m", "3h:31m", "3h:34m", "3h:38m", "3h:41m", "3h:44m", "3h:47m", "3h:50m", "3h:53m", "3h:57m", "4h:0m", "4h:4m", "4h:7m", "4h:10m", "4h:14m", "4h:18m", "4h:21m", "4h:23m"},
			{"2h:10m", "2h:23m", "2h:25m", "2h:27m", "2h:29m", "2h:32m", "2h:34m", "2h:36m", "2h:38m", "2h:40m", "2h:43m", "2h:45m", "2h:48m", "2h:50m", "2h:53m", "2h:55m", "2h:58m", "3h:0m", "3h:3m", "3h:5m", "3h:7m"},
			{"1h:30m", "1h:37m", "1h:39m", "1h:41m", "1h:43m", "1h:45m", "1h:47m", "1h:49m", "1h:50m", "1h:53m", "1h:55m", "1h:56m", "1h:58m", "1h:59m", "2h:1m", "2h:3m", "2h:4m", "2h:7m", "2h:8m", "2h:9m", "2h:10m"},
			{"1h:12m", "1h:17m", "1h:18m", "1h:20m", "1h:21m", "1h:22m", "1h:23m", "1h:25m", "1h:26m", "1h:27m", "1h:28m", "1h:29m", "1h:31m", "1h:32m", "1h:33m", "1h:34m", "1h:36m", "1h:37m", "1h:38m", "1h:40m", "1h:40m"},
			{"1h:4m", "1h:8m", "1h:10m", "1h:11m", "1h:12m", "1h:12m", "1h:14m", "1h:14m", "1h:15m", "1h:16m", "1h:17m", "1h:18m", "1h:19m", "1h:20m", "1h:21m", "1h:22m", "1h:22m", "1h:23m", "1h:24m", "1h:25m", "1h:25m"}
		},
		{
			{"4h:52m", "5h:30m", "5h:42m", "6h:2m", "6h:20m", "6h:29m", "6h:41m", "6h:55m", "7h:4m", "7h:17m", "7h:29m", "7h:38m", "7h:48m", "8h:2m", "8h:13m", "8h:18m", "8h:30m", "8h:41m", "8h:56m", "8h:59m", "9h:5m"},
			{"3h:27m", "3h:55m", "4h:4m", "4h:19m", "4h:29m", "4h:39m", "4h:49m", "4h:54m", "5h:0m", "5h:12m", "5h:22m", "5h:29m", "5h:35m", "5h:42m", "5h:52m", "5h:57m", "6h:5m", "6h:12m", "6h:22m", "6h:26m", "6h:30m"},
			{"2h:24m", "2h:39m", "2h:47m", "2h:56m", "3h:1m", "3h:8m", "3h:14m", "3h:20m", "3h:25m", "3h:31m", "3h:36m", "3h:41m", "3h:46m", "3h:52m", "3h:57m", "4h:2m", "4h:7m", "4h:12m", "4h:17m", "4h:21m", "4h:25m"},
			{"1h:57m", "2h:8m", "2h:14m", "2h:21m", "2h:24m", "2h:29m", "2h:33m", "2h:38m", "2h:41m", "2h:46m", "2h:50m", "2h:54m", "2h:58m", "3h:2m", "3h:6m", "3h:9m", "3h:13m", "3h:17m", "3h:20m", "3h:24m", "3h:27m"},
			{"1h:46m", "1h:57m", "2h:1m", "2h:6m", "2h:9m", "2h:15m", "2h:19m", "2h:21m", "2h:23m", "2h:27m", "2h:29m", "2h:33m", "2h:35m", "2h:38m", "2h:41m", "2h:43m", "2h:47m", "2h:50m", "2h:51m", "2h:54m", "2h:55m"}
		},
		{
			{"6h:23m", "7h:28m", "7h:51m", "8h:22m", "8h:41m", "9h:19m", "9h:32m", "9h:53m", "10h:17m", "10h:36m", "11h:1m", "11h:27m", "11h:43m", "12h:29m", "12h:46m", "12h:9m", "13h:1m", "13h:22m", "13h:45m", "13h:55m", "14h:2m"},
			{"4h:29m", "5h:20m", "5h:38m", "5h:58m", "6h:14m", "6h:37m", "6h:47m", "7h:3m", "7h:18m", "7h:34m", "7h:52m", "8h:9m", "8h:22m", "8h:36m", "8h:52m", "9h:3m", "9h:20m", "9h:33m", "9h:45m", "9h:54m", "9h:58m"},
			{"3h:7m", "3h:41m", "3h:53m", "4h:6m", "4h:20m", "4h:29m", "4h:39m", "4h:51m", "4h:58m", "5h:11m", "5h:23m", "5h:30m", "5h:43m", "5h:55m", "6h:4m", "6h:14m", "6h:21m", "6h:30m", "6h:41m", "6h:45m", "6h:52m"},
			{"2h:32m", "2h:56m", "3h:5m", "3h:15m", "3h:25m", "3h:31m", "3h:41m", "3h:49m", "3h:57m", "4h:4m", "4h:13m", "4h:22m", "4h:31m", "4h:37m", "4h:44m", "4h:50m", "4h:56m", "5h:4m", "5h:10m", "5h:14m", "5h:18m"},
			{"2h:21m", "2h:43m", "2h:50m", "2h:58m", "3h:4m", "3h:11m", "3h:18m", "3h:24m", "3h:30m", "3h:35m", "3h:43m", "3h:47m", "3h:53m", "3h:59m", "4h:4m", "4h:10m", "4h:16m", "4h:20m", "4h:26m", "4h:27m", "4h:30m"}
		},
		{
			{"8h:20m", "9h:51m", "10h:41m", "11h:17m", "12h:5m", "12h:27m", "13h:3m", "13h:44m", "14h:16m", "14h:54m", "15h:27m", "16h:2m", "16h:34m", "17h:30m", "17h:4m", "18h:38m", "18h:5m", "19h:10m", "19h:38m", "19h:57m", "20h:8m"},
			{"5h:51m", "7h:3m", "7h:38m", "8h:5m", "8h:37m", "8h:57m", "9h:23m", "9h:49m", "10h:15m", "10h:38m", "11h:31m", "11h:3m", "11h:43m", "12h:10m", "12h:31m", "12h:54m", "13h:18m", "13h:41m", "14h:4m", "14h:15m", "14h:27m"},
			{"4h:5m", "4h:50m", "5h:13m", "5h:32m", "5h:52m", "6h:8m", "6h:26m", "6h:43m", "7h:3m", "7h:16m", "7h:34m", "7h:49m", "8h:6m", "8h:19m", "8h:36m", "8h:53m", "9h:8m", "9h:19m", "9h:37m", "9h:44m", "9h:52m"},
			{"3h:21m", "3h:55m", "4h:12m", "4h:27m", "4h:40m", "4h:55m", "5h:8m", "5h:21m", "5h:34m", "5h:45m", "5h:59m", "6h:10m", "6h:20m", "6h:32m", "6h:44m", "6h:56m", "7h:6m", "7h:19m", "7h:29m", "7h:35m", "7h:40m"},
			{"3h:4m", "3h:39m", "3h:52m", "4h:7m", "4h:15m", "4h:26m", "4h:35m", "4h:47m", "4h:54m", "5h:5m", "5h:15m", "5h:23m", "5h:32m", "5h:41m", "5h:50m", "6h:0m", "6h:7m", "6h:17m", "6h:25m", "6h:27m", "6h:33m"}
		}
		};

	private int scaleIndex = 0, layerIndex = 0, infillIndex =0;

    public void OnScaleValueChanged(SliderEventData data)
    {
        scaleIndex = (int)Math.Round(data.NewValue * 3f);
        Debug.Log(scaleIndex);
		Debug.Log(PrintTimes[scaleIndex, layerIndex, infillIndex]);
    }

    public void OnLayerHeightValueChanged(SliderEventData data)
    {
        layerIndex = (int)Math.Round(data.NewValue * 4f);
        Debug.Log(layerIndex);
		Debug.Log(PrintTimes[scaleIndex, layerIndex, infillIndex]);
	}

    public void OnInfillValueChanged(SliderEventData data)
    {
        infillIndex = (int)Math.Round(data.NewValue * 20f);
        Debug.Log(infillIndex);
		Debug.Log(PrintTimes[scaleIndex, layerIndex, infillIndex]);
	}
}
