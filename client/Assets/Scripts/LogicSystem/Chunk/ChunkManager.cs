using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChunkManager
{
    public async static void ChunksEnterLeaveView(List<Vector2Int> enterViewChunks, List<Vector2Int> leaveViewChunks = null)
    {
        List<NBTChunk> chunks = new List<NBTChunk>();
        foreach (Vector2Int chunkPos in enterViewChunks)
        {
            NBTChunk chunk = await NBTHelper.LoadChunkAsync(chunkPos.x, chunkPos.y);
            chunks.Add(chunk);
        }
        ChunkRefresher.Add(chunks);
        ChunkChecker.FinishRefresh();

        if (leaveViewChunks != null)
        {
            foreach (Vector2Int chunk in leaveViewChunks)
            {
                NBTHelper.RemoveChunk(chunk.x, chunk.y);
            }
        }
    }

    public static void PreloadChunks(List<Vector2Int> enterViewChunks)
    {
        foreach (Vector2Int chunkPos in enterViewChunks)
        {
            NBTChunk chunk = NBTHelper.LoadChunk(chunkPos.x, chunkPos.y);
            ChunkRefresher.Add(chunk);
        }
        ChunkRefresher.ForceRefreshAll();
    }
}
