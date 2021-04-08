using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class LifetimeSystem : SystemBase
{
    private EntityCommandBufferSystem _entityCommandBuffer;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        _entityCommandBuffer = World.GetOrCreateSystem<EntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = _entityCommandBuffer.CreateCommandBuffer().AsParallelWriter();
        
        var dt = Time.DeltaTime;
        
        
        Entities.ForEach((Entity e, int entityInQueryIndex, ref LifetimeData lifetimeData) =>
        {
            lifetimeData.Lifetime -= dt;
            if (lifetimeData.Lifetime < 0f)
            {
                ecb.DestroyEntity(entityInQueryIndex, e);           
            }
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as 'in', which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,
            //     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;
        }).Schedule();
    }
}
