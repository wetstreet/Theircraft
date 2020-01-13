using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkComparer : IComparer<Chunk>
{
    static ChunkComparer _instance;
    public static ChunkComparer instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ChunkComparer();
            }
            return _instance;
        }
    }

    int IComparer<Chunk>.Compare(Chunk x, Chunk y)
    {
        return -PlayerController.GetChunkToFrontDot(x).CompareTo(PlayerController.GetChunkToFrontDot(y));
    }
}
