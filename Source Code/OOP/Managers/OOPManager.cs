using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OOPManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] planetsPrefabs;
    public GameObject spaceShipPrefab;
    public GameObject supplyPrefab;

    [Header("General Settings")]
    public Vector3 centerWorldPoint;
    public int numberOfSupplyPointsPerPlanet = 2;
    [Range(100, 200)] public float distanceBetweenSupplyPointAndPlanet;

    [Header("Planet Settings")]
    [Range(0f, 1f)] public float minRotationPlanetSpeed;
    [Range(0f, 1f)] public float maxRotationPlanetSpeed;

    [Range(-50, 50)] public float minXPosition;
    [Range(-50, 50)] public float maxXPosition;

    [Range(-50, 50)] public float minYPosition;
    [Range(-50, 50)] public float maxYPosition;

    [Range(0, 10)] public float firstPlanetRadiusOrbit;
    [Range(20, 30)] public float secondPlanetRadiusOrbit;
    [Range(40, 50)] public float thirdPlanetRadiusOrbit;

    [Header("SpaceShip Settings")]
    [Range(-100, 100)] public float spawnMinXPosition;
    [Range(-100, 100)] public float spawnMaxXPosition;

    [Range(-50, 50)] public float spawnMinYPosition;
    [Range(-50, 50)] public float spawnMaxYPosition;

    [Range(-100, 100)] public float spawnMinZPosition;
    [Range(-100, 100)] public float spawnMaxZPosition;

    public int numberOfSpaceShipsToInstantiate = 500;
    // Start is called before the first frame update
    void Start()
    {
        // Instantiate all planets
        InstantiatePlanets();

        //Instantiate all spaceships
        InstantiateSpaceships();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Add UI info
    }

    private void InstantiatePlanets()
    {
        for (int i = 0; i < planetsPrefabs.Length; i++)
        {
            // Instantaite planet instance
            GameObject planetInstance = Instantiate(planetsPrefabs[i]);

            // Compute initial position
            Vector3 initialPosition = ComputeRandomSpawnPlanetPosition(i);

            // Set position
            planetInstance.transform.position = initialPosition;

            // Set planet ID
            planetInstance.GetComponent<RotateAroundPoint>().ID = i;

            // Add planet position to list
            OOPDataManager.instance.AddPlanetPosition(transform.position);

            // Instantiate supplyPoints arround planet
            InstantiateSupplyPoints(initialPosition);
        }
    }

    private void InstantiateSpaceships()
    {
        // Instantiate number of space ships desired
        for (int i = 0; i < numberOfSpaceShipsToInstantiate; i++)
        {
            // Create spaceship instance
            GameObject spaceShipInstance = Instantiate(spaceShipPrefab);

            // Compute initial spaceship position
            Vector3 initialPosition = ComputeRandomSpawnSpaceShipInitialPosition();

            // Set spaceship position
            spaceShipInstance.transform.position = initialPosition;

            // Set planet target
            spaceShipInstance.GetComponent<SpaceShip>().planetIndex = OOPDataManager.instance.GetRandomPlanet();

            // Set supply point target
            spaceShipInstance.GetComponent<SpaceShip>().supplyIndex = OOPDataManager.instance.GetRandomSupplyPoint();
        }

        OOPDataManager.instance.FillGunLocations();
    }

    private void InstantiateSupplyPoints(Vector3 planetPos)
    {
        // Create X number of supply points
        for (int i = 0; i < numberOfSupplyPointsPerPlanet; i++)
        {
            // Create supply instance
            GameObject supplyInstance = Instantiate(supplyPrefab);

            // Compute initial position
            Vector3 initialPosition = GetPositionAroundPlanet(planetPos, distanceBetweenSupplyPointAndPlanet);

            // Set supply position
            supplyInstance.transform.position = initialPosition;

            // Add supply point position in OOPDataManager
            OOPDataManager.instance.AddSupplyPoint(supplyInstance.transform.position);
        }
    }

    private Vector3 ComputeRandomSpawnSpaceShipInitialPosition()
    {
        return transform.TransformPoint(new Vector3(
                Random.Range(spawnMinXPosition, maxXPosition),
                Random.Range(spawnMinYPosition, maxYPosition),
                Random.Range(spawnMinZPosition, thirdPlanetRadiusOrbit)));
    }

    private Vector3 ComputeRandomSpawnPlanetPosition(int index)
    {
        // Create radius variable
        float radius;

        // Set radius depending on planet id
        switch (index)
        {
            case 1:
                radius = firstPlanetRadiusOrbit;
                break;
            case 2:
                radius = secondPlanetRadiusOrbit;
                break;
            default:
                radius = thirdPlanetRadiusOrbit;
                break;
        }

        // Compute and return planet position
        return transform.TransformPoint(new Vector3(
                Random.Range(minXPosition + centerWorldPoint.x, maxXPosition + centerWorldPoint.x),
                Random.Range(minYPosition + centerWorldPoint.y, maxYPosition + centerWorldPoint.y),
                Random.Range(radius + centerWorldPoint.z, radius + centerWorldPoint.z)));
    }

    public static Vector3 GetPositionAroundPlanet(Vector3 planetPosition, float radius)
    {
        // Get random angle
        float angle = Random.Range(0f, 2 * Mathf.PI);
        // Get random y position
        float yPosition = Random.Range(-50, 50);

        // Compute sinus
        float s = Mathf.Sin(angle);
        // Compute cosinus
        float c = Mathf.Cos(angle);

        // Return position
        return new Vector3(c * radius, yPosition, s * radius) + planetPosition;
    }
}
