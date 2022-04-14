using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class NBTPlant : NBTBlock
{
    public virtual string GetTexName(NBTChunk chunk, Vector3Int pos, int data) { return ""; }

    public override float hardness => 0;

    public override SoundMaterial soundMaterial => SoundMaterial.Grass;

    public override bool isTransparent => true;

    public override bool isCollidable => false;

    public override bool hasDropItem => false;
    public override Vector3 itemSize => Vector3.one;

    protected virtual int size => 8;
    protected virtual int height => 16;

    public override void Init()
    {
        float unit = 0.0625f;
        float corner = size * unit;
        float top = -0.5f + height * unit;

        nearBottomLeft = new Vector3(-corner, -0.5f, -corner);
        nearBottomRight = new Vector3(corner, -0.5f, -corner);
        nearTopLeft = new Vector3(-corner, top, -corner);
        nearTopRight = new Vector3(corner, top, -corner);
        farBottomLeft = new Vector3(-corner, -0.5f, corner);
        farBottomRight = new Vector3(corner, -0.5f, corner);
        farTopLeft = new Vector3(-corner, top, corner);
        farTopRight = new Vector3(corner, top, corner);

        diagonalFace = new Vector3[] { farBottomLeft, farTopLeft, nearTopRight, nearBottomRight };
        antiDiagonalFace = new Vector3[] { nearBottomLeft, nearTopLeft, farTopRight, farBottomRight };
    }

    static float epsilon = 0.001f;

    Dictionary<string, Vector2[]> uv_dict = new Dictionary<string, Vector2[]>();
    protected Vector2[] GetUV(NBTChunk chunk, Vector3Int pos, int data)
    {
        string name = GetTexName(chunk, pos, data);
        if (!uv_dict.ContainsKey(name))
        {
            Rect rect = TextureArrayManager.GetRectByName(name);

            float left = rect.center.x - size * rect.width / 16;
            float right = rect.center.x + size * rect.width / 16;
            float bottom = rect.yMin;
            float top = rect.yMin + height * rect.height / 16;

            // prevent uv bleeding
            if (height == 16)
                top -= epsilon;

            if (size == 8)
            {
                left += epsilon;
                right -= epsilon;
            }

            Vector2[] uv_plant = new Vector2[]
            {
                new Vector2(left, bottom),
                new Vector2(left, top),
                new Vector2(right, top),
                new Vector2(right, bottom),
            };
            uv_dict.Add(name, uv_plant);
        }
        return uv_dict[name];
    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = chunk.ca;
        ca.pos = pos;
        ca.blockData = blockData;

        AddDiagonalFace(chunk, nbtGO.nbtMesh, ca);
        AddAntiDiagonalFace(chunk, nbtGO.nbtMesh, ca);
    }

    protected virtual Color GetTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return GetTintColorByData(chunk, data); }
    protected virtual Color GetTintColorByData(NBTChunk chunk, byte data) { return Color.white; }

    public override Color GetFrontTintColorByData(NBTChunk chunk, Vector3Int pos, byte data)
    {
        return GetTintColorByData(chunk, pos, data);
    }

    Vector3[] diagonalFace;
    Vector3[] antiDiagonalFace;

    FaceAttributes diagonalFA = new FaceAttributes()
    {
        skyLight = new float[4],
        blockLight = new float[4],
    };
    protected void AddDiagonalFace(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        diagonalFA.pos = diagonalFace;
        diagonalFA.color = GetTintColorByData(chunk, ca.pos, ca.blockData);
        diagonalFA.skyLight[0] = skyLight;
        diagonalFA.skyLight[1] = skyLight;
        diagonalFA.skyLight[2] = skyLight;
        diagonalFA.skyLight[3] = skyLight;
        diagonalFA.blockLight[0] = blockLight;
        diagonalFA.blockLight[1] = blockLight;
        diagonalFA.blockLight[2] = blockLight;
        diagonalFA.blockLight[3] = blockLight;
        diagonalFA.normal = Vector3.zero;
        diagonalFA.uv = GetUV(chunk, ca.pos, ca.blockData);

        AddFace(mesh, diagonalFA, ca);
    }

    FaceAttributes antidiagonalFA = new FaceAttributes()
    {
        skyLight = new float[4],
        blockLight = new float[4],
    };
    protected void AddAntiDiagonalFace(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        antidiagonalFA.pos = antiDiagonalFace;
        antidiagonalFA.color = GetTintColorByData(chunk, ca.pos, ca.blockData);
        antidiagonalFA.skyLight[0] = skyLight;
        antidiagonalFA.skyLight[1] = skyLight;
        antidiagonalFA.skyLight[2] = skyLight;
        antidiagonalFA.skyLight[3] = skyLight;
        antidiagonalFA.blockLight[0] = blockLight;
        antidiagonalFA.blockLight[1] = blockLight;
        antidiagonalFA.blockLight[2] = blockLight;
        antidiagonalFA.blockLight[3] = blockLight;
        antidiagonalFA.normal = Vector3.zero;
        antidiagonalFA.uv = GetUV(chunk, ca.pos, ca.blockData);

        AddFace(mesh, antidiagonalFA, ca);
    }
    
    public override string pathPrefix => "GUI/block/";

    public override Material GetItemMaterial(byte data)
    {
        if (!itemMaterialDict.ContainsKey(data))
        {
            Material mat = new Material(Shader.Find("Custom/BlockShader"));
            Texture2D tex = Resources.Load<Texture2D>(pathPrefix + GetIconPathByData(data));
            mat.mainTexture = tex;
            itemMaterialDict.Add(data, mat);
        }
        return itemMaterialDict[data];
    }

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte data)
    {
        byte index = (byte)(data % 4);
        if (!itemMeshDict.ContainsKey(index))
        {
            Texture2D tex = Resources.Load<Texture2D>(pathPrefix + GetIconPathByData(index));
            Mesh mesh = ItemMeshGenerator.instance.Generate(tex);
            itemMeshDict.Add(index, mesh);
        }
        return itemMeshDict[index];
    }
}
