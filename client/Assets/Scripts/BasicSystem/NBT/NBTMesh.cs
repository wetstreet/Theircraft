using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class NBTMesh
{
    public NativeArray<Vertex> vertexArray;
    public NativeArray<ushort> triangleArray;
    public ushort vertexCount = 0;
    public ushort triangleCount = 0;

    public Mesh mesh;

    int chunkX = 0;
    int chunkZ = 0;
    bool isChunk = false;

    public NBTMesh(int size, NBTChunk chunk = null)
    {
        vertexArray = new NativeArray<Vertex>(size, Allocator.Persistent);
        triangleArray = new NativeArray<ushort>(size, Allocator.Persistent);

        mesh = new Mesh();

        if (chunk != null)
        {
            isChunk = true;
            chunkX = chunk.x;
            chunkZ = chunk.z;
        }
    }

    public void Dispose()
    {
        vertexArray.Dispose();
        triangleArray.Dispose();
    }

    public void Clear()
    {
        vertexCount = 0;
        triangleCount = 0;
    }

    static VertexAttributeDescriptor[] vertexAttributes;

    public void Refresh()
    {
        mesh.Clear();

        if (vertexAttributes == null)
        {
            vertexAttributes = new[] {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 4),
                new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
            };
        }

        mesh.SetVertexBufferParams(vertexCount, vertexAttributes);
        mesh.SetVertexBufferData(vertexArray, 0, 0, vertexCount);

        mesh.SetIndexBufferParams(triangleCount, IndexFormat.UInt16);
        mesh.SetIndexBufferData(triangleArray, 0, 0, triangleCount);

        mesh.subMeshCount = 1;
        SubMeshDescriptor desc = new SubMeshDescriptor();
        desc.indexStart = 0;
        desc.indexCount = triangleCount;
        desc.baseVertex = 0;
        desc.topology = MeshTopology.Triangles;
        mesh.SetSubMesh(0, desc);

        Vector3[] positions = new Vector3[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            Vertex vert = vertexArray[i];
            positions[i] = vert.pos;
        }
        try
        {
            Bounds bounds = GeometryUtility.CalculateBounds(positions, Matrix4x4.identity);
            mesh.bounds = bounds;
        }
        catch (System.Exception e)
        {
            Debug.LogError("calculate bounds error,ischunk=" + isChunk+",chunk=(" + chunkX + "," + chunkZ + "), vertexCount=" + vertexCount + "\n" + e.Message);
        }
    }
}
