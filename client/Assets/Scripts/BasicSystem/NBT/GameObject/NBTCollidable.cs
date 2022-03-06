using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCollidable : NBTGameObject
{
    public override int meshSize => 65535;

    public static NBTGameObject Create(NBTChunk chunk)
    {
        GameObject go = new GameObject("Collidable");
        go.transform.parent = chunk.transform;
        go.AddComponent<MeshFilter>();
        go.layer = LayerMask.NameToLayer("Chunk");

        go.AddComponent<NavMeshSourceTag>();

        Material mat = new Material(Shader.Find("Custom/TextureArrayShader"));
        go.AddComponent<MeshRenderer>().sharedMaterial = mat;
        go.AddComponent<MeshCollider>();

        NBTGameObject nbtGO = go.AddComponent<NBTCollidable>();
        nbtGO.mat = mat;
        nbtGO.nbtMesh.mesh.name = "Collidable";
        return nbtGO;
    }
}
