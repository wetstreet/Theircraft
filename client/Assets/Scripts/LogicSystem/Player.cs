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

    public Transform transform;
    private Transform steve;
    Transform head;
    private NavMeshAgent agent;
    private CharacterController cc;

    public Player(CSPlayer p)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/OtherPlayer");
        transform = Object.Instantiate(prefab).transform;
        transform.name = "player_" + p.PlayerID;
        steve = transform.Find("steve");
        head = transform.Find("steve/Armature/Move/Body_Lower/Body_Upper/Head.001");
        agent = transform.GetComponent<NavMeshAgent>();
        cc = transform.GetComponent<CharacterController>();

        Move(p.Position, p.Rotation);
    }

    public void SetDestination(Vector3 pos)
    {
        agent.destination = pos;
    }

    public void Update()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 1f, LayerMask.GetMask("Chunk")))
        {
            float dis = Vector3.Distance(transform.position, hit.point);
            steve.transform.localPosition = new Vector3(0, -dis, 0);
        }
    }

    public void Move(CSVector3 pos, CSVector3 rot)
    {
        position = new Vector3(pos.x, pos.y, pos.z);
        rotation = new Vector3(rot.x, rot.y, rot.z);

        transform.position = position;
        transform.localEulerAngles = new Vector3(0, rotation.y, 0);
        head.transform.localEulerAngles = new Vector3(0, 0, rotation.z);
    }
}
