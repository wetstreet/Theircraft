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

    public NBTMesh(int size)
    {
        vertexArray = new NativeArray<Vertex>(size, Allocator.Persistent);
        triangleArray = new NativeArray<ushort>(size, Allocator.Persistent);

        mesh = new Mesh();
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

        mesh.Clear();
    }

    static VertexAttributeDescriptor[] vertexAttributes;

    public void Refresh()
    {
        if (vertexCount > 0)
        {
            mesh.Clear();

            if (vertexAttributes == null)
            {
                vertexAttributes = new[] {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 4, 0),
                new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 0),
                new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4, 0),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 4, 0),
            };
            }

            mesh.SetVertexBufferParams(vertexCount, vertexAttributes);
            try
            {
                mesh.SetVertexBufferData(vertexArray, 0, 0, vertexCount, 0);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message + "\nlength=" + vertexArray.Length + ",vertexcount=" + vertexCount + ",bytes=" + (14 * 4 * vertexCount));
            }

            mesh.SetIndexBufferParams(triangleCount, IndexFormat.UInt16);
            try
            {
                mesh.SetIndexBufferData(triangleArray, 0, 0, triangleCount);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message + "\nlength=" + triangleArray.Length + ",triangleCount=" + triangleCount + ",vertexcount=" + vertexCount);
                throw e;
            }

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
            Bounds bounds = GeometryUtility.CalculateBounds(positions, Matrix4x4.identity);
            mesh.bounds = bounds;
        }
    }
}
