using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class MovementBulletSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        var jobHandle = Entities.WithName("MoveBulletSystem").
            ForEach((ref Translation position, ref Rotation rotation, ref MovementSpeed movementSpeed, ref BulletTag bulletTag) =>
            {
                position.Value += movementSpeed.Value * math.forward(rotation.Value) * deltaTime;
            })
            .Schedule(inputDeps);

        jobHandle.Complete();

        Entities.WithoutBurst().WithStructuralChanges()
            .ForEach((Entity entity, ref Translation position,
            ref Rotation rotation, ref BulletData bulletdata, ref LifeTime lifeTime) =>
            {
                float distanceToPlanet = math.length(ECSDataManager.instance.planetsPosition[bulletdata.planetTargetIndex]);
                if (distanceToPlanet < 20)
                {
                    lifeTime.Value = 0;
                }
            })
            .Run();
        
        return jobHandle;
    }
}
