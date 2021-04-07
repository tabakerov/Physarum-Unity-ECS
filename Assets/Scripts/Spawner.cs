using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Material = UnityEngine.Material;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public int actors;

    public int boxes;
    
    [SerializeField] private Mesh unitMesh;
    [SerializeField] private Material unitMaterial;

    public GameObject gameObjectPrefab;
    [SerializeField] private Mesh particleMesh;
    [SerializeField] private Material particleMaterial;
    
    private Entity entityPrefab;
    private World defaultWorld;
    private EntityManager entityManager;

    void MakeActor()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype archetype = entityManager.CreateArchetype(
                        typeof(Translation),
                        typeof(Rotation),
                        typeof(RenderMesh),
                        typeof(RenderBounds),
                        typeof(LocalToWorld),
                        typeof(MoveData),
                        typeof(SensorData));
        Entity entity = entityManager.CreateEntity(archetype);

        entityManager.AddComponentData(entity,
                        new Translation()
                        {
                                        Value = new float3(Random.Range(0f, 100f), 0f, Random.Range(0f, 100f))
                        }
        );
        entityManager.AddComponentData(entity,
                        new MoveData()
                        {
                                        angle = Random.Range(-2 * Mathf.PI, 2 * Mathf.PI),
                                        rotSpeed = Random.Range(1.1f, 1.3f),
                                        speed = Random.Range(1.1f, 2f)
                        });
        entityManager.AddComponentData(entity,
                        new SensorData()
                        {
                                        centralSensorAngle = 0,
                                        centralSensorDistance = 3f,
                                        centralSensorSize = 1,
                                        leftSensorAngle = 1f,
                                        leftSensorDistance = 3f,
                                        leftSensorSize = 1f,
                                        rightSensorAngle = -1f,
                                        rightSensorDistance = 3f,
                                        rightSensorSize = 1f
                        });
        entityManager.AddSharedComponentData(entity, new RenderMesh
        {
                        mesh = unitMesh,
                        material = unitMaterial
        });
    }
    
    void MakeParticle()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype archetype = entityManager.CreateArchetype(
                        typeof(Translation),
                        typeof(Rotation),
                        typeof(RenderMesh),
                        typeof(RenderBounds),
                        typeof(LocalToWorld),
                        typeof(PhysicsCollider));
        Entity entity = entityManager.CreateEntity(archetype);

        entityManager.AddComponentData(entity,
                        new Translation()
                        {
                                        Value = new float3(Random.Range(0f, 100f), 0f, Random.Range(0f, 100f))
                        }
        );
        var collider = new PhysicsCollider();
        
        
        entityManager.AddSharedComponentData(entity, new RenderMesh
        {
                        mesh = unitMesh,
                        material = unitMaterial
        });
    }
    
    void Start()
    {
        defaultWorld = World.DefaultGameObjectInjectionWorld;
        entityManager = defaultWorld.EntityManager;

        // generate Entity Prefab
        if (gameObjectPrefab != null)
        {
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
            entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObjectPrefab, settings);

            
            for (int i = 0; i < boxes; i++)
            {
                InstantiateEntity(new float3(Random.Range(0f, 100f), 0f, Random.Range(0f, 100f)));
            }
        }
        
        for (int i = 0; i < actors; i++)
        {
            MakeActor();
        }
        
    }
    
    private void InstantiateEntity(float3 position)
    {
        if (entityManager == null)
        {
            Debug.LogWarning("InstantiateEntity WARNING: No EntityManager found!");
            return;
        }

        Entity myEntity = entityManager.Instantiate(entityPrefab);
        entityManager.SetComponentData(myEntity, new Translation
        {
                        Value = position
        });
    }

    private void ConvertToEntity(float3 position)
    {
        if (entityManager == null)
        {
            Debug.LogWarning("ConvertToEntity WARNING: No EntityManager found!");
            return;
        }

        if (gameObjectPrefab == null)
        {
            Debug.LogWarning("ConvertToEntity WARNING: Missing GameObject Prefab");
            return;
        }

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
        entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObjectPrefab, settings);

        Entity myEntity = entityManager.Instantiate(entityPrefab);
        entityManager.SetComponentData(myEntity, new Translation
        {
                        Value = position
        });
    }
}
