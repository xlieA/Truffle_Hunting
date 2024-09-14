using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using System;

using UnityEngine.SceneManagement;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class PlaneObjectSpawner : MonoBehaviour
{
    
    public UpdateUser updateUserInstance;
    public AudioSource audioSource;
    [SerializeField]
    public int numberOfMushroom;
    public Text numberOfMushroomText;
    private int mushroomCounter = 0;
    public Text mushroomCounterText;

    [SerializeField]
    public float distance;
    [SerializeField]
    public float distanceMushroom;

    [SerializeField]
    public GameObject[] mushroom;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private ARRaycastManager _arRaycastManager;
    private ARPlaneManager _arPlaneManager;

    private StyleTransfer style;

    [SerializeField]
    public GameObject explosion;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    RaycastHit hit;

    [SerializeField]
    // How long there shouldn't spawn an object at this position after its collections
    private float spawnCooldownDuration = 5f;
    //public Canvas mushroomCanvas; 
    private Dictionary<Vector3, float> spawnCooldowns = new Dictionary<Vector3, float>();
    bool PlaneDetection()
    {
        return _arRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon);

    }
    void checkLevelUpdate()
    {
        if ((GameManager.Instance.accquiredXP - GameManager.Instance.level * 500) > 0) GameManager.Instance.level++;
    }

    IEnumerator SpawnMushroom()
    {
        yield return new WaitForSeconds(1); // Wait for a short time

        if (PlaneDetection())
        {
            var hitPose = hits[0].pose;
            Debug.Log("Plane detected!");

            if (mushroom != null && mushroom.Length > 0)
            {
                Debug.Log("Spawning mushroom...");

                // Resize mushroom
                int randomPrefab = UnityEngine.Random.Range(0, mushroom.Length);
                float randomScale = UnityEngine.Random.Range(0.1f, 0.4f);
                mushroom[randomPrefab].transform.localScale = new Vector3(randomScale, randomScale, randomScale);//new Vector3(0.1f, 0.1f, 0.1f);

                if (MushroomFarEnough(hitPose.position))
                {
                    GameObject spawnedObject = Instantiate(mushroom[randomPrefab], hitPose.position, hitPose.rotation);
                    spawnedObjects.Add(spawnedObject);
                    mushroomCounter++;
                }

                mushroomCounterText.text = mushroomCounter.ToString();
            }
            if (mushroomCounter != numberOfMushroom)
            {
                StartCoroutine(SpawnMushroom());
            }
        }
        else
        {
            Debug.Log("Plane not detected. Waiting for the next attempt...");
            StartCoroutine(SpawnMushroom());
        }


    }
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
                    if (audioSource != null)
                    {
                        audioSource.Play();
                    }
                    // Set a cooldown for the collected position
                    //SetCooldown(hit.transform.position);

                    GameManager.Instance.accquiredXP += 50;
                    checkLevelUpdate();

                    updateUserInstance.UpdateUserInfo();

                    if (hit.collider.gameObject.name.Contains("mushroom_yellow"))
                    {
                        style.stylizeImage = true;
                    }
                    else
                    {
                        style.stylizeImage = false;
                    }

                }
            }
        }
    }
    void SetCooldown(Vector3 position)
    {
        if (spawnCooldowns.ContainsKey(position))
        {
            spawnCooldowns[position] = Time.time + spawnCooldownDuration;
        }
        else
        {
            spawnCooldowns.Add(position, Time.time + spawnCooldownDuration);
        }
    }


    bool IsOnCooldown(Vector3 position)
    {
        if (!(spawnCooldowns.ContainsKey(position)))
        {
            return false;
        }

        if (Time.time > spawnCooldowns[position])
        {
            spawnCooldowns.Remove(position);
            return false;
        }
        return true;
    }


    bool MushroomFarEnough(Vector3 spawn_position)
    {
        foreach (Vector3 position in spawnCooldowns.Keys)
        {
            // Calculate difference between collected mushroom and mushroom to spawn
            float difference = Vector3.Distance(spawn_position, position);

            // Can't spawn if mushroom position is still in cooldown
            // Checks if a mushroom is already too close
            if (difference < distanceMushroom || IsOnCooldown(position))
            {
                return false;
            }

        }

        foreach (GameObject mushroom in spawnedObjects)
        {
            // Calculate difference between spawned mushroom and mushroom to spawn
            float difference = Vector3.Distance(spawn_position, mushroom.transform.position);

            // Checks if a mushroom is already too close
            if (difference < distanceMushroom)
            {
                return false;
            }
        }

        // All other mushrooms are far enough away
        return true;
    }
    public void CloseScene()
    {
        GameObject.Find("Loader").GetComponent<MapCaptureSceneTrans>().ToCapture = false;
        GameObject.Find("Loader").GetComponent<MapCaptureSceneTrans>().ToMap = true;
    }
    void Update()
    {
        if (spawnedObjects.Count > 0)
        {
            CollectMushroom();
        }

    }
    void Start()
    {
        numberOfMushroom = UnityEngine.Random.Range(1, 10);
        _arRaycastManager = GetComponent<ARRaycastManager>();
        _arPlaneManager = GetComponent<ARPlaneManager>();

        numberOfMushroomText.text = numberOfMushroom.ToString();

        StartCoroutine(SpawnMushroom());
        style = GetComponentInChildren<StyleTransfer>(true);
        new WaitForSeconds(10);
    }

}