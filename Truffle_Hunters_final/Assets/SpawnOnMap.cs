namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;
	using System.Collections;

	public class SpawnOnMap : MonoBehaviour
	{
		public static SpawnOnMap Instance { set; get; }

		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		[Geocode]
		public List<string> _locationStrings = new List<string>();
		Vector2d[] _locations = new Vector2d[0];

		[SerializeField]
		float _spawnScale = 100f;

		[SerializeField]
		GameObject _markerPrefab;

		List<GameObject> _spawnedObjects = new List<GameObject>();

		public int isset = 0;


		IEnumerator SpawnObject()
		{
			while (_locationStrings.Count == 0)
			{
				yield return new WaitForSeconds(1);
			}
			_locations = new Vector2d[_locationStrings.Count];
			_spawnedObjects = new List<GameObject>();
			for (int i = 0; i < _locationStrings.Count; i++)
			{

				var locationString = _locationStrings[i];
				_locations[i] = Conversions.StringToLatLon(locationString);
				var instance = Instantiate(_markerPrefab);

				instance.GetComponent<EventPointer>().eventPos = _locations[i];
				instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
				instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				_spawnedObjects.Add(instance);
				isset++;
			}
		}

		private void Update()
		{
			// Instance = this;
			// DontDestroyOnLoad(gameObject);
			// int count = _spawnedObjects.Count;
			// for (int i = 0; i < count; i++)
			// {
			// 	var spawnedObject = _spawnedObjects[i];
			// 	var location = _locations[i];
			// 	spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
			// 	spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			// }
		}

		private void Start()
		{
			Instance = this;
			// DontDestroyOnLoad(gameObject);
			StartCoroutine(SpawnObject());
		}
	}
}