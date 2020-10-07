using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkComparer : IComparer<NBTChunk>
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

    int IComparer<NBTChunk>.Compare(NBTChunk a, NBTChunk b)
    {
        bool isNearByA = PlayerController.IsNearByChunk(a);
        bool isNearByB = PlayerController.IsNearByChunk(b);
        if (isNearByA && !isNearByB)
        {
            return -1;
        }
        else if (isNearByB && !isNearByA)
        {
            return 1;
        }
        else
        {
            float dotA = PlayerController.GetChunkToFrontDot(a);
            float dotB = PlayerController.GetChunkToFrontDot(b);
            return -dotA.CompareTo(dotB);
        }
    }
}
