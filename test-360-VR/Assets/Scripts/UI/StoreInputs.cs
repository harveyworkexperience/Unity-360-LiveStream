using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreInputs : MonoBehaviour {
    public InputField url_field;
    public InputField ip_field;
    public InputField port_field;
    public string scenename;

    public void StoreInputsOnButtonPress()
    {
        Data.IP_Address = ip_field.text;
        Data.Video_URL = url_field.text;
        System.Int32.TryParse(port_field.text, out Data.Port_Number);
        //Debug.Log("Button pressed!");
        UnityEngine.SceneManagement.SceneManager.LoadScene(scenename);
    }
}
