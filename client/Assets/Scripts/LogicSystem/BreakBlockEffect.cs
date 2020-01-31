using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class BreakBlockEffect : MonoBehaviour
{
    static Dictionary<CSBlockType, Vector2Int> type2breakEffect = new Dictionary<CSBlockType, Vector2Int>
    {
        {CSBlockType.GrassBlock, ChunkMeshGenerator.uv_dirt },
        {CSBlockType.Dirt, ChunkMeshGenerator.uv_dirt },
        {CSBlockType.Tnt, ChunkMeshGenerator.uv_tnt_side },
        {CSBlockType.Brick, ChunkMeshGenerator.uv_bricks },
        {CSBlockType.Furnace, ChunkMeshGenerator.uv_furnace_side },
        {CSBlockType.HayBlock, ChunkMeshGenerator.hay_side },
    };

    public static void Create(CSBlockType type, int x, int y, int z)
    {
        GameObject prefab = Resources.Load("Prefabs/BreakBlockEffect") as GameObject;
        GameObject go = Instantiate(prefab);
        go.transform.localPosition = new Vector3(x, y, z);
        Destroy(go, 1);

        BreakBlockEffect effect = go.AddComponent<BreakBlockEffect>();
        Vector2Int uv = Vector2Int.zero;
        if (type2breakEffect.ContainsKey(type))
        {
            uv = type2breakEffect[type];
        }
        else
        {
            uv = ChunkMeshGenerator.type2texcoords[(byte)type].front;
        }
        effect.x = uv.x;
        effect.y = uv.y;
    }

    Texture2D effectTexture;
    public int x = 0;
    public int y = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (effectTexture == null)
        {
            effectTexture = new Texture2D(16, 16);
            effectTexture.filterMode = FilterMode.Point;
        }
        Texture2D blockTexture = Resources.Load<Material>("Materials/block").mainTexture as Texture2D;

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                Color c = blockTexture.GetPixel(i + x * 16, 927 - y * 16 - j);
                effectTexture.SetPixel(i, 15 - j, c);
            }
        }
        effectTexture.Apply();
        GetComponent<Renderer>().material.mainTexture = effectTexture;
    }
}
