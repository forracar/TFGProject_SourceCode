
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ECSDataManager : MonoBehaviour
{
    public static ECSDataManager instance;
 
    [HideInInspector] public float3[] supplyPoints;
    [HideInInspector] public float3[] planetsPosition;
    [HideInInspector] public float3[] gunLocationsOffSet;

    private List<float3> wp = new List<float3>();
    private List<float3> planetsPos = new List<float3>();
    public List<GameObject> gunLocations = new List<GameObject>();

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    public void AddSupplyPoint(float3 point)
    {
        wp.Add(point);
    }

    public void AddPlanetPosition(float3 position)
    {
        planetsPos.Add(position);
    }

    public void FillGunLocations()
    {
        gunLocationsOffSet = new float3[gunLocations.Count];

        for (int i = 0; i < gunLocations.Count; i++)
        {
            gunLocationsOffSet[i] = gunLocations[i].transform.TransformPoint(gunLocations[i].transform.position);
        }
    }

    public void FillPlanetsPositionArray()
    {
        planetsPosition = new float3[planetsPos.Count];

        for (int i = 0; i < planetsPos.Count; i++)
        {
            planetsPosition[i] = planetsPos[i];
        }
    }

    public void FillSupplyPointsArray()
    {
        supplyPoints = new float3[wp.Count];

        for (int i = 0; i < wp.Count; i++)
        {
            supplyPoints[i] = wp[i];
        }
    }

    public int FindNearestPlanet(float3 position)
    {
        float distance = Mathf.Infinity;
        int returnPointIdx = 0;

        for (int i = 0; i < planetsPosition.Length; i++)
        {
            float computedDistance = Vector3.Distance(position, planetsPosition[i]);
            if (distance > computedDistance)
            {
                distance = computedDistance;
                returnPointIdx = i;
            }
        }

        return returnPointIdx;
    }

    public int GetRandomSupplyPoint()
    {
        return UnityEngine.Random.Range(0, supplyPoints.Length);
    }

    public int GetRandomPlanet()
    {
        return UnityEngine.Random.Range(0, planetsPosition.Length);
    }

    public int FindNearestSupplyPoint(float3 position)
    {
        float distance = Mathf.Infinity;
        int returnPointIdx = 0;

        for (int i = 0; i < supplyPoints.Length; i++)
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

    public static float3 RotateAroundPoint(float3 position, float3 pivot, float3 axis, float delta)
    {
        return math.mul(quaternion.AxisAngle(axis, delta), position - pivot) + pivot;
    }

    public static void UpdatePlanetPosition(int index, float3 position)
    {
        instance.planetsPosition[index] = position;
    }
}
