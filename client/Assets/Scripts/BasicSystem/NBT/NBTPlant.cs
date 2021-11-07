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

    public override float breakNeedTime { get { return 0; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override bool isTransparent { get { return true; } }

    public override bool isCollidable { get { return false; } }

    public override bool hasDropItem { get { return false; } }
    public override Vector3 itemSize => Vector3.one;


    protected PlantCorners corners;
    protected virtual int size { get { return 8; } }
    protected virtual int height { get { return 16; } }


    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        if (corners == null)
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
        CubeAttributes ca = new CubeAttributes();
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

    void AddDiagonalFace(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.pos = diagonalFace;
        fa.faceIndex = GetPlantIndexByData(chunk, ca.pos, ca.blockData);
        fa.color = GetTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.zero;
        fa.uv = uv_plant;

        AddFace(mesh, fa, ca);
    }

    void AddAntiDiagonalFace(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.pos = antiDiagonalFace;
        fa.faceIndex = GetPlantIndexByData(chunk, ca.pos, ca.blockData);
        fa.color = GetTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.zero;
        fa.uv = uv_plant;

        AddFace(mesh, fa, ca);
    }
    
    public override string pathPrefix { get { return "GUI/block/"; } }

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
