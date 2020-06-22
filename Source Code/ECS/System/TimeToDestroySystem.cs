using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Core;
using Unity.Collections;

[UpdateAfter(typeof(MovementBulletSystem))]
public class TimeToDestroySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float dt = Time.DeltaTime;
        Entities.WithoutBurst().WithStructuralChanges()
            .ForEach((Entity entity, ref LifeTime lifetimeData) =>
            {
                lifetimeData.Value -= dt;
                if (lifetimeData.Value <= 0f)
                    EntityManager.DestroyEntity(entity);
            })
        .Run();

        return inputDeps;
    }
}
