using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public struct TrailSpawnJob : IJobEntityBatch
{
    [ReadOnly]public ComponentTypeHandle<Translation> translationTypeHandle;
    [ReadOnly]public ComponentTypeHandle<TrailData> trailDataTypeHandle;
    public EntityCommandBuffer ecb;
    public float CurrentTime;
    public EntityArchetype Archetype;

    [BurstCompile]
    public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
    {
        NativeArray<Translation> translations = batchInChunk.
                        GetNativeArray(translationTypeHandle);
        NativeArray<TrailData> trailDatas = batchInChunk.
                        GetNativeArray(trailDataTypeHandle);

        for (int i = 0; i < batchInChunk.Count; i++)
        {
            if (trailDatas[i].lastSpawnedTime < CurrentTime + trailDatas[i].spawnRate)
            {
                ecb.CreateEntity(Archetype);
            }
        }
    }
}
public class TrailSpawnSystem : SystemBase
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
        var currentTime = (float)Time.ElapsedTime;
        
        Entities.ForEach((int entityInQueryIndex, ref TrailData trailData, in Translation translation) =>
        {
            //if (UnityEngine.Random.value > 0.95f)
            //{
                var instance = ecb.Instantiate(entityInQueryIndex, trailData.particleEntity);
                ecb.SetComponent(entityInQueryIndex, instance, new Translation()
                {
                                Value = translation.Value
                });
                ecb.SetComponent(entityInQueryIndex, instance, new MoveData()
                {
                                angle = 2 * math.PI * math.frac(math.sin(math.frac(100f*translation.Value.x))+
                                                  math.cos(math.frac(323.42f*translation.Value.x))),
                                speed = 0.3f * math.frac(math.sin(math.frac(72.325f*translation.Value.x))+
                                                         math.cos(math.frac(10.452f*translation.Value.x)))
                });
             //   trailData.lastSpawnedTime = currentTime;
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
    }
}
