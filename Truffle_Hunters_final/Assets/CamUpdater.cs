
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CamUpdater : TextureUpdater
{

        public int Width { get { return webCamTexture.width;  } }
        public int Height { get { return webCamTexture.height;  } }

        [SerializeField] protected WebCamTexture webCamTexture;
        [SerializeField] protected int width = 512, height = 512;


    
   void Start () 
   {
        WebCamDevice userCameraDevice = WebCamTexture.devices[0];
        webCamTexture = new WebCamTexture(userCameraDevice.name, width, height);
        webCamTexture.Play();
    }

    void Update ()
    {
        if(webCamTexture != null && webCamTexture.didUpdateThisFrame)
        {
            textureUpdateEvent.Invoke(webCamTexture);
        }
    }

   
    protected void OnDestroy()
    {
    }
    public void CloseScene()
    {
        GameObject.Find("Loader").GetComponent<MapCaptureSceneTrans>().ToCapture = false;
        GameObject.Find("Loader").GetComponent<MapCaptureSceneTrans>().ToMap = true;
    }
}
