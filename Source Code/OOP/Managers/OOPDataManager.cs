using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class OOPDataManager : MonoBehaviour
{
    public static OOPDataManager instance;

    private List<Vector3> supplyPoints = new List<Vector3>();
    public static List<Vector3>  SupplyPoints
    {
        get { return instance.supplyPoints; }
    }
    private List<Vector3> planetsPos = new List<Vector3>();
    public static List<Vector3> PlanetPosition
    {
        get { return instance.planetsPos; }
    }
    public List<Transform> gunLocations = new List<Transform>();
    private List<Vector3> gunPosition = new List<Vector3>();
    public static List<Vector3> GunPositions
    {
        get { return instance.gunPosition; }
    }

    private void Awake()
    {
        // Singleton Pattern
        // If instance is already instantiated and instance isn't equal to this instance destroy
        if (instance != null && instance != this)
            Destroy(this);
        // Otherwise set instance as self
        else
            instance = this;
    }

    // Add supply point to list
    public void AddSupplyPoint(Vector3 point)
    {
        supplyPoints.Add(point);
    }

    public void FillGunLocations()
    {
        for (int i = 0; i < gunLocations.Count; i++)
        {
            gunPosition.Add(gunLocations[i].TransformPoint(gunLocations[i].position));
        }
    }

    // Add planet position to list
    public void AddPlanetPosition(Vector3 position)
    {
        planetsPos.Add(position);
    }

    // Search near planet with a given point in world
    public int FindNearestPlanet(Vector3 position)
    {
        // Set distance as infinity, so any distance computed will be less than infinity
        float distance = Mathf.Infinity;
        // Current planet ID
        int returnPointIdx = 0;

        // Iterate over planet positions
        for (int i = 0; i < planetsPos.Count; i++)
        {
            // Compute distance
            float computedDistance = Vector3.Distance(position, planetsPos[i]);

            // If current disntace is less than computed distance
            if (distance > computedDistance)
            {
                // Set current distance as computed distance
                distance = computedDistance;
                // Set planet ID as return ID
                returnPointIdx = i;
            }
        }

        // Return planet ID
        return returnPointIdx;
    }

    // Return random supply point ID(int)
    public int GetRandomSupplyPoint()
    {
        return UnityEngine.Random.Range(0, supplyPoints.Count);
    }

    // Return random planet ID(int)
    public int GetRandomPlanet()
    {
        return UnityEngine.Random.Range(0, planetsPos.Count);
    }

    // Return supply point ID. The nearest supply in poistion
    public int FindNearestSupplyPoint(Vector3 position)
    {
        float distance = Mathf.Infinity;
        int returnPointIdx = 0;

        for (int i = 0; i < supplyPoints.Count; i++)
        {
            float computedDistance = Vector3.Distance(position, supplyPoints[i]);
            if (distance > computedDistance)
            {
                distance = computedDistance;
                returnPointIdx = i;
            }
        }

        return returnPointIdx;
    }

    // Return position rotating around a point
    public static Vector3 RotateAroundPoint(Vector3 position, Vector3 pivot, Vector3 axis, float delta)
    {
        return (Vector3)math.mul(quaternion.AxisAngle(axis, delta), position - pivot) + pivot;
    }

    // Update planet position at given index
    public static void UpdatePlanetPosition(int index, float3 position)
    {
        instance.planetsPos[index] = position;
    }
}
