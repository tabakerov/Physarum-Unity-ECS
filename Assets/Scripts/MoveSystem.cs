using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public struct MoveJob : IJobEntityBatch
{
    public ComponentTypeHandle<Translation> translationTypeHandle;
    [ReadOnly]public ComponentTypeHandle<MoveData> moveDataTypeHandle;

    public float DeltaTime;

    [BurstCompile]
    private float3 GetDeltaVector(float distance, float angle)
    {
        return new float3(
                distance * math.cos(angle),
                0,
                distance * math.sin(angle)
            ) ;
    }

    [BurstCompile]
    public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
    {
        NativeArray<Translation> translations = batchInChunk.
            GetNativeArray(translationTypeHandle);
        NativeArray<MoveData> moveDatas = batchInChunk.
            GetNativeArray(moveDataTypeHandle);

        for (int i = 0; i < batchInChunk.Count; i++)
        {
           
            float2x2 rot = new float2x2(
                math.cos(moveDatas[i].angle), -math.sin(moveDatas[i].angle),
                math.sin(moveDatas[i].angle), math.cos(moveDatas[i].angle));

            float3 newTranslation = translations[i].Value + DeltaTime * GetDeltaVector(moveDatas[i].speed, moveDatas[i].angle);
            
            if (newTranslation.x < 0) newTranslation.x += 100;
            if (newTranslation.z < 0) newTranslation.z += 100;
            newTranslation.x %= 100;
            newTranslation.z %= 100;
            
            translations[i] = new Translation() { Value = newTranslation };
        }
    }
}

//[DisableAutoCreation]
public class MoveSystem : SystemBase
{
    EntityQuery query;

    protected override void OnCreate()
    {
        base.OnCreate();
        var description = new EntityQueryDesc()
        {
            All = new ComponentType[]
            {
                ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadOnly<MoveData>()
            }
        };
        query = this.GetEntityQuery(description);
    }

    protected override void OnUpdate()
    {

        var updateTranslationJob = new MoveJob();
        updateTranslationJob.moveDataTypeHandle = this.
            GetComponentTypeHandle<MoveData>(false);
        updateTranslationJob.translationTypeHandle = this.
            GetComponentTypeHandle<Translation>(false);

        updateTranslationJob.DeltaTime = World.Time.DeltaTime;


        this.Dependency = updateTranslationJob.
            ScheduleParallel(query, 1, this.Dependency);
        
        
    }
}
