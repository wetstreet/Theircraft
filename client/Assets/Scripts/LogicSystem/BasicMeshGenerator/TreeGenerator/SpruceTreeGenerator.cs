using protocol.cs_theircraft;
using UnityEngine;

public class SpruceTreeGenerator : TreeGenerator
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
        GenerateSingleBlockLayer(CSBlockType.SpruceLog);
        GenerateSingleBlockLayer(CSBlockType.SpruceLog);
        GenerateLeavesLayer(3, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(2, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(1, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(2, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(1, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateSingleBlockLayer(CSBlockType.SpruceLog);
        GenerateLeavesLayer(1, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateSingleBlockLayer(CSBlockType.SpruceLeaves);
    }

    static void Pattern2()
    {
        // from bottom to top
        GenerateSingleBlockLayer(CSBlockType.SpruceLog);
        GenerateLeavesLayer(2, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(1, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(2, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(1, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(2, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(1, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateSingleBlockLayer(CSBlockType.SpruceLog);
        GenerateLeavesLayer(1, CSBlockType.SpruceLeaves, CSBlockType.SpruceLeaves);
        GenerateSingleBlockLayer(CSBlockType.SpruceLeaves);
    }

    static void Pattern3()
    {
        // from bottom to top
        GenerateSingleBlockLayer(CSBlockType.SpruceLog);
        GenerateSingleBlockLayer(CSBlockType.SpruceLog);
        GenerateLeavesLayer(2, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(1, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateSingleBlockLayer(CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(1, CSBlockType.SpruceLeaves, CSBlockType.SpruceLeaves);
        GenerateSingleBlockLayer(CSBlockType.SpruceLeaves);
    }

    static void Pattern4()
    {
        // from bottom to top
        GenerateSingleBlockLayer(CSBlockType.SpruceLog);
        GenerateSingleBlockLayer(CSBlockType.SpruceLog);
        GenerateLeavesLayer(1, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(2, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(1, CSBlockType.SpruceLog, CSBlockType.SpruceLeaves);
        GenerateSingleBlockLayer(CSBlockType.SpruceLeaves);
        GenerateLeavesLayer(1, CSBlockType.SpruceLeaves, CSBlockType.SpruceLeaves);
        GenerateSingleBlockLayer(CSBlockType.SpruceLeaves);
    }
}
