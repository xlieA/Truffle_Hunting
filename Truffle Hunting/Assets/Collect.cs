using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{
    // Collects the mushroom when clicked on it

    public GameObject arCamera;

    // Detect object infront of camera
    RaycastHit hit;

    // Update is called once per frame
    void Update()
    {
        // Detect touch
        if(Input.touchCount > 0)
        {
            // Check if the an object is infront of the camera
            if(Physics.Raycast(arCamera.transform.position, arCamera.transform.forward, out hit))
            {
                // If the object is a mushroom --> destroy it (gameObject must have correct tag set)
                if(hit.transform.tag == "mushroom")
                {
                    Destroy(hit.transform.gameObject);
                }
            }
        }
    }
}
