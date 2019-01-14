// Basic Libraries
using System;
using System.Text;
// Unity Libraries
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
// Connecting to server libraries
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class PlayLiveStream : MonoBehaviour
{
    // State Variables
    private bool connection_success = false;
    private bool thread_started = false;
    private bool server_error = false;
    private bool connector_working = false;
    private bool _thread_running = false;
    private bool is_ready = false;

    // Connection Variables
    private string ipaddr = Data.IP_Address;
    private int port_num = Data.Port_Number;
    private UdpClient udpclient;
    private IPEndPoint ep;
    private byte[] received_bytes;
    private int num_packets = 0;
    private int received_bytes_ptr = 0;
    private Thread _thread;
    private Thread connector_thread;

    // Unity Game Objects
    private Renderer sphere_texture;
    private Text stream_info;
    private Text stream_status;

    // Rendering Variables
    private int img_num = 0;
    private const int image_size = 5000000;
    private byte[] image = new byte[image_size];
    private byte[][] ready_image = new byte[2][];
    private Mutex ready_mtx = new Mutex();
    private Mutex img_num_mtx = new Mutex();

    // Use this for initialization
    void Start()
    {
        // Grabbing Important gameobjects
        sphere_texture = GameObject.Find("Sphere").GetComponent<Renderer>();

        // Checking ip and port
        if (ipaddr == "" && port_num == 0)
        {
            ipaddr = "127.0.0.1";
            port_num = 11000;
        }

        // Grabbing UI Text
        stream_info = GameObject.Find("Stream Info").GetComponent<Text>();
        stream_info.text = "Live Stream From: " + ipaddr + ":" + port_num;
        stream_status = GameObject.Find("Stream Status").GetComponent<Text>();
        stream_status.text = "Status: Connecting ...";

        // Connecting to server
        connector_working = true;
        connector_thread = new Thread(ConnectToStreamService);
        connector_thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // Changing scenes
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("0 - UI");
            UnityEngine.XR.XRSettings.enabled = false;
            OnApplicationQuit();
        }
        // Rendering image to sky sphere
        if (is_ready)
        {
            Texture2D tex = new Texture2D(2, 2);

            // Checking if image has been successfully loaded before rendering onto the sky sphere
            img_num_mtx.WaitOne();
            // Checking again before proceeding
            if (is_ready)
            {
                try
                {
                    if (tex.LoadImage(ready_image[img_num]) && (tex.width > 8) && (tex.height > 8))
                        sphere_texture.material.mainTexture = tex;
                    else if (tex.LoadImage(ready_image[(img_num + 1) % 2]) && (tex.width > 8) && (tex.height > 8))
                        sphere_texture.material.mainTexture = tex;
                }
                catch { };
                // Working on next image
                img_num = (img_num + 1) % 2;
                img_num_mtx.ReleaseMutex();

                ready_mtx.WaitOne();
                is_ready = false;
                ready_mtx.ReleaseMutex();
            }
        }
        // Starting a thread for processing the byte stream sent across by the server
        if (connection_success && !thread_started)
        {
            //Debug.Log("Successfully connected to: " + ep.ToString());
            stream_status.text = "Status: Connected!";
            thread_started = true;
            // Receiving byte stream
            _thread_running = true;
            _thread = new Thread(ProcessMJPEGByteStream);
            _thread.Start();
        }
        // Server error
        if (connection_success && server_error)
        {
            stream_status.text = "Status: Disconnected!";
            //UnityEngine.Debug.Log("Starting to clean up...");

            // Cleaning up
            connection_success = false;
            thread_started = false;
            server_error = false;
            is_ready = false;
            // Disconnecting
            if (connection_success)
                udpclient.Close();
            // Shutting down thread(s)
            if (_thread_running)
            {
                _thread_running = false;
                _thread.Join();
            }
            if (connector_working)
            {
                connector_working = false;
                connector_thread.Join();
            }
            // Restarting
            connector_working = true;
            connector_thread = new Thread(ConnectToStreamService);
            connector_thread.Start();
            //UnityEngine.Debug.Log("Finished cleaning up!");
        }
    }

    // Function for connecting to a UDP stream server
    void ConnectToStreamService()
    {
        while (!connection_success && connector_working)
        {
            udpclient = new UdpClient();
            ep = new IPEndPoint(IPAddress.Parse(ipaddr), port_num);
            try
            {
                udpclient.Connect(ep);
                udpclient.Client.ReceiveTimeout = 1000;
                byte[] datagram = Encoding.ASCII.GetBytes("Send me stuff!");
                udpclient.Send(datagram, datagram.Length);
                var data = Encoding.ASCII.GetString(udpclient.Receive(ref ep));
                if (data == "Okay here you go!")
                {
                    connection_success = true;
                    break;
                }
            }
            catch
            {
                udpclient.Close();
            }
        }
        connector_working = false;
    }

    // Function for processing jpeg byte streams into an images that are to
    // be used to rendered onto the sky sphere.
    void ProcessMJPEGByteStream()
    {
        while (_thread_running)
        {
            // Looking for JPEG headers
            int found_jpg_header = 0;
            byte b;
            while (true)
            {
                if (!server_error)
                    b = GetStreamByte();
                else
                    return;
                if (b == 0xff)
                    found_jpg_header = 1;
                else if (b == 0xd8 && found_jpg_header == 1)
                    break;
                else
                    found_jpg_header = 0;
            }

            // Start working on making a new image
            // Initialising image
            int byte_count = 0;
            int byte_count_chk = 0;
            image = new byte[image_size];
            image[byte_count++] = 0xff;
            image[byte_count++] = 0xd8;

            // Building image
            int end_flag = 0;
            while (byte_count < image_size)
            {
                byte tmp_b;
                if (!server_error)
                    tmp_b = GetStreamByte();
                else
                    return;
                try
                {
                    image[byte_count++] = tmp_b;
                }
                catch
                {
                    //UnityEngine.Debug.Log("byte_count++ error!: "+byte_count);
                    byte_count_chk = 1;
                    break;
                }
                if (tmp_b == 0xff && end_flag == 0)
                    end_flag++;
                else if (tmp_b == 0xd9 && end_flag == 1)
                    break;
                else
                    end_flag = 0;
            }
            if (byte_count_chk == 1) continue;
            // Completing image
            if (byte_count == image_size - 1)
            {
                image[image_size - 2] = 0xff;
                image[image_size - 1] = 0xd9;
            }
            else Array.Resize<byte>(ref image, byte_count);

            // Storing completed image into another byte array
            img_num_mtx.WaitOne();
            ready_image[img_num] = new byte[byte_count];
            Array.Copy(image, ready_image[img_num], byte_count);
            img_num_mtx.ReleaseMutex();

            // Signalling image is ready to be used
            ready_mtx.WaitOne();
            is_ready = true;
            ready_mtx.ReleaseMutex();

            // Letting the Update thread catch up
            System.Threading.Thread.Sleep(50);

            // Resetting
            continue;
        }
    }

    // Function that returns a byte of the stored byte stream
    byte GetStreamByte()
    {
        // Fetching bytes to store in received_bytes array
        if (received_bytes == null || received_bytes_ptr >= received_bytes.Length)
        {
            try
            {
                received_bytes = udpclient.Receive(ref ep);
            }
            catch
            {
                //UnityEngine.Debug.Log("Disconnected from server: " + ep.ToString());
                server_error = true;
                try
                {
                    ready_mtx.ReleaseMutex();
                }
                catch { };
                try
                {
                    img_num_mtx.ReleaseMutex();
                }
                catch { };
                return 0x00;
            }
            received_bytes_ptr = 0;
            num_packets++;
        }
        try
        {
            return received_bytes[received_bytes_ptr++];
        }
        catch
        {
            //UnityEngine.Debug.Log("GetStreamBye error = " + received_bytes_ptr);
            received_bytes_ptr = 0;
            return received_bytes[received_bytes_ptr++];
        }
    }

    void OnApplicationQuit()
    {
        //Debug.Log("Disconnecting from server.");
        //Debug.Log("Cleaning up...");
        // Cleaning up
        connection_success = false;
        thread_started = false;
        server_error = false;
        is_ready = false;
        // Notifying server
        var datagram = Encoding.ASCII.GetBytes("Disconnecting from you.");
        try
        {
            udpclient.Send(datagram, datagram.Length);
        }
        catch { };
        // Disconnecting
        if (connection_success)
            udpclient.Close();
        // Shutting down thread(s)
        if (_thread_running)
        {
            _thread_running = false;
            _thread.Join();
        }
        if (connector_working)
        {
            connector_working = false;
            connector_thread.Join();
        }
        //Debug.Log("Finished cleaning up!");
    }
}
