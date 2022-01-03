using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlantCorners
{
    public Vector3 nearBottomLeft;
    public Vector3 nearBottomRight;
    public Vector3 nearTopLeft;
    public Vector3 nearTopRight;
    public Vector3 farBottomLeft;
    public Vector3 farBottomRight;
    public Vector3 farTopLeft;
    public Vector3 farTopRight;
    public Vector2 bottomLeftUV;
    public Vector2 bottomRightUV;
    public Vector2 topLeftUV;
    public Vector2 topRightUV;
}

public class NBTPlant : NBTBlock
{
    public virtual int GetPlantIndexByData(NBTChunk chunk, Vector3Int pos, int data) { return GetPlantIndexByData(chunk, data); }

    public virtual int GetPlantIndexByData(NBTChunk chunk, int data) { return GetPlantIndexByData(data); }

    public virtual int GetPlantIndexByData(int data) { return 0; }

    public override float hardness => 0;

    public override SoundMaterial soundMaterial => SoundMaterial.Grass;

    public override bool isTransparent => true;

    public override bool isCollidable => false;

    public override bool hasDropItem => false;
    public override Vector3 itemSize => Vector3.one;


    protected PlantCorners corners;
    protected virtual int size => 8;
    protected virtual int height => 16;

    public override void Init()
    {
        float unit = size * 0.0625f;
        float top = -0.5f + height * 0.0625f;
        corners = new PlantCorners
        {
            nearBottomLeft = new Vector3(-unit, -0.5f, -unit),
            nearBottomRight = new Vector3(unit, -0.5f, -unit),
            nearTopLeft = new Vector3(-unit, top, -unit),
            nearTopRight = new Vector3(unit, top, -unit),
            farBottomLeft = new Vector3(-unit, -0.5f, unit),
            farBottomRight = new Vector3(unit, -0.5f, unit),
            farTopLeft = new Vector3(-unit, top, unit),
            farTopRight = new Vector3(unit, top, unit),
            bottomLeftUV = new Vector2(0.5f - size * 0.0625f, 0),
            bottomRightUV = new Vector2(0.5f + size * 0.0625f, 0),
            topLeftUV = new Vector2(0.5f - size * 0.0625f, height * 0.0625f),
            topRightUV = new Vector2(0.5f + size * 0.0625f, height * 0.0625f),
        };
        diagonalFace = new Vector3[] { corners.farBottomLeft, corners.farTopLeft, corners.nearTopRight, corners.nearBottomRight };
        antiDiagonalFace = new Vector3[] { corners.nearBottomLeft, corners.nearTopLeft, corners.farTopRight, corners.farBottomRight };
        uv_plant = new Vector2[] { corners.bottomLeftUV, corners.topLeftUV, corners.topRightUV, corners.bottomRightUV };
    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
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

    Vector2[] uv_plant;

    Vector3[] diagonalFace;
    Vector3[] antiDiagonalFace;

    FaceAttributes diagonalFA = new FaceAttributes()
    {
        skyLight = new float[4],
        blockLight = new float[4],
    };
    void AddDiagonalFace(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        diagonalFA.pos = diagonalFace;
        diagonalFA.faceIndex = GetPlantIndexByData(chunk, ca.pos, ca.blockData);
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
        diagonalFA.uv = uv_plant;

        AddFace(mesh, diagonalFA, ca);
    }

    FaceAttributes antidiagonalFA = new FaceAttributes()
    {
        skyLight = new float[4],
        blockLight = new float[4],
    };
    void AddAntiDiagonalFace(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        antidiagonalFA.pos = antiDiagonalFace;
        antidiagonalFA.faceIndex = GetPlantIndexByData(chunk, ca.pos, ca.blockData);
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
        antidiagonalFA.uv = uv_plant;

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
