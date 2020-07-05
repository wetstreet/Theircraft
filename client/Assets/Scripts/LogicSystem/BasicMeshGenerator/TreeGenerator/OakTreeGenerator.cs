using protocol.cs_theircraft;
using UnityEngine;

public class OakTreeGenerator : TreeGenerator
{
    public static void Generate(int _x, int _y, int _z)
    {
        Init(_x, _y, _z);

        Pattern1();

        RebuildChunkMeshes();
    }

    static void Pattern1()
    {
        // from bottom to top
        int count = Mathf.RoundToInt(Random.Range(1, 3));
        for (int i = 0; i < count; i++)
        {
            GenerateSingleBlockLayer(CSBlockType.OakLog);
        }
        GenerateLeavesLayer(2, CSBlockType.OakLog, CSBlockType.OakLeaves, CornerType.Random);
        GenerateLeavesLayer(2, CSBlockType.OakLog, CSBlockType.OakLeaves, CornerType.Random);
        GenerateLeavesLayer(1, CSBlockType.OakLog, CSBlockType.OakLeaves, CornerType.Random);
        GenerateLeavesLayer(1, CSBlockType.OakLeaves, CSBlockType.OakLeaves);
    }

    static void Pattern2()
    {
        // from bottom to top
        GenerateSingleBlockLayer(CSBlockType.OakLog);
        GenerateSingleBlockLayer(CSBlockType.OakLog);
        GenerateLeavesLayer(1, CSBlockType.OakLog, CSBlockType.OakLeaves);
        GenerateLeavesLayer(2, CSBlockType.OakLog, CSBlockType.OakLeaves);
        GenerateLeavesLayer(2, CSBlockType.OakLog, CSBlockType.OakLeaves);
        GenerateLeavesLayer(2, CSBlockType.OakLeaves, CSBlockType.OakLeaves);
        GenerateLeavesLayer(1, CSBlockType.OakLeaves, CSBlockType.OakLeaves);
    }
}
