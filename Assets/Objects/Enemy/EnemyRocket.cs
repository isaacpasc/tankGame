using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRocket : MonoBehaviour
{
    [SerializeField] private GameObject explode;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private GameObject player;


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            searchForPlayer();
            selfDestruction();
        }
        else if (collision.gameObject.CompareTag("Player1"))
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

    void searchForPlayer()
    {
        //Adds targets in view radius to an array
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, 360, targetMask);
        //For every targetsInViewRadius it checks if they are inside the field of view
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.up, dirToTarget) < 360 / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                //If line draw from object to target is not interrupted by wall, add target to list of visible 
                //targets
                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget - 3, obstacleMask))
                {
                    if (dstToTarget < 6f)
                    {
                        print("kill player");
                        //Destroy(player);
                    }
                }
            }
        }
    }
}
