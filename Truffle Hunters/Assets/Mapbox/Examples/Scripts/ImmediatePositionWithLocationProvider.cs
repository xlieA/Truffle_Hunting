namespace Mapbox.Examples
{
	using Mapbox.Unity.Location;
	using Mapbox.Unity.Map;
	using UnityEngine;
	using Mapbox.Utils;
	public class ImmediatePositionWithLocationProvider : MonoBehaviour
	{

		bool _isInitialized;

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

		// Vector3 _targetPosition;
		public Animator animator;
    	private bool isWalking = false;

   		// Set a threshold distance for movement detection
   		public float movementThreshold = 0.1f;
	
   		private Vector2d previousLocation;
		void CheckWalking()
   		{
   		    // Get the current location
   		    Vector2d currentLocation = LocationProvider.CurrentLocation.LatitudeLongitude;
		
   		    // Calculate the distance between current and previous locations
   		    float distance = (float)Vector2d.Distance(currentLocation, previousLocation);
		
   		    // Check if the distance exceeds the movement threshold
   		    isWalking = distance > movementThreshold;
   		}
		void Start()
		{
			LocationProviderFactory.Instance.mapManager.OnInitialized += () => _isInitialized = true;
			  // Initialize previousLocation with the current location
   			previousLocation = LocationProvider.CurrentLocation.LatitudeLongitude;
		}

		void LateUpdate()
		{
			 CheckWalking();

        	// Update the isWalking variable in the animator
        	animator.SetBool("isWalking", isWalking);

        	// Update the previousLocation for the next frame
        	previousLocation = LocationProvider.CurrentLocation.LatitudeLongitude;
			if (_isInitialized)
			{
				var map = LocationProviderFactory.Instance.mapManager;
				transform.localPosition = map.GeoToWorldPosition(LocationProvider.CurrentLocation.LatitudeLongitude);
			}
		}
	}
}