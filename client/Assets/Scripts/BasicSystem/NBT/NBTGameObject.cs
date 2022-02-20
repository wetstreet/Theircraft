using UnityEngine;

public class NBTGameObject : MonoBehaviour
{
    public NBTMesh nbtMesh;

    bool isCollidable = true;

    public Material mat;

    public static NBTGameObject Create(string name, NBTChunk chunk, int layer, bool navigate = true)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = chunk.transform;
        go.AddComponent<MeshFilter>();
        go.layer = layer;
        if (navigate)
        {
            go.AddComponent<NavMeshSourceTag>();
        }

        Material mat = new Material(Shader.Find("Custom/TextureArrayShader"));
        if (layer == 12)
        {
            mat.SetFloat("_Culling", 0);
        }
        go.AddComponent<MeshRenderer>().sharedMaterial = mat;
        go.AddComponent<MeshCollider>();

        NBTGameObject nbtGO = go.AddComponent<NBTGameObject>();
        nbtGO.mat = mat;
        nbtGO.nbtMesh.mesh.name = name;
        return nbtGO;
    }

    public static NBTGameObject CreateWater(string name, NBTChunk chunk, int layer)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = chunk.transform;
        go.AddComponent<MeshFilter>();
        go.layer = layer;

        Material mat = Resources.Load<Material>("Materials/block/water_still");

        go.AddComponent<MeshRenderer>().sharedMaterial = mat;
        go.AddComponent<MeshCollider>();

        NBTGameObject nbtGO = go.AddComponent<NBTGameObject>();
        nbtGO.mat = mat;
        nbtGO.nbtMesh.mesh.name = name;
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

        //GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_Array", TextureArrayManager.GetArray());
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = TextureArrayManager.atlas;
        GetComponent<MeshFilter>().sharedMesh = nbtMesh.mesh;
        if (isCollidable)
        {
            GetComponent<MeshCollider>().sharedMesh = nbtMesh.mesh;
        }
    }

}
