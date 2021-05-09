using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTStairs : NBTBlock
{
    public virtual string stairsName { get { return null; } }

    public override bool isTransparent => true;

    Mesh GetMesh()
    {
        Mesh mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-x");
        //switch (blockData)
        //{
        //    case 1:
        //        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+x");
        //        break;
        //    case 2:
        //        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-z");
        //        break;
        //    case 3:
        //        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+z");
        //        break;
        //}
        return mesh;
    }

    public override void AddCube(NBTChunk chunk, byte blockData, byte skyLight, Vector3Int pos, NBTGameObject nbtGO)
    {
        //this.pos = pos;
        //this.blockData = blockData;
        //vertices = nbtGO.vertexList;
        //triangles = nbtGO.triangles;

        //Mesh mesh = GetMesh();

        //int faceIndex = TextureArrayManager.GetIndexByName(stairsName);

        //int length = vertices.Count;
        //for (int i = 0; i < mesh.vertices.Length; i++)
        //{
        //    vertices.Add(new Vertex { pos = ToVector4(mesh.vertices[i] + pos, faceIndex), texcoord = mesh.uv[i], color = Color.white });
        //}
        //foreach (int index in mesh.triangles)
        //{
        //    triangles.Add(index + length);
        //}
    }
    
    public override Mesh GetItemMesh(NBTChunk chunk, byte data)
    {
        return GetMesh();
    }

    public override Material GetItemMaterial(byte data)
    {
        if (!itemMaterialDict.ContainsKey(0))
        {
            Material mat = new Material(Shader.Find("Custom/BlockShader"));
            Texture2D tex = Resources.Load<Texture2D>("GUI/block/" + stairsName);
            mat.mainTexture = tex;

            itemMaterialDict.Add(0, mat);
        }
        return itemMaterialDict[0];
    }
}
