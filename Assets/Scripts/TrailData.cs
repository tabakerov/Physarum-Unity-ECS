    using Unity.Entities;

    [GenerateAuthoringComponent]
    public struct TrailData : IComponentData
    {
        public float spawnRate;
        public float lastSpawnedTime;
        public Entity particleEntity;
    }
