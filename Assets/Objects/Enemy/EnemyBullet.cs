using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private GameObject explode;


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            selfDestruction();
        } else if (collision.gameObject.CompareTag("Player1"))
        {
            //Destroy(collision.gameObject);
            selfDestruction();
        }
    }

    void selfDestruction()
    {
        GameObject ex = Instantiate(explode, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
