using UnityEngine;

public class NBTGameObject : MonoBehaviour
{
    public NBTMesh nbtMesh;

    bool isCollidable;

    public Material mat;

    public static NBTGameObject Create(string name, NBTChunk chunk, int layer, bool isCollidable = true)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = chunk.transform;
        go.AddComponent<MeshFilter>();
        go.layer = layer;
        if (isCollidable)
        {
            go.AddComponent<MeshCollider>();
        }

        Material mat = new Material(Shader.Find("Custom/TextureArrayShader"));
        if (layer == 12)
        {
            mat.SetFloat("_Culling", 0);
        }
        go.AddComponent<MeshRenderer>().sharedMaterial = mat;
        go.AddComponent<NavMeshSourceTag>();

        NBTGameObject nbtGO = go.AddComponent<NBTGameObject>();
        nbtGO.mat = mat;
        nbtGO.nbtMesh.mesh.name = name;
        nbtGO.isCollidable = isCollidable;
        return nbtGO;
    }

    public void Clear()
    {
        nbtMesh.Clear();
    }

    private void Awake()
    {
        nbtMesh = new NBTMesh(65536);
    }

    private void OnDestroy()
    {
        nbtMesh.Dispose();
    }

    public void Refresh()
    {
        nbtMesh.Refresh();

        GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_Array", TextureArrayManager.GetArray());
        GetComponent<MeshFilter>().sharedMesh = nbtMesh.mesh;
        if (isCollidable)
        {
            GetComponent<MeshCollider>().sharedMesh = nbtMesh.mesh;
        }
    }

}
