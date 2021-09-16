using Unity.Entities;

[GenerateAuthoringComponent]
public struct CardEntityComponent : IComponentData
{
    public Entity entity;
    public int id;
}
