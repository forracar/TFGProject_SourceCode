using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Core;
using Unity.Collections;

public class SapceshipShootSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        NativeArray<float3> gunPositions = new NativeArray<float3>(ECSDataManager.instance.gunLocationsOffSet, Allocator.TempJob);

        Entities.WithoutBurst().WithStructuralChanges()
            .ForEach((Entity entity,
            ref Translation position,
            ref Rotation rotation,
            ref SpaceShipTag spaceShipTag,
            ref SpaceShipData spaceShipData)
            => {
                float distanceFromPlanet = math.distance(position.Value, ECSDataManager.instance.planetsPosition[spaceShipData.planetIndex]);
                float3 directionToTarget = ECSDataManager.instance.planetsPosition[spaceShipData.planetIndex] - position.Value;
                float angleToTarget = math.acos(math.dot(math.forward(rotation.Value), directionToTarget) /
                    (math.length(math.forward(rotation.Value)) * math.length(directionToTarget)));

                if (angleToTarget < math.radians(10) && distanceFromPlanet < 35)
                {
                    foreach (float3 gunPosition in gunPositions)
                    {
                        Entity bulletInstance = EntityManager.Instantiate(spaceShipData.bulletEntityPrefab);
                        EntityManager.SetComponentData(bulletInstance, new Translation { Value = position.Value + math.mul(rotation.Value, gunPosition) });
                        EntityManager.SetComponentData(bulletInstance, new Rotation { Value = rotation.Value });
                        EntityManager.SetComponentData(bulletInstance, new BulletData { planetTargetIndex = spaceShipData.planetIndex });
                    }
                }
            })
            .Run();

        gunPositions.Dispose();

        return inputDeps;
    }
}
