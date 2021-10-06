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
    protected int plantIndex;
    public virtual int GetPlantIndexByData(NBTChunk chunk, Vector3Int pos, int data) { return GetPlantIndexByData(chunk, data); }

    public virtual int GetPlantIndexByData(NBTChunk chunk, int data) { return GetPlantIndexByData(data); }

    public virtual int GetPlantIndexByData(int data) { return 0; }

    public override float breakNeedTime { get { return 0; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override bool isTransparent { get { return true; } }

    public override bool isCollidable { get { return false; } }

    public override bool hasDropItem { get { return false; } }


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
        }
        plantIndex = GetPlantIndexByData(chunk, pos, blockData);
        tintColor = GetTintColorByData(chunk, pos, blockData);

        float skyLight = chunk.GetSkyLight(pos.x, pos.y, pos.z);

        AddDiagonalFace(nbtGO.nbtMesh, pos, skyLight);
        AddAntiDiagonalFace(nbtGO.nbtMesh, pos, skyLight);
    }

    protected Color tintColor;

    protected virtual Color GetTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return GetTintColorByData(chunk, data); }
    protected virtual Color GetTintColorByData(NBTChunk chunk, byte data) { return Color.white; }

    public override Color GetFrontTintColorByData(NBTChunk chunk, Vector3Int pos, byte data)
    {
        return GetTintColorByData(chunk, pos, data);
    }

    protected void AddPlantFace(NBTMesh mesh, Vector3Int pos,
        Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4,
        int faceIndex, Color color, float skyLight, Vector3 normal)
    {
        ushort startIndex = mesh.vertexCount;

        SetVertex(mesh, pos1 + pos, faceIndex, corners.bottomLeftUV, skyLight, color, normal);
        SetVertex(mesh, pos2 + pos, faceIndex, corners.topLeftUV, skyLight, color, normal);
        SetVertex(mesh, pos3 + pos, faceIndex, corners.topRightUV, skyLight, color, normal);
        SetVertex(mesh, pos4 + pos, faceIndex, corners.bottomRightUV, skyLight, color, normal);

        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 1);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 3);
    }

    void AddDiagonalFace(NBTMesh mesh, Vector3Int pos, float skyLight)
    {
        AddPlantFace(mesh, pos, corners.farBottomLeft, corners.farTopLeft, corners.nearTopRight, corners.nearBottomRight, plantIndex, tintColor, skyLight, Vector3.zero);
    }

    void AddAntiDiagonalFace(NBTMesh mesh, Vector3Int pos, float skyLight)
    {
        AddPlantFace(mesh, pos, corners.nearBottomLeft, corners.nearTopLeft, corners.farTopRight, corners.farBottomRight, plantIndex, tintColor, skyLight, Vector3.zero);
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

    public override Mesh GetItemMesh(NBTChunk chunk, byte data)
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
