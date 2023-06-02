using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;
    // Start is called before the first frame update

    void OnCollisionEnter(Collision collision) //총알 isTrigger일 시에는 바꾸기
    {
        if (!isRock && collision.gameObject.CompareTag("Floor"))
        {
            Invoke("DestoryBullet", 3);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            ObjectPoolManager.Instance.BulletInsertQueue(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.CompareTag("Floor"))
        {
            Invoke("DestoryBullet", 3);
        }
    }
    void DestoryBullet()
    {
        ObjectPoolManager.Instance.BulletInsertQueue(gameObject);
    }

}
