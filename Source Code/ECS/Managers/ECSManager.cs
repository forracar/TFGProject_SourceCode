using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ECSManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] planetsPrefabs;
    public GameObject spaceShipPrefab;
    public GameObject bulletPrefab;
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


    [Header("Debuggin")]
    public TextMeshProUGUI text;


    private EntityManager manager;

    // Start is called before the first frame update
    void Start()
    {
        // Get Entity Manager
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Create Conversion settings
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

        // Iterate over planets
        InstantiatePlanets(settings);

        // Iterate over spaceships
        InstantiateSpaceShips(settings);
    }

    private void Update()
    {
        int totalPlanets = manager.CreateEntityQuery(ComponentType.ReadOnly<PlanetTag>()).CalculateEntityCount();
        int totalSpaceShips = manager.CreateEntityQuery(ComponentType.ReadOnly<SpaceShipTag>()).CalculateEntityCount();
        int totalEntities = World.DefaultGameObjectInjectionWorld.GetExistingSystem<RotatePlanetsSystem>().EntityManager.GetAllEntities().Length;
        text.text = "Total Entities: " + totalEntities + "\n" + "Total Planets: " + totalPlanets + "\n" + "Total SpaceShips: " + totalSpaceShips; 
    }

    private void InstantiatePlanets(GameObjectConversionSettings settings)
    {
        // Create supply entity prefab
        Entity supplyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(supplyPrefab, settings);

        // Iterate over planets
        for (int i = 0; i < planetsPrefabs.Length; i++)
        {
            // Create planet entity prefab
            Entity planetEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(planetsPrefabs[i], settings);

            // Instantiate Entity to default world
            Entity entityInstance = manager.Instantiate(planetEntityPrefab);

            // Compute initial planet position
            Vector3 initialPosition = ComputeRandomSpawnPlanetPosition(i);

            // Set entity position
            manager.SetComponentData(entityInstance, new Translation() { Value = initialPosition });
            manager.SetComponentData(entityInstance, new PlanetTag() { ID = i });

            // Create waypoints arround planet
            CreateSupplyPoints(initialPosition, numberOfSupplyPointsPerPlanet, supplyEntityPrefab);

            // Add planet positions to datamanager list
            AddPlanetposition(initialPosition);
        }

        ECSDataManager.instance.FillSupplyPointsArray();
        ECSDataManager.instance.FillPlanetsPositionArray();
        ECSDataManager.instance.FillGunLocations();
    }

    private void InstantiateSpaceShips(GameObjectConversionSettings settings)
    {
        // Convert prefab to entity prefab
        Entity spaceShipEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(spaceShipPrefab, settings);
        Entity bulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, settings);

        for (int i = 0; i < numberOfSpaceShipsToInstantiate; i++)
        {
            // Instantiate Entity to default world
            Entity entityInstance = manager.Instantiate(spaceShipEntityPrefab);

            // Compute initial planet position
            Vector3 initialPosition = ComputeRandomSpawnSpaceShipInitialPosition();

            // Set entity position
            manager.SetComponentData(entityInstance, new Translation()
            {
                Value = initialPosition
            });

            // Set entity target position
            manager.SetComponentData(entityInstance, new SpaceShipData()
            {
                supplyPointIndex = ECSDataManager.instance.GetRandomSupplyPoint(),
                isNearToPlanet = false,
                planetIndex = ECSDataManager.instance.GetRandomPlanet(),
                bulletEntityPrefab = bulletEntityPrefab
            });
        }
    }

    private void CreateSupplyPoints(Vector3 planetPos, int numOfSupplyPoints, Entity supplyEntityPrefab)
    {
        for (int i = 0; i < numOfSupplyPoints; i++)
        {
            // Get supply inital position
            float3 initialPosition = GetPositionAroundPlanet(planetPos, distanceBetweenSupplyPointAndPlanet);

            // Create entity instace
            Entity entityInstance = manager.Instantiate(supplyEntityPrefab);

            // Set entity position
            manager.SetComponentData(entityInstance, new Translation()
            {
                Value = initialPosition
            });

            // Add supply point to initial pos
            ECSDataManager.instance.AddSupplyPoint(initialPosition);
        }
    }

    private void AddPlanetposition(Vector3 planetpos)
    {
        ECSDataManager.instance.AddPlanetPosition(planetpos);
    }

    private Vector3 ComputeRandomSpawnPlanetPosition(int index)
    {
        float radius;

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

        return transform.TransformPoint(new float3(
                UnityEngine.Random.Range(minXPosition + centerWorldPoint.x, maxXPosition + centerWorldPoint.x),
                UnityEngine.Random.Range(minYPosition + centerWorldPoint.y, maxYPosition + centerWorldPoint.y),
                UnityEngine.Random.Range(radius + centerWorldPoint.z, radius + centerWorldPoint.z)));
    }

    private Vector3 ComputeRandomSpawnSpaceShipInitialPosition()
    {
        return transform.TransformPoint(new float3(
                UnityEngine.Random.Range(spawnMinXPosition, maxXPosition),
                UnityEngine.Random.Range(spawnMinYPosition, maxYPosition),
                UnityEngine.Random.Range(spawnMinZPosition, thirdPlanetRadiusOrbit)));
    }

    public static Vector3 GetPositionAroundPlanet(Vector3 planetPosition, float radius)
    {
        float angle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
        float yPosition = UnityEngine.Random.Range(-50, 50);

        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);

        return new Vector3(c * radius, yPosition, s * radius) + planetPosition;
    }

}
