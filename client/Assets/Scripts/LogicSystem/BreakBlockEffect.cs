using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class BreakBlockEffect : MonoBehaviour
{
    // special break effects (default uses front uv)
    static Dictionary<CSBlockType, Vector2Int> type2breakEffect = new Dictionary<CSBlockType, Vector2Int>
    {
        {CSBlockType.GrassBlock, ChunkMeshGenerator.uv_dirt },
    };

    public static void Create(CSBlockType type, Vector3 pos)
    {
        GameObject prefab = Resources.Load("Prefabs/BreakBlockEffect") as GameObject;
        GameObject go = Instantiate(prefab);
        go.transform.localPosition = pos;
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
