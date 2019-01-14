using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct ButtonInfo
{
    public string ipaddr;
    public int portnum;
    public string url;
    public override string ToString()
    {
        return ipaddr + ":" + portnum.ToString() + " - " + url;
    }
}

public class MakeButtons : MonoBehaviour {
    public GameObject buttonPrefab;
    public RectTransform parentPanel;

    private 

    // Use this for initialization
    void Start () {
        ButtonInfo[] bilist = MakeButtonInfoArray();
        for (int i = 0; i < bilist.Length; i++)
        {
            GameObject gO = Instantiate(buttonPrefab);
            gO.transform.SetParent(parentPanel, false);
            gO.transform.localScale = new Vector3(1, 1, 1);
            gO.name = "Stream Service Button " + i;
            gO.GetComponentInChildren<Text>().text = "Stream Service " + i;
            var btn = GameObject.Find(gO.name).GetComponent<Button>();
            //Debug.Log(btn.ToString());
            int i2 = i;
            btn.onClick.AddListener(() => ButtonClickEvent(bilist[i2]));
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    // Button methods
    ButtonInfo[] MakeButtonInfoArray()
    {
        string[] ip = { "127.0.0.1", "10.1.2.2", "10.1.3.180", "", "", "" };
        int[] portnum = { 11000, 11000, 11000, 0, 0, 0 };
        string[] url = { "", "", "", "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4", "http://softwareservices.ptgrey.com/LadybugResource/VR360/LD5Plus/Night%20Drive%20in%20LA/LD5%20Night%20Drive%20in%20LA-%20Short%204k24fps.mp4", "http://softwareservices.ptgrey.com/LadybugResource/VR360/LD5Plus/Dirty%20Daddies/DD-Short-Clip.mp4" };
        ButtonInfo[] ret = new ButtonInfo[ip.Length];
        for (int i=0; i<ip.Length; i++)
        {
            ret[i] = new ButtonInfo();
            ret[i].ipaddr = ip[i];
            ret[i].portnum = portnum[i];
            ret[i].url = url[i];
        }
        return ret;
    }

    void ButtonClickEvent(ButtonInfo bi)
    {
        Data.IP_Address = bi.ipaddr;
        Data.Port_Number = bi.portnum;
        Data.Video_URL = bi.url;
        //Debug.Log("Button pressed!");
        if (bi.ipaddr == "" && bi.url != "")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("2 - URL");
        }
        else if (bi.ipaddr != "" && bi.url == "")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("1 - LiveStream");
        }
    }
}
