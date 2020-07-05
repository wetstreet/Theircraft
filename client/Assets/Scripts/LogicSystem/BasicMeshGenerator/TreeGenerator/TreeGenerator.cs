using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CornerType
{
    None,
    Random
}

public abstract class TreeGenerator
{
    protected static int x;
    protected static int y;
    protected static int z;
    protected static HashSet<Chunk> chunks = new HashSet<Chunk>();

    protected static void GenerateLeavesLayer(int radius, CSBlockType centerType, CSBlockType leavesType, CornerType cornerType = CornerType.None)
    {
        for (int i = x - radius; i <= x + radius; i++)
        {
            for (int j = z - radius; j <= z + radius; j++)
            {
                if ((i == x - radius && j == z - radius) ||
                    (i == x - radius && j == z + radius) ||
                    (i == x + radius && j == z - radius) ||
                    (i == x + radius && j == z + radius))
                {
                    if (cornerType == CornerType.Random && Random.value > 0.5)
                    {
                        Chunk chunk = ChunkManager.GetChunk(i, y, j);
                        chunk.SetBlockTypeByGlobalPosition(i, y, j, leavesType);
                        if (!chunks.Contains(chunk))
                        {
                            chunks.Add(chunk);
                        }
                    }
                }
                else
                {
                    Chunk chunk = ChunkManager.GetChunk(i, y, j);
                    chunk.SetBlockTypeByGlobalPosition(i, y, j, i == x && j == z ? centerType : leavesType);
                    if (!chunks.Contains(chunk))
                    {
                        chunks.Add(chunk);
                    }
                }
            }
        }
        y++;
    }

    protected static void GenerateSingleBlockLayer(CSBlockType type)
    {
        Chunk chunk = ChunkManager.GetChunk(x, y, z);
        chunk.SetBlockTypeByGlobalPosition(x, y, z, type);
        if (!chunks.Contains(chunk))
        {
            chunks.Add(chunk);
        }
        y++;
    }

    protected static void Init(int _x, int _y, int _z)
    {
        chunks.Clear();
        x = _x;
        y = _y;
        z = _z;
    }

    protected static void RebuildChunkMeshes()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.RebuildMesh();
        }
    }
}
