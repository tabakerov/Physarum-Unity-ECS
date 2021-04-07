
using Unity.Entities;

[GenerateAuthoringComponent]
public struct MoveData : IComponentData
{
    public float speed;
    public float angle;
    public float rotSpeed;
}
