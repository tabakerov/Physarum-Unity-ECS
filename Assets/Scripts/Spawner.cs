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

    public GameObject actorPrefab;
    public Entity actorEntity;
    public GameObject particlePrefab;
    public Entity particleEntity;
    
   
    private Entity entityPrefab;
    private World defaultWorld;
    private EntityManager entityManager;

    public float trailSpawnRate;
    public float trailLifetime;

    void MakeActor()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        Entity entity = entityManager.Instantiate(actorEntity);

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
        entityManager.AddComponentData(entity,
                        new TrailData()
                        {
                                        spawnRate = trailSpawnRate,
                                        particleEntity = particleEntity,
                                        lastSpawnedTime = 0f
                        });
    }

    
    void Start()
    {
        defaultWorld = World.DefaultGameObjectInjectionWorld;
        entityManager = defaultWorld.EntityManager;

        if (particlePrefab != null)
        {
            particleEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                            particlePrefab,
                            GameObjectConversionSettings.FromWorld(defaultWorld, null)
            );
        }

        if (actorPrefab != null)
        {
            actorEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                            actorPrefab,
                            GameObjectConversionSettings.FromWorld(defaultWorld, null));
        }
        
        for (int i = 0; i < actors; i++)
        {
            MakeActor();
        }
        
    }

}
