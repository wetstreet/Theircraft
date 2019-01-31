using System;
using Unity.Entities;

[Serializable]
public struct Chunk : IComponentData
{
    public float x;
    public float z;
}

public class ChunkComponent : ComponentDataWrapper<Chunk> { }
