using System;       
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mapbox.Utils;
using Mapbox.Unity.Location;

public class MapCaptureSceneTrans : MonoBehaviour
{
    public Vector2d MushroomPosition;
    public Vector2d PlayerLocation;
	
   	public bool ToMap;
    public bool ToCapture;
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
	
    bool CheckIfClose(Vector2d source, Vector2d destination, float distance)
    {

            // Check if player is close to mushroom
            double latitudeDifference = Math.Abs(source.x - destination.x);
            double longitudeDifference = Math.Abs(source.y - destination.y);

            if (latitudeDifference < distance && longitudeDifference < distance)
            {
                return true;
            }

        return false;
    }
    // public void LoadMushroomScene()
    // {
    //     SceneManager.LoadScene("TrufflePlane");
    //     //new WaitForSeconds(20);
    // }

    private void Update()
    {
		PlayerLocation = LocationProvider.CurrentLocation.LatitudeLongitude;
		if(PlayerLocation[0]!=0.0f&&PlayerLocation[1]!=0.0f){
        	Scene currentScene = SceneManager.GetActiveScene();
        	if (currentScene.name == "Map" && ToCapture && !ToMap)
        	{
        	    if (CheckIfClose(PlayerLocation, MushroomPosition, 0.1f))
        	    {
        	        Debug.Log("close");
        	        SceneManager.LoadScene("TrufflePlane");
        	        new WaitForSeconds(20);
        	        ToCapture = false;
        	    }
        	}
        	else
        	{
        	    if (ToMap)
        	    {
        	        SceneManager.LoadScene("Map");
        	        ToMap = false;
        	    }
        	}
		}
    }

}
