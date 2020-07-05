using protocol.cs_theircraft;
using UnityEngine;

public class BirchTreeGenerator : TreeGenerator
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
        int count = Mathf.RoundToInt(Random.Range(2, 4));
        for (int i = 0; i < count; i++)
        {
            GenerateSingleBlockLayer(CSBlockType.BirchLog);
        }
        GenerateLeavesLayer(2, CSBlockType.BirchLog, CSBlockType.BirchLeaves, CornerType.Random);
        GenerateLeavesLayer(2, CSBlockType.BirchLog, CSBlockType.BirchLeaves, CornerType.Random);
        GenerateLeavesLayer(1, CSBlockType.BirchLog, CSBlockType.BirchLeaves, CornerType.Random);
        GenerateLeavesLayer(1, CSBlockType.BirchLeaves, CSBlockType.BirchLeaves);
    }
}
