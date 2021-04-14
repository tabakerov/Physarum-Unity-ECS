using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Collider = Unity.Physics.Collider;

//[DisableAutoCreation]
public class TrailSpawnSystem : SystemBase
{
    public static RenderMesh renderMesh;
    public static bool enabled;
    private EntityCommandBufferSystem _entityCommandBuffer;
    

    protected override void OnCreate()
    {
        base.OnCreate();
        _entityCommandBuffer = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        
    }

    protected override void OnUpdate()
    {
        if (!enabled) return;
        
        var ecb = _entityCommandBuffer.CreateCommandBuffer().AsParallelWriter();
        var currentTime = (float) Time.ElapsedTime;
        BlobAssetReference<Unity.Physics.Collider> spCollider =
                        Unity.Physics.SphereCollider.Create(new SphereGeometry()
                        {
                                        Center = float3.zero,
                                        Radius = 1f
                        });
        PhysicsCollider collider = new PhysicsCollider() {Value = spCollider};
        var t = Time.ElapsedTime;
        Entities.ForEach((Entity e, int entityInQueryIndex, ref TrailData trailData, in Translation translation) =>
        {
            if (trailData.lastSpawnedTime + trailData.spawnRate > t) return;
            
            var instance = ecb.Instantiate(entityInQueryIndex, e);
            
            /*
            ecb.AddComponent(entityInQueryIndex, instance, new Translation()
            {
                            Value = translation.Value
            });
            */
            ecb.AddComponent(entityInQueryIndex, instance, new LifetimeData()
            {
                            Lifetime = 5f
            });
            ecb.AddComponent(entityInQueryIndex, instance, collider);
            ecb.RemoveComponent<TrailData>(entityInQueryIndex, instance);
            ecb.SetComponent(entityInQueryIndex, instance, new MoveData()
            {
                            angle = 4 * math.PI * math.frac(math.sin(math.frac(100f * translation.Value.x)) +
                                                            math.cos(math.frac(323.42f * translation.Value.x))),
                            speed = 2f * math.frac(math.sin(math.frac(72.325f * translation.Value.x)) +
                                                     math.cos(math.frac(10.452f * translation.Value.x)))
            });
            /*
            ecb.SetComponent(entityInQueryIndex, instance, new MoveData()
            {
                            angle = 2 * math.PI * math.frac(math.sin(math.frac(100f * translation.Value.x)) +
                                                            math.cos(math.frac(323.42f * translation.Value.x))),
                            speed = 0.3f * math.frac(math.sin(math.frac(72.325f * translation.Value.x)) +
                                                     math.cos(math.frac(10.452f * translation.Value.x)))
            });*/
            
            ecb.SetComponent(entityInQueryIndex, instance, new Translation()
            {
                            Value = translation.Value
            });
            //ecb.SetSharedComponent(entityInQueryIndex, instance, rm);
            
            trailData.lastSpawnedTime = currentTime;
            //}
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as 'in', which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,
            //     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;
        }).ScheduleParallel();
        _entityCommandBuffer.AddJobHandleForProducer(this.Dependency);
    }
}