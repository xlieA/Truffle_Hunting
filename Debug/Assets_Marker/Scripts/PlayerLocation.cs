using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLocation : MonoBehaviour
{
    public Text GPSStatus;
    public Text latitudeText;
    public Text longitudeText;
    public Text altitudeText;

    public double latitudeValue;
    public double longitudeValue;
    public double altitudeValue;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GPSLoc());
    }

    IEnumerator GPSLoc()
    {
        // Check if user has location service enabled
        if(!Input.location.isEnabledByUser)
        {
            GPSStatus.text = "GPS location not enabled";
            yield break;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Initialization failed
        if(maxWait < 1)
        {
            GPSStatus.text = "Time out";
            yield break;
        }

        // Connection failed
        if(Input.location.status == LocationServiceStatus.Failed)
        {
            GPSStatus.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            // Access granted
            GPSStatus.text = "Running";

            // Update GPS location every second
            InvokeRepeating("UpdateGPSData", 0.5f, 1f);
        }
    }

    private void UpdateGPSData()
    {
        if(Input.location.status == LocationServiceStatus.Running)
        {
            // Access granted
            GPSStatus.text = "Running";

            latitudeText.text = Input.location.lastData.latitude.ToString();
            longitudeText.text = Input.location.lastData.longitude.ToString();
            altitudeText.text = Input.location.lastData.altitude.ToString();

            latitudeValue = Input.location.lastData.latitude;
            longitudeValue = Input.location.lastData.longitude;
            altitudeValue = Input.location.lastData.altitude;
        }
        else
        {
            // Service is stopped
            GPSStatus.text = "Stopped";
        }
    }
}
