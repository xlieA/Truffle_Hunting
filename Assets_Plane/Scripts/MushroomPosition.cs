using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomPosition : MonoBehaviour
{
    // x: latitude, y: longitude, z: altitude
    public List<DoubleVector3> allTheMushroom = new List<DoubleVector3>();
    public int numberAllTheMushroom = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize mushroom positions for debugging
        // Set to your current position
        SetPosition(new DoubleVector3(1, 1, 1));
    }

    // Adds a new mushroom to the mushroom list
    public void SetPosition(DoubleVector3 position) 
    {
        DoubleVector3 pos = new DoubleVector3(position.latitude, position.longitude, 0);
        allTheMushroom.Add(pos);
        numberAllTheMushroom++;
    }

    // Removes a mushroom at position i in the mushroom list
    public void RemoveMushroom(int i)
    {
        if(i >= 0 && i < allTheMushroom.Count)
        {
            allTheMushroom.RemoveAt(i);
            numberAllTheMushroom--;
        }    
    }

    // Returns the mushroom on the ith position in the mushroom list
    public DoubleVector3 GetMushroom(int i)
    {
        if(i < allTheMushroom.Count)
        {
            return allTheMushroom[i];
        }

        // max lat: 90, max long: 180, max alt: 1.000
        // just something that's never close to a mushroom
        return new DoubleVector3(190, 280, 2000);
    }
}
