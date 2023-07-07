using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

// Marker and prefab MUST have the EXACT same name
// Expands the AR Tracked Image Manger to detect multiple different markers
[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    // Contains all the prefabs (mushroom models)
    [SerializeField]
    private GameObject[] placeablePrefabs;

    // Use string of same name to find prefab in placeable prefabs
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    private ARTrackedImageManager trackedImageManager;

    private void Awake()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        // Pre-spawn one of each placeable prefabs
        foreach(GameObject prefab in placeablePrefabs)
        {
            // Vector3.zero ... starts out hidden, Quaternion.identity ... default rotation
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            // Make sure name is correctly stored
            newPrefab.name = prefab.name;
            spawnedPrefabs.Add(prefab.name, newPrefab);
        }
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    private void onDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }

    // Manages models
    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach(ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }

        foreach(ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        foreach(ARTrackedImage trackedImage in eventArgs.removed)
        {
            // Disable model
            spawnedPrefabs[trackedImage.name].SetActive(false);
        }
    }

    // Shows model
    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        Vector3 position = trackedImage.transform.position;

        GameObject prefab = spawnedPrefabs[name];
        prefab.transform.position = position;


        // Brown mushroom model is sideways --> rotate
        if (name == "mushroom_brown")
        {
            prefab.transform.localEulerAngles = new Vector3(-90, 0, 0);
        }

        prefab.SetActive(true);

        // Hides other model when looking at new one
        foreach (GameObject go in spawnedPrefabs.Values)
        {
            if(go.name != name)
            {
                go.SetActive(false);
            }
        }
    }
}
