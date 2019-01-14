using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using FreeImageAPI;
using System.IO;

public class FreeImageExample : MonoBehaviour {

    //public Renderer sphere_texture;
    //public Text byte_stream_info;
    //public string m_Path;
    //public int state = 0;

    // Use this for initialization
    void Start () {
        //m_Path = Application.dataPath;
        //sphere_texture = GameObject.Find("Sphere").GetComponent<Renderer>();
        //byte_stream_info = GameObject.Find("Byte Stream Info").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        //FIBITMAP dib;
        //Texture2D tex = null;
        //byte[] fileBytes = null;

        //switch (state)
        //{
        //    case 0:
        //        //dib = FreeImage.LoadEx(m_Path + "/Assets/JPEG2000_Images/file1.jp2");
        //        //FreeImage.Save(FREE_IMAGE_FORMAT.FIF_JPEG, dib, m_Path + "/Assets/JPEG_Images/test.jpg", FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYNORMAL);
        //        tex = new Texture2D(2, 2);
        //        fileBytes = System.IO.File.ReadAllBytes(m_Path + "/Assets/JPEG_Images/test.jpg");
        //        tex.LoadImage(fileBytes);
        //        sphere_texture.material.mainTexture = tex;
        //        state = 1;
        //        break;
        //    case 1:
        //        //dib = FreeImage.LoadEx(m_Path + "/Assets/JPEG2000_Images/relax.jp2");
        //        //FreeImage.Save(FREE_IMAGE_FORMAT.FIF_JPEG, dib, m_Path + "/Assets/JPEG_Images/test2.jpg", FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYNORMAL);
        //        tex = new Texture2D(2, 2);
        //        fileBytes = System.IO.File.ReadAllBytes(m_Path + "/Assets/JPEG_Images/test2.jpg");
        //        tex.LoadImage(fileBytes);
        //        sphere_texture.material.mainTexture = tex;
        //        state = 0;
        //        break;
        //}
    }
}
