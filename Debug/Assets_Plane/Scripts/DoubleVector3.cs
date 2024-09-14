using UnityEngine;

public class DoubleVector3 
{
    public double latitude;
    public double longitude;
    public double altitude;

    public DoubleVector3(double x, double y, double z) 
    {
        this.latitude = x;
        this.longitude = y;
        this.altitude = z;
    }
}