using UnityEngine;

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

    public static GameObject CreateCube()
    {
        GameObject cube = new GameObject("cube");
        cube.AddComponent<BoxCollider>();

        AddFace(cube, FaceType.FrontFace, "tnt_side");
        AddFace(cube, FaceType.BackFace, "tnt_side");
        AddFace(cube, FaceType.LeftFace, "tnt_side");
        AddFace(cube, FaceType.RightFace, "tnt_side");
        AddFace(cube, FaceType.TopFace, "tnt_top");
        AddFace(cube, FaceType.BottomFace, "tnt_bottom");

        return cube;
    }

    static void AddFace(GameObject cube, FaceType faceType, string blockTexturePath)
    {
        GameObject face = CreateFace(faceType, blockTexturePath);
        face.transform.parent = cube.transform;
    }

    static GameObject CreateFace(FaceType faceType, string blockTexturePath)
    {
        GameObject face = new GameObject("face");
        MeshFilter mf = face.AddComponent<MeshFilter>();
        MeshRenderer mr = face.AddComponent<MeshRenderer>();
        mr.material = LoadMaterial(blockTexturePath);
        mf.mesh = CreateFaceMesh(faceType);
        return face;
    }

    public static Texture LoadTexture(string texturePath)
    {
        Texture t = Resources.Load<Texture>(texturePath);
        if (t.filterMode != FilterMode.Point)
            t.filterMode = FilterMode.Point;
        return t;
    }

    public static Texture LoadBlockTexture(string blockTexturePath)
    {
        return LoadTexture("textures/blocks/" + blockTexturePath);
    }

    static Material LoadMaterial(string blockTexturePath)
    {
        Material m = new Material(Shader.Find("Unlit/Texture"));
        m.mainTexture = LoadBlockTexture(blockTexturePath);
        return m;
    }

    static Mesh CreateFaceMesh(FaceType faceType)
    {
        Vector3[] vertex = { };
        Vector2[] uv = { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
        int[] triangles = { 0, 2, 1, 0, 3, 2 };

        switch (faceType)
        {
            case FaceType.FrontFace:
                vertex = new Vector3[] { near_left_bottom, near_right_bottom, near_right_top, near_left_top };
                break;
            case FaceType.BackFace:
                vertex = new Vector3[] { far_right_bottom, far_left_bottom, far_left_top, far_right_top };
                break;
            case FaceType.LeftFace:
                vertex = new Vector3[] { far_left_bottom, near_left_bottom, near_left_top, far_left_top };
                break;
            case FaceType.RightFace:
                vertex = new Vector3[] { near_right_bottom, far_right_bottom, far_right_top, near_right_top };
                break;
            case FaceType.TopFace:
                vertex = new Vector3[] { near_left_top, near_right_top, far_right_top, far_left_top };
                break;
            case FaceType.BottomFace:
                vertex = new Vector3[] { far_left_bottom, far_right_bottom, near_right_bottom, near_left_bottom };
                break;
        }

        Mesh m = new Mesh();
        m.vertices = vertex;
        m.uv = uv;
        m.triangles = triangles;
        return m;
    }
}
