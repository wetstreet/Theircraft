using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public uint id;
    public string name;
    public Vector3 position;
    public Vector3 rotation;

    Transform trans;
    Transform head;

    public Player(CSPlayer p)
    {
        GameObject prefab = Resources.Load<GameObject>("merge-test/OtherPlayer");
        trans = Object.Instantiate(prefab).transform;
        head = trans.Find("steve/Armature/Move/Body_Lower/Body_Upper/Head.001");

        SetPosAndRot(p.Position, p.Rotation);
        MoveInstantly();
        //Move(p.Position, p.Rotation);
    }

    public void SetPosAndRot(CSVector3 pos, CSVector3 rot)
    {
        position = new Vector3(pos.x, pos.y, pos.z);
        rotation = new Vector3(rot.x, rot.y, rot.z);
    }

    public void MoveInstantly()
    {
        trans.position = position;
        trans.localEulerAngles = new Vector3(0, rotation.y, 0);
        head.transform.localEulerAngles = new Vector3(0, 0, rotation.z);
    }

    public void MoveLerp()
    {
        Debug.Log(position);
        trans.position = Vector3.Lerp(trans.position, position, Time.deltaTime);
        trans.localEulerAngles = Vector3.Lerp(trans.localEulerAngles, new Vector3(0, rotation.y, 0), Time.deltaTime);
        head.transform.localEulerAngles = Vector3.Lerp(trans.localEulerAngles, new Vector3(0, 0, rotation.z), Time.deltaTime);
    }

    public void Move(CSVector3 pos, CSVector3 rot)
    {
        position = new Vector3(pos.x, pos.y, pos.z);
        rotation = new Vector3(rot.x, rot.y, rot.z);

        trans.position = position;
        trans.localEulerAngles = new Vector3(0, rotation.y, 0);
        head.transform.localEulerAngles = new Vector3(0, 0, rotation.z);
    }
}
