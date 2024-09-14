using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Examples;
using Mapbox.Utils;
using UnityEngine.UI;
using TMPro;
public class EventPointer : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float amplitude = 2.0f;
    [SerializeField] float frequency = 0.50f;
    float scale = 1.1f;
    bool ClickEnabled = false;
    LocationStatus playerLocation;
    public Mapbox.Utils.Vector2d eventPos;
    [SerializeField] public TextMeshProUGUI debug_touch;

    // Update is called once per frame
    void Update()
    {
        FloatAndRotatePointer();
        if (Input.touchCount > 0)
        {
            // Loop through all the touches
            for (int i = 0; i < Input.touchCount; i++)
            {
                // Get the current touch
                Touch touch = Input.GetTouch(i);

                // Check if the touch is over the current GameObject
                if (IsTouchOverObject(touch))
                {
                    debug_touch.text = "object touched";
                    // Perform actions when the GameObject is touched
                    HandleTouch();
                }
            }
        }
    }

    bool IsTouchOverObject(Touch touch)
    {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        debug_touch.text = "istouchoverobject called";
        debug_touch.text += touch.position.ToString();
        // Cast a ray and check if it hits the GameObject
        if (Physics.Raycast(ray, out hit))
        {
            bool res = hit.collider.gameObject.name == "pinpoint(Clone)";
            debug_touch.text += hit.collider.gameObject.name;

            debug_touch.text += " return " + res.ToString();
            return res;
        }
        return false;
    }

    void HandleTouch()
    {
        if (true)
        {
            // Do something when the GameObject is touched
            debug_touch.text = "handletouch called ";
            GameObject.Find("Loader").GetComponent<MapCaptureSceneTrans>().MushroomPosition.Set((float)eventPos.x, (float)eventPos.y);
            GameObject.Find("Loader").GetComponent<MapCaptureSceneTrans>().ToCapture = true;
            debug_touch.text += GameObject.Find("Loader").GetComponent<MapCaptureSceneTrans>().ToCapture.ToString();
            //ClickEnabled = false;
        }
    }

    void FloatAndRotatePointer()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude) + 3, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ClickEnabled = true;
            //amplitude *= scale;
        }
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player") && ClickEnabled)
    //     {
    //         ClickEnabled = false;
    //         amplitude /= scale;
    //     }
    // }

}
