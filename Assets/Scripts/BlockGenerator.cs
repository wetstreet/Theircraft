using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public enum BlockType
{
    Grass,
    Tnt
}

public struct BlockTexture
{
    public string TopTexture, BottomTexture, LeftTexture, RightTexture, FrontTexture, BackTexture;
    BlockTexture(string topTexture, string bottomTexture, string leftTexture, string rightTexture, string frontTexture, string backTexture)
    {
        TopTexture = topTexture;
        BottomTexture = bottomTexture;
        LeftTexture = leftTexture;
        RightTexture = rightTexture;
        FrontTexture = frontTexture;
        BackTexture = backTexture;
    }
    //类型对应贴图，后续改成xml
    public static Dictionary<BlockType, BlockTexture> type2texture = new Dictionary<BlockType, BlockTexture>
    {
        {BlockType.Grass, new BlockTexture("grass_top","dirt","grass_side","grass_side","grass_side","grass_side") },
        {BlockType.Tnt, new BlockTexture("tnt_top","tnt_bottom","tnt_side","tnt_side","tnt_side","tnt_side") },
    };
}


public static class BlockGenerator {


    enum FaceType
    {
        TopFace,
        BottomFace,
        LeftFace,
        RightFace,
        FrontFace,
        BackFace
    }

    static Vector3 near_left_bottom = new Vector3(-0.5f, -0.5f, -0.5f);
    static Vector3 near_right_bottom = new Vector3(0.5f, -0.5f, -0.5f);
    static Vector3 near_right_top = new Vector3(0.5f, 0.5f, -0.5f);
    static Vector3 near_left_top = new Vector3(-0.5f, 0.5f, -0.5f);
    static Vector3 far_left_bottom = new Vector3(-0.5f, -0.5f, 0.5f);
    static Vector3 far_right_bottom = new Vector3(0.5f, -0.5f, 0.5f);
    static Vector3 far_right_top = new Vector3(0.5f, 0.5f, 0.5f);
    static Vector3 far_left_top = new Vector3(-0.5f, 0.5f, 0.5f);

    static List<Vector3> vertex = new List<Vector3>();
    static List<Vector2> uv = new List<Vector2>();
    static List<int> triangles = new List<int>();

    static Dictionary<string, Texture2D> path2texture = new Dictionary<string, Texture2D>();
    static Dictionary<BlockType, MeshAndMaterial> blockType2MeshAndMaterial = new Dictionary<BlockType, MeshAndMaterial>();

    struct MeshAndMaterial
    {
        public Mesh mesh;
        public Material material;
        public MeshAndMaterial(Mesh _mesh, Material _material)
        {
            mesh = _mesh;
            material = _material;
        }
    }

    static void AddFace(FaceType faceType, Rect rect)
    {
        Vector2 uv_left_bottom = new Vector2(rect.x, rect.y);
        Vector2 uv_right_bottom = new Vector2(rect.x + rect.width, rect.y);
        Vector2 uv_right_top = new Vector2(rect.x + rect.width, rect.y + rect.height);
        Vector2 uv_left_top = new Vector2(rect.x, rect.y + rect.height);
        uv.AddRange(new Vector2[] { uv_left_bottom, uv_right_bottom, uv_right_top, uv_left_top });

        triangles.AddRange(new int[] { vertex.Count + 0, vertex.Count + 2, vertex.Count + 1, vertex.Count + 0, vertex.Count + 3, vertex.Count + 2 });
        switch (faceType)
        {
            case FaceType.FrontFace:
                vertex.AddRange(new Vector3[] { near_left_bottom, near_right_bottom, near_right_top, near_left_top });
                break;
            case FaceType.BackFace:
                vertex.AddRange(new Vector3[] { far_right_bottom, far_left_bottom, far_left_top, far_right_top });
                break;
            case FaceType.LeftFace:
                vertex.AddRange(new Vector3[] { far_left_bottom, near_left_bottom, near_left_top, far_left_top });
                break;
            case FaceType.RightFace:
                vertex.AddRange(new Vector3[] { near_right_bottom, far_right_bottom, far_right_top, near_right_top });
                break;
            case FaceType.TopFace:
                vertex.AddRange(new Vector3[] { near_left_top, near_right_top, far_right_top, far_left_top });
                break;
            case FaceType.BottomFace:
                vertex.AddRange(new Vector3[] { far_left_bottom, far_right_bottom, near_right_bottom, near_left_bottom });
                break;
        }
    }

    // 读取材质并缓存起来
    static Texture2D GetBlockTexture(string path)
    {
        if (!path2texture.ContainsKey(path))
        {
            Texture2D tex = Resources.Load<Texture2D>("textures/blocks/" + path);
            if (tex != null)
                path2texture[path] = tex;
        }
        return path2texture[path];
    }

    // 根据blocktype获得该方块的贴图列表（可能重复）
    static Texture2D[] GetTexturesByBlockType(BlockType blockType)
    {
        BlockTexture bt = BlockTexture.type2texture[blockType];
        Texture2D texTop = GetBlockTexture(bt.TopTexture);
        Texture2D texBottom = GetBlockTexture(bt.BottomTexture);
        Texture2D texFront = GetBlockTexture(bt.FrontTexture);
        Texture2D texBack = GetBlockTexture(bt.BackTexture);
        Texture2D texLeft = GetBlockTexture(bt.LeftTexture);
        Texture2D texRight = GetBlockTexture(bt.RightTexture);
        Texture2D[] textures = new Texture2D[] { texTop, texBottom, texFront, texBack, texLeft, texRight };

        foreach (Texture2D tex in textures)
        {
            TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex));
            ti.isReadable = true;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tex));
        }

        return textures;
    }

    static public GameObject CreateCube(BlockType blockType)
    {
        if (!blockType2MeshAndMaterial.ContainsKey(blockType))
        {
            Texture2D[] textures = GetTexturesByBlockType(blockType);
            Texture2D combinedTex = new Texture2D(128, 128);
            Rect[] rects = combinedTex.PackTextures(textures, 0, 128);
            combinedTex.filterMode = FilterMode.Point;

            vertex.Clear();
            uv.Clear();
            triangles.Clear();

            AddFace(FaceType.TopFace, rects[0]);
            AddFace(FaceType.BottomFace, rects[1]);
            AddFace(FaceType.FrontFace, rects[2]);
            AddFace(FaceType.BackFace, rects[3]);
            AddFace(FaceType.LeftFace, rects[4]);
            AddFace(FaceType.RightFace, rects[5]);

            Mesh mesh = new Mesh();
            mesh.vertices = vertex.ToArray();
            mesh.uv = uv.ToArray();
            mesh.triangles = triangles.ToArray();

            Material material = new Material(Shader.Find("Unlit/Texture"));
            material.mainTexture = combinedTex;

            blockType2MeshAndMaterial[blockType] = new MeshAndMaterial(mesh, material);
        }

        MeshAndMaterial mam = blockType2MeshAndMaterial[blockType];

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.tag = "Block";
        MeshFilter mf = cube.GetComponent<MeshFilter>();
        mf.mesh = mam.mesh;
        Renderer renderer = cube.GetComponent<Renderer>();
        renderer.material = mam.material;

        return cube;
    }

    //public static GameObject CreateCube(BlockType blockType)
    //{
    //    GameObject cube = new GameObject("cube");
    //    cube.AddComponent<BoxCollider>();

    //    BlockTexture bt = BlockTexture.type2texture[blockType];

    //    AddFace(cube, FaceType.TopFace, bt.TopTexture);
    //    AddFace(cube, FaceType.BottomFace, bt.BottomTexture);
    //    AddFace(cube, FaceType.FrontFace, bt.FrontTexture);
    //    AddFace(cube, FaceType.BackFace, bt.BackTexture);
    //    AddFace(cube, FaceType.LeftFace, bt.LeftTexture);
    //    AddFace(cube, FaceType.RightFace, bt.RightTexture);

    //    return cube;
    //}

    //static void AddFace(GameObject cube, FaceType faceType, string blockTexturePath)
    //{
    //    GameObject face = CreateFace(faceType, blockTexturePath);
    //    face.transform.parent = cube.transform;
    //}

    //static GameObject CreateFace(FaceType faceType, string blockTexturePath)
    //{
    //    GameObject face = new GameObject("face");
    //    MeshFilter mf = face.AddComponent<MeshFilter>();
    //    MeshRenderer mr = face.AddComponent<MeshRenderer>();
    //    mr.material = LoadMaterial(blockTexturePath);
    //    mf.mesh = CreateFaceMesh(faceType);
    //    return face;
    //}

    //public static Texture LoadTexture(string texturePath)
    //{
    //    Texture t = Resources.Load<Texture>(texturePath);
    //    if (t.filterMode != FilterMode.Point)
    //        t.filterMode = FilterMode.Point;
    //    return t;
    //}

    //public static Texture LoadBlockTexture(string blockTexturePath)
    //{
    //    return LoadTexture("textures/blocks/" + blockTexturePath);
    //}

    //static Material LoadMaterial(string blockTexturePath)
    //{
    //    Material m = new Material(Shader.Find("Unlit/Texture"));
    //    m.mainTexture = LoadBlockTexture(blockTexturePath);
    //    return m;
    //}

    //static Mesh CreateFaceMesh(FaceType faceType)
    //{
    //    Vector3[] vertex = { };
    //    Vector2[] uv = { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
    //    int[] triangles = { 0, 2, 1, 0, 3, 2 };

    //    switch (faceType)
    //    {
    //        case FaceType.FrontFace:
    //            vertex = new Vector3[] { near_left_bottom, near_right_bottom, near_right_top, near_left_top };
    //            break;
    //        case FaceType.BackFace:
    //            vertex = new Vector3[] { far_right_bottom, far_left_bottom, far_left_top, far_right_top };
    //            break;
    //        case FaceType.LeftFace:
    //            vertex = new Vector3[] { far_left_bottom, near_left_bottom, near_left_top, far_left_top };
    //            break;
    //        case FaceType.RightFace:
    //            vertex = new Vector3[] { near_right_bottom, far_right_bottom, far_right_top, near_right_top };
    //            break;
    //        case FaceType.TopFace:
    //            vertex = new Vector3[] { near_left_top, near_right_top, far_right_top, far_left_top };
    //            break;
    //        case FaceType.BottomFace:
    //            vertex = new Vector3[] { far_left_bottom, far_right_bottom, near_right_bottom, near_left_bottom };
    //            break;
    //    }

    //    Mesh m = new Mesh();
    //    m.vertices = vertex;
    //    m.uv = uv;
    //    m.triangles = triangles;
    //    return m;
    //}
}
