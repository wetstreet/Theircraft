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

    public bool CoolDownFinish()
    {
        return Time.time - createTime > item.coolDownTime;
    }

    public bool MergeCoolDownFinish()
    {
        return Time.time - createTime > 1;
    }

    void CheckShouldMove(Collider other)
    {
        if (CoolDownFinish())
        {
            if (other.tag == Tag.Player)
            {
                item.StartMove();
            }
        }
        if (other.gameObject.layer == LayerMask.NameToLayer(Layer.ItemTrigger))
        {
            Item otherItem = other.transform.parent.GetComponent<Item>();
            if (!item.destroyed && !otherItem.destroyed &&
                otherItem.generator == item.generator &&
                MergeCoolDownFinish() && other.GetComponent<ItemTrigger>().MergeCoolDownFinish() &&
                Vector3.Distance(other.transform.position, transform.position) < 1f)
            {
                if (otherItem.Count > item.Count)
                {
                    otherItem.AddCount(item.Count);
                    item.destroyed = true;
                    Destroy(item.gameObject);
                }
                else
                {
                    item.AddCount(otherItem.Count);
                    otherItem.destroyed = true;
                    Destroy(otherItem.gameObject);
                }
            }
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
