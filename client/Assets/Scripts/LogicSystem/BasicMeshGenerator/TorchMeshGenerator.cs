using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class TorchMeshGenerator : IMeshGenerator
{
    static TorchMeshGenerator _instance;
    public static TorchMeshGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TorchMeshGenerator();
            }
            return _instance;
        }
    }

    override public Mesh GenerateSingleMesh(CSBlockType type)
    {
        Mesh torchMesh = Resources.Load<Mesh>("Meshes/blocks/torch/torch");

        Mesh mesh = new Mesh();
        mesh.name = "CubeMesh";

        List<Vector2> uv = new List<Vector2>();

        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)CSBlockType.Torch];
        Vector2Int texPos = texCoords.front;
        texPos.y = (atlas_row - 1) - texPos.y;

        foreach (Vector2 singleUV in torchMesh.uv)
        {
            uv.Add(new Vector2((texPos.x + singleUV.x) / atlas_column, (texPos.y + singleUV.y) / atlas_row));
        }
        
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.up / 2, Quaternion.Euler(-12, 28, 24), Vector3.one * 2);
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < torchMesh.vertices.Length; i++)
        {
            vertices.Add(matrix.MultiplyPoint(torchMesh.vertices[i]));
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = torchMesh.triangles;

        return mesh;
    }

    static Vector3Int forward = new Vector3Int(0, 0, -1);
    static Vector3Int back = new Vector3Int(0, 0, 1);
    static Mesh GetTorchMesh(Vector3Int globalPosition)
    {
        Mesh mesh = LoadMesh("Meshes/blocks/torch/torch");
        //Debug.Log("orient=" + orientation);
        Vector3Int dependPos = ChunkManager.GetBlockDependence(globalPosition);
        Vector3Int diff = dependPos - globalPosition;
        Debug.Log("diff=" + diff);
        if (diff == Vector3Int.left)
        {
            mesh = LoadMesh("Meshes/blocks/torch/torch_+x");
        }
        else if (diff == Vector3Int.right)
        {
            mesh = LoadMesh("Meshes/blocks/torch/torch_-x");
        }
        else if (diff == forward)
        {
            mesh = LoadMesh("Meshes/blocks/torch/torch_+z");
        }
        else if (diff == back)
        {
            mesh = LoadMesh("Meshes/blocks/torch/torch_-z");
        }
        return mesh;
    }

    override public void GenerateMeshInChunk(CSBlockType type, Vector3Int posInChunk, Vector3Int globalPos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
        Vector2Int texPos = texCoords.front;
        texPos.y = (atlas_row - 1) - texPos.y;

        Mesh torchMesh = GetTorchMesh(globalPos);
        int length = vertices.Count;
        foreach (Vector3 singleVertex in torchMesh.vertices)
        {
            Vector3 pos = singleVertex + posInChunk;
            vertices.Add(pos);
        }

        foreach (Vector2 singleUV in torchMesh.uv)
        {
            uv.Add(new Vector2((texPos.x + singleUV.x) / atlas_column, (texPos.y + singleUV.y) / atlas_row));
        }

        foreach (int index in torchMesh.triangles)
        {
            triangles.Add(index + length);
        }
    }
}
