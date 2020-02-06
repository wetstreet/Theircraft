using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    float createTime;
    Item item;

    private void Start()
    {
        createTime = Time.time;
        item = transform.parent.GetComponent<Item>();
    }

    void CheckShouldMove(Collider other)
    {
        if (other.tag == Tag.Player && Time.time - createTime > 2)
        {
            item.GetComponent<Rigidbody>().useGravity = false;
            item.move2player = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckShouldMove(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckShouldMove(other);
    }
}
