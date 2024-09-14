using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using System;
using System.Collections;
public class CaptureARScene : MonoBehaviour
{
    [SerializeField] public GameObject canvas ;
 
    public void Screenshot()
    {
        canvas.SetActive(false);
        string fileName = GenerateFileName("TruffleHunters", "png");
        ScreenCapture.CaptureScreenshot(fileName);

        StartCoroutine(WaitForScreenshot(fileName));
    }

    IEnumerator WaitForScreenshot(string fileName)
    {
        yield return new WaitForSeconds(1.0f);

        string screenshotPath = System.IO.Path.Combine(Application.persistentDataPath, fileName);

        NativeGallery.SaveImageToGallery(screenshotPath, Application.productName, fileName, (success, path) =>
        {
            if (success)
            {
                Debug.Log("Screenshot saved to gallery: " + path);
            }
            else
            {
                Debug.LogError("Failed to save screenshot to gallery");
            }
        });
        canvas.SetActive(true);
    }

    private string GenerateFileName(string prefix, string extension)
    {
        DateTime now = DateTime.Now;
        string timestamp = $"{now.Year}_{now.Month:D2}_{now.Day:D2}_{now.Hour:D2}_{now.Minute:D2}_{now.Second:D2}";
        return $"{prefix}_{timestamp}.{extension}";
    }

}
