using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PlayURLVideo : MonoBehaviour
{
    private VideoPlayer vp;
    private Text stream_info;

    // Use this for initialization
    void Start()
    {
        vp = GameObject.Find("Sphere").GetComponent<VideoPlayer>();
        if (Data.Video_URL != "")
            vp.url = Data.Video_URL;
        else
            vp.url = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4";
        stream_info = GameObject.Find("Stream Info").GetComponent<Text>();
        stream_info.text = "Stream: " + vp.clip;
    }

    // Update is called once per frame
    void Update()
    {
        // Changing scenes
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("0 - UI");
            UnityEngine.XR.XRSettings.enabled = false;
        }
    }
}