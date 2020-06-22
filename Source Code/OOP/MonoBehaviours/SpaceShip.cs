using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class SpaceShip : MonoBehaviour
{
    public GameObject bulletPrefab = null;
    [Range(1f, 5f)] public float minMovementSpeed = 5f;
    [Range(5f, 10f)] public float maxMovementSpeed = 10f;
    public float rotationSpeed = 1f;

    public int shootMinDistance = 45;
    public int planetOffSetDistance = 20;
    public int supplyOffSetDistance = 10;
    public int planetIndex = 1;
    public int supplyIndex = 1;

    private bool m_isNearToPlanet = false;
    private float movementSpeed = 0;
    private Transform m_transform = null;

    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
        movementSpeed = UnityEngine.Random.Range(minMovementSpeed, maxMovementSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        // Compute distance from planet and supply point
        float distanceFromPlanet = math.distance(m_transform.position, OOPDataManager.PlanetPosition[planetIndex]);
        float distanceFromSupplyPoint = math.distance(m_transform.position, OOPDataManager.SupplyPoints[supplyIndex]);

        // Update spaceship state
        UpdateState(distanceFromPlanet, distanceFromSupplyPoint);

        //Update spaceship position
        UpdatePosition(dt);

        // Shoot if it's needed
        Shoot(distanceFromPlanet);
    }

    private void UpdateState(float distanceFromPlanet, float distanceFromSupplyPoint)
    {
        if (distanceFromPlanet < planetOffSetDistance)
        {
            ChangeSupplyPoint();
            m_isNearToPlanet = true;
        }
        else if (distanceFromSupplyPoint < supplyOffSetDistance)
        {
            ChangePlanetTarget();
            m_isNearToPlanet = false;
        }
    }

    private void UpdatePosition(float dt)
    {
        Vector3 l_faceDirection;
        quaternion l_targetDirection;

        // Compute forward vector
        if (m_isNearToPlanet)
            l_faceDirection = OOPDataManager.SupplyPoints[supplyIndex] - m_transform.position;
        else
            l_faceDirection = OOPDataManager.PlanetPosition[planetIndex] - m_transform.position;

        l_targetDirection = quaternion.LookRotation(l_faceDirection, math.up());

        // Set rotation value
        m_transform.rotation = math.slerp(m_transform.rotation, l_targetDirection, rotationSpeed * dt);

        // Set position value
        m_transform.position += (Vector3)math.forward(m_transform.rotation) * movementSpeed * dt;
    }

    private void Shoot(float distanceFromPlanet)
    {
        Vector3 directionToTarget = OOPDataManager.PlanetPosition[planetIndex] - m_transform.position;
        // Compute angle to target (in radians)
        float angleToTarget = math.acos(math.dot(math.forward(m_transform.rotation), directionToTarget) /
                    (math.length(math.forward(m_transform.rotation)) * math.length(directionToTarget)));

        // If angleToTarget is less than 10 and disntace is less than the min distanco allowed for fire
        if (angleToTarget < math.radians(10) && distanceFromPlanet < shootMinDistance)
        {
            // Fore each gun position
            foreach (Vector3 gunPosition in OOPDataManager.GunPositions)
            {
                // Instantiate bullet
                GameObject bulletInstance = Instantiate(bulletPrefab);

                // Set position
                bulletInstance.transform.position = transform.position + (Vector3)math.mul(m_transform.rotation, gunPosition);

                // Set rotation
                bulletInstance.transform.rotation = transform.rotation;
            }
        }
    }

    private void ChangeSupplyPoint()
    {
        supplyIndex++;
        if (supplyIndex >= OOPDataManager.SupplyPoints.Count)
        {
            supplyIndex = 0;
        }
    }

    private void ChangePlanetTarget()
    {
        planetIndex++;
        if (planetIndex >= OOPDataManager.PlanetPosition.Count)
        {
            planetIndex = 0;
        }
    }
}
