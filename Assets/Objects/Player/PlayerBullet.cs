using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private int bounce = 1;
    [SerializeField] private GameObject smallExplosion;
    [SerializeField] private GameObject enemyDeathPrefab;
    private Rigidbody2D rb;
    private Vector3 lastVel;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        if(bounce <= -1 || rb.velocity.magnitude == 0)
        {
            selfDestruction();
        } else
        {
            lastVel = rb.velocity;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Block"))
        {
            var speed = lastVel.magnitude;
            var dir = Vector3.Reflect(lastVel.normalized, collision.contacts[0].normal);
            rb.velocity = dir * Mathf.Max(speed, 0f);

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            bounce--;
        } else if(collision.gameObject.CompareTag("EnemyBullet"))
        {
            Destroy(collision.gameObject);
            selfDestruction();
        } else if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject enemyDeath = Instantiate(enemyDeathPrefab, collision.gameObject.transform.position, collision.gameObject.transform.rotation);
            Destroy(collision.gameObject);
            GameObject go = GameObject.Find("EnemiesLeftText");
            EnemyCounter counter = (EnemyCounter)go.GetComponent(typeof(EnemyCounter));
            counter.enemyDies();
            selfDestruction();
        }
    }

    void selfDestruction()
    {
        GameObject bulletHit = Instantiate(smallExplosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
