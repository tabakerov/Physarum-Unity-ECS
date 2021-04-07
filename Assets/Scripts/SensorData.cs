using Unity.Entities;


[GenerateAuthoringComponent]
public struct SensorData : IComponentData
{
    public float leftSensorDistance;
    public float leftSensorSize;
    public float leftSensorAngle;

    public float centralSensorDistance;
    public float centralSensorSize;
    public float centralSensorAngle;

    public float rightSensorDistance;
    public float rightSensorSize;
    public float rightSensorAngle;
}
