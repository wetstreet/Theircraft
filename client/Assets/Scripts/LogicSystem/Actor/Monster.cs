using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public uint id;
    public Vector3 position;

    private Transform steve;
    private Transform head;
    private CharacterController cc;
    private NavMeshPath path;

    private int currTargetIndex;

    public float speed = 5f;
    public float jumpSpeed = 9;
    public float gravity = 28;
    Vector3 moveDir = Vector3.zero;

    public float angularSpeed = 300f;

    public static Monster CreateMonster(uint id, Vector3 position)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Zombie");
        GameObject go = Instantiate(prefab);
        Monster monster = go.AddComponent<Monster>();
        monster.id = id;
        monster.position = position;
        return monster;
    }

    private void Start()
    {
        transform.name = "monster_" + id;
        //steve = transform.Find("steve");
        head = transform.Find("zombie/Move/Body/Head");
        cc = transform.GetComponent<CharacterController>();
        path = new NavMeshPath();

        transform.position = position;
        //transform.localEulerAngles = new Vector3(0, rotation.y, 0);
        //head.transform.localEulerAngles = new Vector3(0, 0, rotation.z);

        Move();
    }

    Vector3 realForward;
    void LookAt(Vector3 targetPosition)
    {
        realForward = -head.right;
        Vector2 forwardHorizontal = new Vector2(realForward.x, realForward.z).normalized;

        Vector2 headHorizontal = new Vector2(head.position.x, head.position.z);
        Vector2 targetHorizontal = new Vector2(targetPosition.x, targetPosition.z);
        Vector2 head2targetH = (targetHorizontal - headHorizontal).normalized;

        Vector3 head2target = (targetPosition - head.position).normalized;

        float dot = Vector2.Dot(forwardHorizontal, head2targetH);

        Debug.DrawLine(head.position, head.position + head.forward);

        //if (transform.InverseTransformPoint(targetPosition).x < 0)
        //{
        //    head.localEulerAngles = new Vector3(0, (1 - dot) * -90, head2target.y * -90);
        //}
        //else
        //{
        //    head.localEulerAngles = new Vector3(0, (1 - dot) * 90, head2target.y * -90);
        //}


        //Debug.Log("dot=" + dot + ",forward = " + forwardHorizontal + ",head2targetH=" + head2targetH);

        //Debug.Log(Vector3.Dot(realForward, target2head) + "," + Vector3.Angle(realForward, target2head));
        //Debug.DrawLine(head.position, targetPosition);

    }

    static Vector3 offset = new Vector3(0, 1.38f, 0);
    private void Update()
    {
        LookAt(PlayerController.instance.position + offset);

        Vector3 horizontalDir = Vector3.zero;
        if (path.corners.Length > 0 && currTargetIndex != path.corners.Length)
        {
            Vector3 player2target = path.corners[currTargetIndex] - transform.position;
            horizontalDir = new Vector3(player2target.x, 0, player2target.z);
            float dis = horizontalDir.sqrMagnitude;
            //Debug.Log("dis=" + dis);
            if (dis < 0.005f)
            {
                if (currTargetIndex < path.corners.Length - 1)
                {
                    currTargetIndex++;
                    player2target = path.corners[currTargetIndex] - transform.position;
                    horizontalDir = new Vector3(player2target.x, 0, player2target.z);
                }
                else
                {
                    horizontalDir = Vector3.zero;
                }
            }

            if (horizontalDir != Vector3.zero)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(horizontalDir, Vector3.up), angularSpeed * Time.deltaTime);
            }
        }
        horizontalDir = horizontalDir.normalized;
        horizontalDir *= speed;

        moveDir = new Vector3(horizontalDir.x, moveDir.y, horizontalDir.z);

        if (cc.isGrounded)
        {
            moveDir.y = 0;

            if (shouldJump)
            {
                shouldJump = false;
                moveDir.y = jumpSpeed;
            }
        }

        moveDir.y -= gravity * Time.deltaTime;

        cc.Move(moveDir * Time.deltaTime);
    }

    bool shouldJump = false;
    float lastCollisionTime;
    protected void OnControllerColliderHit(ControllerColliderHit collision)
    {
        float timediff = Time.time - lastCollisionTime;
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Chunk") && collision.normal.y == 0 && cc.isGrounded && timediff >= 0.1f)
        {
            shouldJump = true;
            lastCollisionTime = Time.time;
        }
    }

    public void Move()
    {
        if (Physics.Raycast(transform.position + transform.up / 2, -transform.up, out RaycastHit playerHit))
        {
            if (NavMesh.CalculatePath(playerHit.point, position, NavMesh.AllAreas, path))
            {
                currTargetIndex = 1;
            }
        }
    }

    public void Move(Vector3 pos)
    {
        position = pos;
        Move();
    }
}
