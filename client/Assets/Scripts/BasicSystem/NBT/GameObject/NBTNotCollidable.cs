using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTNotCollidable : NBTGameObject
{
    public override int meshSize => 8192 ;

    public static NBTGameObject Create(NBTChunk chunk)
    {
        GameObject go = new GameObject("NotCollidable");
        go.transform.parent = chunk.transform;
        go.AddComponent<MeshFilter>();
        go.layer = LayerMask.NameToLayer("Plant");

        Material mat = new Material(Shader.Find("Custom/TextureArrayShader"));
        mat.SetFloat("_Culling", 0);
        go.AddComponent<MeshRenderer>().sharedMaterial = mat;
        go.AddComponent<MeshCollider>();

        NBTGameObject nbtGO = go.AddComponent<NBTNotCollidable>();
        nbtGO.mat = mat;
        nbtGO.nbtMesh.mesh.name = "NotCollidable";
        return nbtGO;
    }
}
