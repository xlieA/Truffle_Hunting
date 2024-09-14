using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using Mapbox.Examples;
using System.Net;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;
public class ServerTalker : MonoBehaviour
{
    public static ServerTalker Instance { set; get; }
    private string remote_url = "http://16.170.112.13:8000/api/mushroom/location/";
    // public string request_status;
    // bool hasSpawned;

    ILocationProvider _locationProvider;
	ILocationProvider LocationProvider
		{
			get
			{
				if (_locationProvider == null)
				{
					_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
				}

				return _locationProvider;
			}
	}

    Vector2d currentLocation;

    private void Update(){
        // Instance = this;
        // DontDestroyOnLoad(gameObject);
        currentLocation = LocationProvider.CurrentLocation.LatitudeLongitude;
    }

    private void Start()
    {
        currentLocation = LocationProvider.CurrentLocation.LatitudeLongitude;
        StartCoroutine(GetWebData(remote_url));
    }

    IEnumerator GetWebData(string address)
    {
        int maxWait = 5;
        while ((currentLocation[0]==0.0f&&currentLocation[1]==0.0f) && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            yield break;
        }

        UnityWebRequest www = UnityWebRequest.Get(address + currentLocation[0].ToString() + "/" + currentLocation[1].ToString());
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong: " + www.error);
        }
        else
        {
            ProcessServerResponse(www.downloadHandler.text);
           
        }
    }

    void ProcessServerResponse(string rawResponse)
    {
        JSONNode node = JSON.Parse(rawResponse);
        Debug.Log(node["response"].Count);
        for (int i = 0 ; i < node["response"].Count; i++)
        {
            string x = node["response"][i]["latitude"]["$numberDecimal"];
            string y = node["response"][i]["longitude"]["$numberDecimal"];
            string test_location = x + "," + y;
            SpawnOnMap.Instance._locationStrings.Add(test_location);
        }

            // SpawnOnMap.Instance.SpawnObject();
            // hasSpawned = true;
    }    
}