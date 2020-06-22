using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SpaceShipData : IComponentData
{
    public int supplyPointIndex;
    public int planetIndex;
    public bool isNearToPlanet;
    public Entity bulletEntityPrefab;
}
