using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Core;
using Unity.Collections;

/// <summary>
/// System that moves spaceship around the world
/// </summary>
public class MovementSpaceShipSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        NativeArray<float3> supplyPointPositions = new NativeArray<float3>(ECSDataManager.instance.supplyPoints, Allocator.TempJob);
        NativeArray<float3> planetPositions = new NativeArray<float3>(ECSDataManager.instance.planetsPosition, Allocator.TempJob);

        var jobHandle = Entities.WithName("MoveSpaceshipSystem").
            ForEach((ref Translation position,
            ref Rotation rotation,
            ref MovementSpeed speed,
            ref RotationSpeed rotationSpeed,
            ref SpaceShipTag spaceShipTag,
            ref SpaceShipData spaceShipData)
            =>
            {
                // Compute distance from planet
                float distanceFromPlanet = math.distance(position.Value, planetPositions[spaceShipData.planetIndex]);
                float distanceFromSupplyPoint = math.distance(position.Value, supplyPointPositions[spaceShipData.supplyPointIndex]);
                float3 faceDirection = float3.zero;
                quaternion targetDirection = quaternion.identity;

                if (distanceFromPlanet < 20)
                {
                    spaceShipData.isNearToPlanet = true;
                    spaceShipData.planetIndex++;
                    if (spaceShipData.planetIndex >= planetPositions.Length)
                    {
                        spaceShipData.planetIndex = 0;
                    }
                }
                else if (distanceFromSupplyPoint < 10)
                {
                    spaceShipData.supplyPointIndex++;
                    if (spaceShipData.supplyPointIndex >= supplyPointPositions.Length)
                    {
                        spaceShipData.supplyPointIndex = 0;
                    }
                    spaceShipData.isNearToPlanet = false;
                }

                if (spaceShipData.isNearToPlanet)
                    faceDirection = supplyPointPositions[spaceShipData.supplyPointIndex] - position.Value;
                else
                    faceDirection = planetPositions[spaceShipData.planetIndex] - position.Value;

                targetDirection = quaternion.LookRotation(faceDirection, math.up());

                // Set rotation value and position value
                rotation.Value = math.slerp(rotation.Value, targetDirection, deltaTime * rotationSpeed.Value);
                position.Value += deltaTime * speed.Value * math.forward(rotation.Value);

            })
            .Schedule(inputDeps);

        supplyPointPositions.Dispose(jobHandle);
        planetPositions.Dispose(jobHandle);
        

        return jobHandle;
    }
}
