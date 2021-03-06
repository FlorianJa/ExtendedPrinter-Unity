﻿using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    string[,,] FilamentLenght = new string[4, 5, 21]{ // scale [100,150,200,250], layer [007.01.015.02.025], layerheight [0,05,1,15,2,25,3,35,4,45,5,55,6,65,7,75,8,85,9,95,1]
		{
            {"3,13 m","3,65 m","3,85 m","4,03 m","4,19 m","4,36 m","4,52 m","4,68 m","4,84 m","5 m","5,16 m","5,32 m","5,48 m","5,64 m","5,8 m","5,96 m","6,12 m","6,27 m","6,43 m","6,59 m","6,63 m"},
            {"3,04 m","3,59 m","3,79 m","3,96 m","4,13 m","4,3 m","4,46 m","4,62 m","4,78 m","4,95 m","5,11 m","5,27 m","5,43 m","5,59 m","5,75 m","5,91 m","6,07 m","6,23 m","6,4 m","6,56 m","6,61 m"},
            {"3,02 m","3,51 m","3,75 m","3,9 m","4,08 m","4,24 m","4,41 m","4,56 m","4,72 m","4,88 m","5,03 m","5,19 m","5,33 m","5,49 m","5,65 m","5,8 m","5,95 m","6,11 m","6,27 m","6,42 m","6,56 m" },
            {"3,2 m","3,74 m","3,92 m","4,09 m","4,24 m","4,39 m","4,54 m","4,69 m","4,84 m","4,99 m","5,14 m","5,29 m","5,44 m","5,59 m","5,74 m","5,89 m","6,03 m","6,18 m","6,33 m","6,45 m","6,58 m"},
            {"3,52 m","4,08 m","4,23 m","4,37 m","4,64 m","4,64 m","4,89 m","4,89 m","5,02 m","5,14 m","5,27 m","5,4 m","5,52 m","5,65 m","5,78 m","6,03 m","6,03 m","6,15 m","6,28 m","6,5 m","6,5 m"}
        },
        {
            {"6,02 m","6,74 m","7,36 m","7,8 m","8,15 m","8,5 m","8,79 m","9,1 m","9,41 m","9,7 m","10,02 m","10,28 m","10,62 m","10,92 m","11,2 m","11,5 m","11,81 m","12,11 m","12,44 m","12,75 m","13,29 m"},
            {"5,9 m","6,7 m","7,32 m","7,76 m","8,11 m","8,47 m","8,77 m","9,05 m","9,35 m","9,68 m","10,01 m","10,28 m","10,58 m","10,9 m","11,19 m","11,51 m","11,8 m","12,11 m","12,44 m","12,74 m","13,29 m"},
            {"5,88 m","6,84 m","7,3 m","7,76 m","8,1 m","8,44 m","8,77 m","9,11 m","9,44 m","9,76 m","10,09 m","10,4 m","10,73 m","11,05 m","11,37 m","11,7 m","12,02 m","12,34 m","12,66 m","12,98 m","13,23 m" },
            {"6,14 m","7,07 m","7,52 m","7,92 m","8,26 m","8,58 m","8,9 m","9,22 m","9,53 m","9,85 m","10,14 m","10,46 m","10,77 m","11,07 m","11,38 m","11,68 m","11,99 m","12,3 m","12,6 m","12,91 m","13,16 m"},
            {"6,56 m","7,44 m","7,97 m","8,31 m","8,63 m","8,95 m","9,26 m","9,48 m","9,71 m","9,96 m","10,2 m","10,5 m","10,74 m","10,99 m","11,26 m","11,51 m","11,78 m","12,06 m","12,31 m","12,58 m","13,06 m"}
        },
        {
            {"8,17 m","9,6 m","10,51 m","11,21 m","11,84 m","12,68 m","13,24 m","13,86 m","14,49 m","15,14 m","15,72 m","16,37 m","16,98 m","17,62 m","18,26 m","18,85 m","19,45 m","20,07 m","20,72 m","21,35 m","22,28m"},
            {"7,93 m","9,52 m","10,44 m","11,11 m","11,78 m","12,57 m","13,12 m","13,78 m","14,37 m","15,04 m","15,65 m","16,28 m","16,91 m","17,52 m","18,21 m","18,8 m","19,41 m","20,06 m","20,68 m","21,27 m","22,23 m"},
            {"7,95 m","9,5 m","10,49 m","11,08 m","11,84 m","12,41 m","13,06 m","13,73 m","14,32 m","14,99 m","15,59 m","16,18 m","16,83 m","17,48 m","18,1 m","18,73 m","19,31 m","19,92 m","20,57 m","21,21 m","22,15 m"},
            {"8,41 m","9,91 m","10,85 m","11,43 m","12,19 m","12,67 m","13,39 m","14 m","14,6 m","15,18 m","15,76 m","16,43 m","17,03 m","17,58 m","18,17 m","18,74 m","19,34 m","19,96 m","20,54 m","21,14 m","22,07 m"},
            {"9,14 m","11,04 m","11,89 m","12,48 m","13,06 m","13,6 m","14,22 m","14,74 m","15,26 m","15,78 m","16,34 m","16,85 m","17,38 m","17,95 m","18,45 m","18,99 m","19,54 m","20,06 m","20,6 m","21,12 m","21,97 m"}
        },
        {
            {"11,53 m","13,6 m","15,2 m","16,34 m","17,59 m","18,55 m","19,62 m","20,74 m","21,77 m","22,87 m","23,91 m","24,97 m","26,01 m","27,03 m","28,07 m","29,16 m","30,17 m","31,24 m","32,28 m","33,33 m","34,87 m"},
            {"11,18 m","13,56 m","15,12 m","16,24 m","17,45 m","18,52 m","19,59 m","20,71 m","21,68 m","22,79 m","23,86 m","24,93 m","25,91 m","26,99 m","28,03 m","29,09 m","30,15 m","31,19 m","32,26 m","33,29 m","34,84 m"},
            {"11,25 m","13,61 m","15,12 m","16,16 m","17,33 m","18,52 m","19,59 m","20,63 m","21,65 m","22,74 m","23,81 m","24,81 m","25,92 m","26,89 m","27,95 m","29,01 m","30,01 m","31,06 m","32,13 m","33,17 m","34,69 m"},
            {"12,05 m","14,4 m","15,9 m","16,88 m","17,98 m","19,14 m","20,19 m","21,18 m","22,16 m","23,15 m","24,16 m","25,19 m","26,12 m","27,13 m","28,14 m","29,1 m","30,1 m","31,12 m","32,1 m","33,11 m","34,6 m"},
            {"12,92 m","15,92 m","17,29 m","18,33 m","19,3 m","20,28 m","21,21 m","22,17 m","23,08 m","24,01 m","24,9 m","25,8 m","26,74 m","27,67 m","28,53 m","29,42 m","30,35 m","31,26 m","32,19 m","33,08 m","34,48 m"}

        }
        };

    string[,,] FilamentWeight = new string[4, 5, 21]{ // scale [100,150,200,250], layer [007.01.015.02.025], layerheight [0,05,1,15,2,25,3,35,4,45,5,55,6,65,7,75,8,85,9,95,1]
		{
            {"9,32 g","10,89 g","11,48 g","12,01 g","12,51 g","12,99 g","13,47 g","13,95 g","14,43 g","14,91 g","15,39 g","15,86 g","16,34 g","16,82 g","17,29 g","17,77 g","18,24 g","18,71 g","19,19 g","19,66 g","19,78 g"},
            {"9,05 g","10,7 g","11,29 g","11,83 g","12,32 g","12,82 g","13,3 g","13,79 g","14,27 g","14,76 g","15,24 g","15,72 g","16,2 g","16,68 g","17,16 g","17,64 g","18,12 g","18,6 g","19,07 g","19,55 g","19,71 g"},
            {"9,01 g","10,46 g","11,17 g","11,64 g","12,16 g","12,64 g","13,16 g","13,61 g","14,07 g","14,56 g","15,02 g","15,47 g","15,89 g","16,39 g","16,84 g","17,3 g","17,76 g","18,22 g","18,7 g","19,13 g","19,57 g"},
            {"9,55 g","11,16 g","11,69 g","12,19 g","12,64 g","13,1 g","13,55 g","14 g","14,45 g","14,89 g","15,34 g","15,78 g","16,22 g","16,67 g","17,11 g","17,56 g","18 g","18,44 g","18,88 g","19,21 g","19,61 g"},
            { "10,49 g","12,17 g","12,62 g","13,05 g","13,83 g","13,83 g","14,59 g","14,59 g","14,97 g","15,34 g","15,73 g","16,1 g","16,47 g","16,85 g","17,23 g","17,97 g","17,97 g","18,34 g","18,72 g","19,38 g","19,38 g"}
        },
        {
            {"17,95 g","20,12 g","21,96 g","23,28 g","24,31 g","25,34 g","26,21 g","27,13 g","28,07 g","28,93 g","29,89 g","30,66 g","31,68 g","32,57 g","33,41 g","34,31 g","35,21 g","36,12 g","37,09 g","38,02 g","39,64 g"},
            {"17,6 g","19,99 g","21,84 g","23,16 g","24,19 g","25,25 g","26,14 g","26,99 g","27,89 g","28,86 g","29,85 g","30,65 g","31,55 g","32,5 g","33,38 g","34,34 g","35,19 g","36,13 g","37,1 g","38 g","39,63 g"},
            {"17,53 g","20,39 g","21,77 g","23,16 g","24,16 g","25,19 g","26,15 g","27,16 g","28,14 g","29,11 g","30,08 g","31,02 g","31,99 g","32,95 g","33,91 g","34,88 g","35,85 g","36,79 g","37,76 g","38,7 g","39,45 g"},
            {"18,31 g","21,09 g","22,43 g","23,63 g","24,63 g","25,6 g","26,54 g","27,49 g","28,41 g","29,37 g","30,26 g","31,19 g","32,11 g","33,02 g","33,94 g","34,84 g","35,76 g","36,68 g","37,59 g","38,5 g","39,24 g"},
            {"19,55 g","22,19 g","23,77 g","24,77 g","25,74 g","26,7 g","27,61 g","28,27 g","28,97 g","29,71 g","30,41 g","31,31 g","32,03 g","32,79 g","33,59 g","34,34 g","35,14 g","35,98 g","36,7 g","37,51 g","38,96 g"}
        },
        {
            {"24,36 g","28,64 g","31,34 g","33,45 g","35,31 g","37,81 g","39,48 g","41,34 g","43,21 g","45,15 g","46,89 g","48,83 g","50,63 g","52,56 g","54,46 g","56,23 g","58,01 g","59,86 g","61,79 g","63,68 g","66,45 g"},
            {"23,65 g","28,4 g","31,13 g","33,13 g","35,15 g","37,49 g","39,12 g","41,1 g","42,87 g","44,86 g","46,68 g","48,56 g","50,44 g","52,26 g","54,32 g","56,08 g","57,88 g","59,82 g","61,68 g","63,43 g","66,31 g"},
            {"23,71 g","28,32 g","31,29 g","33,04 g","35,3 g","37,02 g","38,96 g","40,96 g","42,72 g","44,7 g","46,49 g","48,26 g","50,2 g","52,15 g","53,98 g","55,87 g","57,58 g","59,42 g","61,36 g","63,26 g","66,05 g"},
            {"25,1 g","29,56 g","32,37 g","34,08 g","36,34 g","37,8 g","39,94 g","41,75 g","43,53 g","45,26 g","47,01 g","48,99 g","50,79 g","52,44 g","54,2 g","55,91 g","57,68 g","59,52 g","61,27 g","63,04 g","65,84 g"},
            { "27,27 g","32,92 g","35,46 g","37,22 g","38,95 g","40,56 g","42,4 g","43,97 g","45,53 g","47,07 g","48,75 g","50,25 g","51,83 g","53,54 g","55,01 g","56,64 g","58,26 g","59,83 g","61,45 g","62,99 g","65,53 g"},
        },
        {
            {"34,38 g","40,58 g","45,33 g","48,75 g","52,45 g","55,33 g","58,51 g","61,85 g","64,94 g","68,2 g","71,32 g","74,49 g","77,59 g","80,63 g","83,73 g","86,96 g","89,97 g","93,17 g","96,28 g","99,42 g","104,01 g"},
            {"33,34 g","40,46 g","45,1 g","48,43 g","52,03 g","55,23 g","58,42 g","61,77 g","64,66 g","67,98 g","71,16 g","74,37 g","77,28 g","80,49 g","83,59 g","86,75 g","89,92 g","93,02 g","96,21 g","99,3 g","103,93 g"},
            {"33,56 g","40,6 g","45,11 g","48,21 g","51,68 g","55,24 g","58,42 g","61,52 g","64,57 g","67,82 g","71,01 g","74,01 g","77,3 g","80,2 g","83,37 g","86,53 g","89,51 g","92,63 g","95,83 g","98,94 g","103,47 g"},
            {"35,93 g","42,94 g","47,43 g","50,34 g","53,62 g","57,09 g","60,22 g","63,16 g","66,1 g","69,04 g","72,07 g","75,13 g","77,91 g","80,91 g","83,94 g","86,78 g","89,77 g","92,81 g","95,73 g","98,74 g","103,18 g"},
            {"38,52 g","47,47 g","51,57 g","54,68 g","57,58 g","60,48 g","63,25 g","66,11 g","68,83 g","71,61 g","74,27 g","76,94 g","79,74 g","82,52 g","85,1 g","87,74 g","90,52 g","93,24 g","96,02 g","98,65 g","102,84 g"}
        }
        };



    private int scaleIndex = 0, layerIndex = 0, infillIndex = 0;

    [SerializeField]
    private TextMeshPro Time;
    [SerializeField]
    private TextMeshPro Filament;
    [SerializeField]
    private TextMeshPro Weight;

    public void OnScaleValueChanged(SliderEventData data)
    {
        scaleIndex = (int)Math.Round(data.NewValue * 3f);
        //Debug.Log(scaleIndex);
        //Debug.Log(PrintTimes[scaleIndex, layerIndex, infillIndex]);

        Time.text = PrintTimes[scaleIndex, layerIndex, infillIndex];
        Filament.text = FilamentLenght[scaleIndex, layerIndex, infillIndex];
        Weight.text = FilamentWeight[scaleIndex, layerIndex, infillIndex];

    }

    public void OnLayerHeightValueChanged(SliderEventData data)
    {
        layerIndex = (int)Math.Round(data.NewValue * 4f);
        //      Debug.Log(layerIndex);
        //Debug.Log(PrintTimes[scaleIndex, layerIndex, infillIndex]);
        Time.text = PrintTimes[scaleIndex, layerIndex, infillIndex];
        Filament.text = FilamentLenght[scaleIndex, layerIndex, infillIndex];
        Weight.text = FilamentWeight[scaleIndex, layerIndex, infillIndex];
    }

    public void OnInfillValueChanged(SliderEventData data)
    {
        infillIndex = (int)Math.Round(data.NewValue * 20f);
        //      Debug.Log(infillIndex);
        //Debug.Log(PrintTimes[scaleIndex, layerIndex, infillIndex]);
        Time.text = PrintTimes[scaleIndex, layerIndex, infillIndex];
        Filament.text = FilamentLenght[scaleIndex, layerIndex, infillIndex];
        Weight.text = FilamentWeight[scaleIndex, layerIndex, infillIndex];

    }
}
