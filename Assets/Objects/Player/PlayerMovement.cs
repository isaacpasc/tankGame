using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Joystick js;
    [SerializeField] private new Animator animation;
    [SerializeField] private Transform body;

    Vector2 movement;

    void Update()
    {
        movementInput();
        if(js.Vertical != 0 || js.Horizontal !=0)
        {
            rotateBody();
        }
    }
    void FixedUpdate()
    {
        rb.velocity = movement * movementSpeed;
    }
    void movementInput()
    {
        float mx = js.Horizontal;
        float my = js.Vertical;

        movement = new Vector2(mx, my).normalized;

        if (mx != 0 && my != 0)
        {
            animation.enabled = true;



        } else
        {
            animation.enabled = false;
            
            
        }
    }
    void rotateBody()
    {
        float angle = Mathf.Atan2(js.Vertical, js.Horizontal) * Mathf.Rad2Deg - 90f;
        body.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
