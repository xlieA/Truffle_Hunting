using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class PlaneObjectSpawner : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> mushroom = new List<GameObject>();
    private List<GameObject> spawnedObjects = new List<GameObject>();

    [SerializeField]
    public int numberOfMushroom;
    public Text numberOfMushroomText;
    private int mushroomCounter = 0;
    public Text mushroomCounterText;

    // Contains the player's position (constantly updated)
    public PlayerLocation playerLocation;
    
    // List of all mushroom positions
    private MushroomPosition mushroomPositions;

    // How far away from the player should a mushroom spawn
    [SerializeField]
    public float distance;
    // How far away from another mushroom should a mushroom spawn
    [SerializeField]
    public float distanceMushroom;

    public Text longitudeDifferenceText;
    public Text latitudeDifferenceText;
    public Text mushroomDifference;

    private ARRaycastManager _arRaycastManager;
    private ARPlaneManager _arPlaneManager;
    private Vector2 touchPosition;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Effect for picking up mushroom
    [SerializeField]
    public GameObject explosion;

    // Detects objects infront of camera
    RaycastHit hit;

    [SerializeField]
    // How long there shouldn't spawn an object at this position after its collections
    private float spawnCooldownDuration = 5f;
    private Dictionary<Vector3, float> spawnCooldowns = new Dictionary<Vector3, float>();

    // TODO: Create network


    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        _arPlaneManager = GetComponent<ARPlaneManager>();

        playerLocation = GameObject.Find("PlayerLocation").GetComponent<PlayerLocation>();
        mushroomPositions = GameObject.Find("MushroomPosition").GetComponent<MushroomPosition>();

        numberOfMushroomText.text = numberOfMushroom.ToString();

        foreach(GameObject prefab in mushroom)
        {
            // Resize mushroom (the name must be exact)
            if(prefab.name == "mushroom_pink" || prefab.name == "mushroom_red"
                || prefab.name == "mushroom_green" || prefab.name == "mushroom_yellow")
            {
                prefab.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
        }

        // TODO: Initialize instance of network
    }

    // Find first mushroom that's close to the player
    bool FindCloseMushroom()
    {
        // Iterate through all possible mushroom positions
        for(int i = 0; i < mushroomPositions.numberAllTheMushroom - 1; i++) {
            // Check if player is close to mushroom
            double latitudeDifference = Math.Abs(playerLocation.latitudeValue - mushroomPositions.GetMushroom(i).latitude);
            double longitudeDifference = Math.Abs(playerLocation.longitudeValue - mushroomPositions.GetMushroom(i).longitude);
            
            latitudeDifferenceText.text = latitudeDifference.ToString("F5");
            longitudeDifferenceText.text = longitudeDifference.ToString("F5");

            // distance ... maximum distance a user is allowed to be away form a mushroom position and the mushroom still spawns
            if(latitudeDifference < distance && longitudeDifference < distance)
            {
                Debug.Log("Mushroom GPS is close to player!");
                return true;
            }
        }
        
        Debug.Log("Mushroom GPS is too far away from player!");
        return false; 
    }

    // Checks if two mushrooms are too close to spawn next to each other
    bool MushroomFarEnough(Vector3 spawn_position)
    {
        // Remove all the mushroom who have finished their cooldown --> new mushroom can be spawned at this position again
        UpdateCooldown();
        foreach(Vector3 position in spawnCooldowns.Keys)
        {
            // Calculate difference between collected mushroom and mushroom to spawn
            float difference = Vector3.Distance(spawn_position, position);
            mushroomDifference.text = difference.ToString("F5");

            // Can't spawn if mushroom position is still in cooldown
            // Checks if a mushroom is already too close
            if(difference < distanceMushroom && IsOnCooldown(position)) 
            {
                Debug.Log("Mushroom in cooldown is too close to spawn new mushroom!");
                return false;
            }

        }

        foreach(GameObject mushroom in spawnedObjects)
        {
            // Calculate difference between spawned mushroom and mushroom to spawn
            float difference = Vector3.Distance(spawn_position, mushroom.transform.position);
            mushroomDifference.text = difference.ToString("F5");

            // Checks if a mushroom is already too close
            if(difference < distanceMushroom) 
            {
                Debug.Log("Spawned mushroom is too close to spawn new mushroom");
                return false;
            }
        }

        // All other mushrooms are far enough away
        Debug.Log("All mushrooms are far enough away! New mushroom can be spawned!");
        return true;
    }

    // Checks if the user touches the screen
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if(Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }
    
    // Check if a position is on cooldown
    bool IsOnCooldown(Vector3 position)
    {
        if(!(spawnCooldowns.ContainsKey(position)))
        {
            Debug.Log("There is no mushroom on cooldown on this position!");
            return false;
        }

        // Cooldown is finished
        if(Time.time > spawnCooldowns[position])
        {
            Debug.Log("The mushroom at this position has already finished its cooldown");
            return false;
        }

        Debug.Log("There is a mushroom at cooldown at this position");
        return true;
    }

    // Set the cooldown for a position
    void SetCooldown(Vector3 position)
    {
        if(spawnCooldowns.ContainsKey(position))
        {
            Debug.Log("Cooldown of mushroom gets increased!");
            spawnCooldowns[position] = Time.time + spawnCooldownDuration;
        }
        else
        {
            Debug.Log("New mushroom gets set on cooldown");
            spawnCooldowns.Add(position, Time.time + spawnCooldownDuration);
        }
    }

    // Remove position from the directory if it has finished its cooldown
    void UpdateCooldown()
    {
        foreach(Vector3 position in spawnCooldowns.Keys)
        {
            // Cooldown is finished
            if(Time.time > spawnCooldowns[position])
            {
                Debug.Log("The mushroom at this position has already finished its cooldown");
                spawnCooldowns.Remove(position);
            }
        }
    }

    // Spawns a mushroom on the plane the user touches
    void SpawnOnTouch()
    {
        // Check for user input
        if(!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        // Raycast to find plane
        if(_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            if(mushroom != null) {
                // Get random mushroom
                int randomPrefab = UnityEngine.Random.Range(0, mushroom.Count);

                // Spawn mushroom
                if(MushroomFarEnough(hitPose.position))
                {
                    GameObject spawnedObject = Instantiate(mushroom[randomPrefab], hitPose.position, hitPose.rotation);
                    spawnedObjects.Add(spawnedObject);
                    mushroomCounter++;
                }

                mushroomCounterText.text = mushroomCounter.ToString();
            }
        }
    }

    // Spawns a mushroom at the first plane a ray hits
    void RandomSpawn()
    {
        // Raycast to find plane
        if(_arRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon))
        {
            Debug.Log("Plane detected!");
            var hitPose = hits[0].pose;

            if(mushroom != null) {
                // Get random mushroom
                int randomPrefab = UnityEngine.Random.Range(0, mushroom.Count);

                // Spawn mushroom
                if(MushroomFarEnough(hitPose.position))
                {
                    Debug.Log("Spawn mushroom");
                    GameObject spawnedObject = Instantiate(mushroom[randomPrefab], hitPose.position, hitPose.rotation);
                    spawnedObjects.Add(spawnedObject);
                    mushroomCounter++;
                }
                
                mushroomCounterText.text = mushroomCounter.ToString();
            }
        }
    }

    // Collects a mushroom if the player touches it
    void CollectMushroom()
    {
        // Check for touches to destroy objects
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            if (Physics.Raycast(ray, out hit))
            {
                if (spawnedObjects.Contains(hit.collider.gameObject))
                {
                    Destroy(hit.collider.gameObject);
                    spawnedObjects.Remove(hit.collider.gameObject);
                    GameObject spawnedExplosion = Instantiate(explosion, hit.transform.position, hit.transform.rotation);
                    Destroy(spawnedExplosion, 2f); // nothing is left behind

                    // Set a cooldown for the collected position
                    SetCooldown(hit.transform.position);
                    
                    if (hit.collider.gameObject.name == "mushroom_red")
                    {
                        // TODO: Start network
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerLocation.GPSStatus.text == "Running" && mushroomPositions.numberAllTheMushroom > 0 && mushroomCounter < numberOfMushroom)
        {
            Debug.Log("GPS works! Ready for spawning mushroom!");
            // Check if a mushroom is near the player
            if(FindCloseMushroom()) 
            {
                // Spawn mushroom
                //SpawnOnTouch();
                Debug.Log("Call the spawner");
                RandomSpawn();
            }
        }

        if(spawnedObjects.Count > 0)
        {
            CollectMushroom();
        }
    }
}
