using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Core;

public class RotatePlanetsSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        float3 centerWorldPoint = new float3(0, 0, 0);

        var jobHandle = Entities.WithName("RotatePlanetsSystem").
            ForEach((ref Translation position, ref Rotation rotation, ref RotationSpeed rotationSpeed, ref PlanetTag planetTag) =>
            {
                position.Value = ECSDataManager.RotateAroundPoint(position.Value, float3.zero, math.up(), 0.02f * deltaTime);
                rotation.Value = math.mul(math.normalize(rotation.Value), quaternion.AxisAngle(math.up(), rotationSpeed.Value * deltaTime));
            })
            .Schedule(inputDeps);

        jobHandle.Complete();

        Entities.WithoutBurst().WithStructuralChanges()
            .ForEach((ref Translation position, ref PlanetTag planetTag) =>
            {
                ECSDataManager.UpdatePlanetPosition(planetTag.ID, position.Value);
            })
            .Run();

        return jobHandle;
    }
}
