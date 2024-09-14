using UnityEngine;
using Mapbox.Unity.Location;
using Mapbox.Utils;
using Mapbox.Unity.Map;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    private bool isWalking = false;

    // You may need references to Map components
    // Adjust the following variables based on your project setup
    public AbstractMap map;
    public ILocationProvider locationProvider;

    // Set a threshold distance for movement detection
    public float movementThreshold = 0.1f;

    private Vector2d previousLocation;

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator not assigned to PlayerMovementController script!");
        }

        // Initialize previousLocation with the current location
        previousLocation = locationProvider.CurrentLocation.LatitudeLongitude;

        // Enable the gyroscope
        Input.gyro.enabled = true;
    }

    void Update()
    {
        // Check if the player is walking based on movement distance
        CheckWalking();

        // Update the isWalking variable in the animator
        animator.SetBool("isWalking", isWalking);

        // Update the previousLocation for the next frame
        previousLocation = locationProvider.CurrentLocation.LatitudeLongitude;

        // Rotate the character based on the device's orientation
        RotatePlayer();
    }

    void CheckWalking()
    {
        // Get the current location
        Vector2d currentLocation = locationProvider.CurrentLocation.LatitudeLongitude;

        // Calculate the distance between current and previous locations
        float distance = (float)Vector2d.Distance(currentLocation, previousLocation);

        // Check if the distance exceeds the movement threshold
        isWalking = distance > movementThreshold;
    }

    void RotatePlayer()
    {
        // Use the device's gyroscope to get the orientation
        Quaternion playerRotation = Input.gyro.attitude;

        // Apply the rotation to the player's transform
        transform.rotation = playerRotation;
    }
}

