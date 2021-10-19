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

    Material dynamicMat;

    public float speed = 2f;
    public float jumpSpeed = 9;
    public float gravity = 28;
    Vector3 moveDir = Vector3.zero;

    public float angularSpeed = 300f;

    float healthInternal = 20;
    public float health
    {
        get { return healthInternal; }
        set
        {
            healthInternal = value;
            if (healthInternal <= 0)
            {
                Die();
            }
        }
    }

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

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        dynamicMat = Instantiate(renderers[0].sharedMaterial);
        foreach (Renderer renderer in renderers)
        {
            renderer.sharedMaterial = dynamicMat;
        }

        transform.position = position;
        //transform.localEulerAngles = new Vector3(0, rotation.y, 0);
        //head.transform.localEulerAngles = new Vector3(0, 0, rotation.z);

        Move();
    }

    private void OnDestroy()
    {
        Destroy(dynamicMat);
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public void AddForce(Vector3 force)
    {
        //horizontalSpeed.x += force.x;
        moveDir.y += force.y;
        //horizontalSpeed.z += force.z;
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

    Color colorHit = new Color(2.19f, 0.27f, 0.36f);
    Color colorNormal = new Color(0.52f, 0.52f, 0.52f);
    float hitTime;
    float hitColorExistTime = 0.45f;
    public void OnHit()
    {
        dynamicMat.color = colorHit;
        hitTime = Time.time;
    }

    void UpdateColor()
    {
        if (Time.time - hitTime > hitColorExistTime)
        {
            dynamicMat.color = colorNormal;
        }
    }

    static Vector3 offset = new Vector3(0, 1.38f, 0);
    public float attenuation = 0.75f;
    void MoveByPath()
    {
        LookAt(PlayerController.instance.position + offset);

        Vector3 horizontalDir = Vector3.zero;
        if (GameModeManager.isSurvival && path.corners.Length > 0 && currTargetIndex != path.corners.Length)
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
        //else
        //{
        //    horizontalDir *= attenuation;
        //}
        horizontalDir = horizontalDir.normalized;
        horizontalDir *= speed;

        moveDir = new Vector3(horizontalDir.x, moveDir.y, horizontalDir.z);

        moveDir.y -= gravity * Time.deltaTime;

        cc.Move(moveDir * Time.deltaTime);

        if (cc.isGrounded)
        {
            moveDir.y = 0;

            if (shouldJump)
            {
                shouldJump = false;
                moveDir.y = jumpSpeed;
            }
        }
    }

    void Navigate()
    {
        float dist = (PlayerController.instance.position - transform.position).magnitude;
        if (dist < 10)
        {
            Move(PlayerController.instance.position);
        }

        if (dist < 1 && Time.time - lastAttackTime > attackInterval)
        {
            Attack();
        }

        if (dist >= 0.5f)
        {
            MoveByPath();
        }
    }

    float attackInterval = 1f;
    float lastAttackTime;
    private void Update()
    {
        Navigate();

        UpdateColor();
    }

    public float attackStrength = 5f;
    public float attackDamage = 1f;
    void Attack()
    {
        if (GameModeManager.isSurvival)
        {
            Vector3 hitForce = transform.forward + Vector3.up;
            PlayerController.instance.AddForce(hitForce * attackStrength);

            PlayerController.instance.OnHit(attackDamage);

            lastAttackTime = Time.time;
        }
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
