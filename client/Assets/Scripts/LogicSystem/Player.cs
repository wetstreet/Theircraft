using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player
{
    public uint id;
    public string name;
    public Vector3 position;
    public Vector3 rotation;

    Transform trans;
    Transform head;
    private NavMeshAgent agent;

    public Player(CSPlayer p)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/OtherPlayer");
        trans = Object.Instantiate(prefab).transform;
        head = trans.Find("steve/Armature/Move/Body_Lower/Body_Upper/Head.001");
        agent = trans.GetComponent<NavMeshAgent>();

        SetPosAndRot(p.Position, p.Rotation);
        MoveInstantly();
    }

    public void SetDestination(Vector3 pos)
    {
        agent.destination = pos;
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

    public void Move(CSVector3 pos, CSVector3 rot)
    {
        position = new Vector3(pos.x, pos.y, pos.z);
        rotation = new Vector3(rot.x, rot.y, rot.z);

        trans.position = position;
        trans.localEulerAngles = new Vector3(0, rotation.y, 0);
        head.transform.localEulerAngles = new Vector3(0, 0, rotation.z);
    }
}
