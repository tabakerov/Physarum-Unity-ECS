using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using Collider = Unity.Physics.Collider;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public EntityManager entityManager;
    public World defaultWorld;
    public RenderMesh renderMesh;

    public Entity CreateBodyWithCollider(
                    RenderMesh displayMesh, float3 position, quaternion orientation,
                    BlobAssetReference<Collider> collider,
                    float3 linearVelocity, float3 angularVelocity, float mass)
    {
        ComponentType[] componentTypes = new ComponentType[8];

        componentTypes[0] = typeof(RenderMesh);
        componentTypes[1] = typeof(RenderBounds);
        componentTypes[2] = typeof(Translation);
        componentTypes[3] = typeof(Rotation);
        componentTypes[4] = typeof(LocalToWorld);
        componentTypes[5] = typeof(PhysicsCollider);
        componentTypes[6] = typeof(MoveData);
        componentTypes[7] = typeof(TrailData);

        Entity entity = entityManager.CreateEntity(componentTypes);

        entityManager.SetSharedComponentData(entity, displayMesh);
        entityManager.SetComponentData(entity, new RenderBounds {Value = displayMesh.mesh.bounds.ToAABB()});

        entityManager.SetComponentData(entity, new Translation {Value = position});
        entityManager.SetComponentData(entity, new Rotation {Value = orientation});
        entityManager.SetComponentData(entity, new TrailData() {lastSpawnedTime = 0f, spawnRate = 1f});
        entityManager.SetComponentData(entity, new PhysicsCollider {Value = collider});
        entityManager.SetComponentData(entity, new MoveData(){
                            //angle = Random.Range(0f, 4*Mathf.PI),
                            angle = 0f,
                            rotSpeed = 2f,
                            speed = 0f
                            //speed = Random.Range(5f, 7f)
                        });
        
        return entity;
    }
    
    public Entity CreateBodyWithoutCollider(
                    RenderMesh displayMesh, float3 position, quaternion orientation,
                    float3 linearVelocity, float3 angularVelocity, float mass)
    {
        ComponentType[] componentTypes = new ComponentType[8];

        componentTypes[0] = typeof(RenderMesh);
        componentTypes[1] = typeof(RenderBounds);
        componentTypes[2] = typeof(Translation);
        componentTypes[3] = typeof(Rotation);
        componentTypes[4] = typeof(LocalToWorld);
        componentTypes[5] = typeof(MoveData);
        componentTypes[6] = typeof(TrailData);
        componentTypes[7] = typeof(SensorData);

        Entity entity = entityManager.CreateEntity(componentTypes);

        entityManager.SetSharedComponentData(entity, displayMesh);
        entityManager.SetComponentData(entity, new RenderBounds {Value = displayMesh.mesh.bounds.ToAABB()});

        entityManager.SetComponentData(entity, new Translation {Value = position});
        entityManager.SetComponentData(entity, new Rotation {Value = orientation});
        entityManager.SetComponentData(entity, new TrailData() {lastSpawnedTime = 0f, spawnRate = 0.2f});
        entityManager.SetComponentData(entity, new MoveData(){
                            angle = Random.Range(0f, 4*Mathf.PI),
                            //angle = 0f,
                            rotSpeed = 2f,
                            //speed = 1f
                            speed = Random.Range(15f, 17f)
                        });
        entityManager.SetComponentData(entity, new SensorData()
        {
                        centralSensorAngle = 0f,
                        centralSensorDistance = 2f,
                        centralSensorSize = 1f,
                        leftSensorAngle = 1f,
                        leftSensorDistance = 2f,
                        leftSensorSize = 1f,
                        rightSensorAngle = -1f,
                        rightSensorDistance = 2f,
                        rightSensorSize = 1f
        });
        return entity;
    }

    public Entity CreateDynamicSphereWithCollider(RenderMesh displayMesh, float radius, float3 position, quaternion orientation)
    {
        // Sphere with default filter and material. Add to Create() call if you want non default:
        BlobAssetReference<Unity.Physics.Collider> spCollider =
                        Unity.Physics.SphereCollider.Create(new SphereGeometry());
        return CreateBodyWithCollider(displayMesh, position, orientation, spCollider, float3.zero, float3.zero, 1.0f);
    }

    
    public Entity CreateDynamicSphereWithoutCollider(RenderMesh displayMesh, float radius, float3 position, quaternion orientation)
    {
        return CreateBodyWithoutCollider(displayMesh, position, orientation, float3.zero, float3.zero, 1.0f);
    }


    void Start()
    {
        defaultWorld = World.DefaultGameObjectInjectionWorld;
        entityManager = defaultWorld.EntityManager;

        //var e = CreateDynamicSphereWithoutCollider(renderMesh, 3, float3.zero, quaternion.identity);
        //var r = CreateDynamicSphereWithCollider(renderMesh, 3, 
        //                new float3(5f, 0f, -2f),
        //                quaternion.identity);

        
        for (int i = 0; i < 200; i++)
        {
            var e = CreateDynamicSphereWithoutCollider(renderMesh, 3, new float3(
                                Random.Range(0f, 100f),
                                    0f,
                                Random.Range(0f, 100f)
            ), quaternion.identity);
        }
        
        //entityManager.CompleteAllJobs();
    }
}