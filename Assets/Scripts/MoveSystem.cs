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
    public ComponentTypeHandle<MoveData> moveDataTypeHandle;
    [ReadOnly]public ComponentTypeHandle<SensorData> sensorDataTypeHandle;

    public float DeltaTime;
    [ReadOnly] public PhysicsWorld physicsWorld;

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
        NativeArray<SensorData> sensorDatas = batchInChunk.
            GetNativeArray(sensorDataTypeHandle);

        for (int i = 0; i < batchInChunk.Count; i++)
        {
            int left = 0;
            int right = 0;
            int center = 0;
            float3 leftPosition = GetDeltaVector(
                sensorDatas[i].leftSensorDistance,
                sensorDatas[i].leftSensorAngle);
            float3 centerPosition = GetDeltaVector(
                sensorDatas[i].centralSensorDistance,
                sensorDatas[i].centralSensorAngle);
            float3 rightPosition = GetDeltaVector(
                sensorDatas[i].rightSensorDistance,
                sensorDatas[i].rightSensorAngle);

            NativeList<DistanceHit> distanceHits =
                new NativeList<DistanceHit>(Allocator.Temp);

            if (physicsWorld.CalculateDistance(new PointDistanceInput()
            {
                Filter = CollisionFilter.Default,
                MaxDistance = sensorDatas[i].centralSensorSize,
                Position = translations[i].Value + centerPosition
            }, ref distanceHits))
            {
                center = distanceHits.Length;
            }

            if (physicsWorld.CalculateDistance(new PointDistanceInput()
            {
                Filter = CollisionFilter.Default,
                MaxDistance = sensorDatas[i].leftSensorSize,
                Position = translations[i].Value + leftPosition
            }, ref distanceHits))
            {
                left = distanceHits.Length;
            }

            if (physicsWorld.CalculateDistance(new PointDistanceInput()
            {
                Filter = CollisionFilter.Default,
                MaxDistance = sensorDatas[i].rightSensorSize,
                Position = translations[i].Value + rightPosition
            }, ref distanceHits))
            {
                right = distanceHits.Length;
            }
            distanceHits.Dispose();

            float angle = DeltaTime * moveDatas[i].rotSpeed;
            if (right > center && right > left)
            {
                moveDatas[i] = new MoveData()
                {
                    speed = moveDatas[i].speed,
                    rotSpeed = moveDatas[i].rotSpeed,
                    angle = moveDatas[i].angle - angle
                };
            } else if (left > right && left > center)
            {
                moveDatas[i] = new MoveData()
                {
                    speed = moveDatas[i].speed,
                    rotSpeed = moveDatas[i].rotSpeed,
                    angle = moveDatas[i].angle + angle
                };
            } else if (left == right && left > center)
            {
                if (UnityEngine.Random.value > 0.5)
                {
                    moveDatas[i] = new MoveData()
                    {
                        speed = moveDatas[i].speed,
                        rotSpeed = moveDatas[i].rotSpeed,
                        angle = moveDatas[i].angle - angle
                    };
                } else
                {
                    moveDatas[i] = new MoveData()
                    {
                        speed = moveDatas[i].speed,
                        rotSpeed = moveDatas[i].rotSpeed,
                        angle = moveDatas[i].angle + angle
                    };
                }
            }

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
        updateTranslationJob.sensorDataTypeHandle = this.
            GetComponentTypeHandle<SensorData>(true);

        updateTranslationJob.DeltaTime = World.Time.DeltaTime;
        updateTranslationJob.physicsWorld = World.
            DefaultGameObjectInjectionWorld.
            GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;

        this.Dependency = updateTranslationJob.
            ScheduleParallel(query, 1, this.Dependency);
        
        
    }
}
