using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBarrel : MonoBehaviour
{
    [SerializeField] private Joystick js;
    [SerializeField] private Transform body;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletForce = 20f;
    [SerializeField] private float shotPeriod = .5f;
    [SerializeField] private new Animator animation;

    private float shotTimer = 0f;

    void Update()
    {
        if(js.Horizontal != 0 || js.Vertical != 0)
        {
            if(shotTimer == 0f)
            {
                shoot();
            }
            else
            {
                shotTimer -= Time.deltaTime;     
                shotTimer = Mathf.Max(shotTimer, 0);
            }
        }
    }
    void FixedUpdate()
    {
        if (js.Vertical != 0 || js.Horizontal != 0)
        {
            rotateBarrel();
        }
    }
    void rotateBarrel()
    {
        float angle = Mathf.Atan2(js.Vertical, js.Horizontal) * Mathf.Rad2Deg - 90f;
        body.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    void shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
        animation.Play("GreenMuzzleFlash");
        shotTimer = shotPeriod;
    }
}
