using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    //General
    enum MovementState { still, moving, dashing, knockBacked}
    MovementState currentState;
    Rigidbody2D rBod;


    //move
    public float acc;
    public float decc;
    public float maxVel;
    float speed = 0;
    Vector2 moveDir;
    Vector2 lastDir = Vector2.right;

    //dash
    float dashTimer;
    public float dashDuration;
    public float dashSpeed;
    bool dashTriggered;
    bool dashPressed;

    //KnockBack
    Vector2 knockBackDir;
    float knockBackTimer;
    public float knockBackDuration;
    public float knockBackSpeed;

    [HideInInspector] public Transform spawnPoint;

    public void OnMove(InputAction.CallbackContext _ctx)
    {
        if (currentState == MovementState.still || currentState == MovementState.moving)
        {
            currentState = MovementState.moving;
            moveDir = _ctx.ReadValue<Vector2>().normalized;
        }
    }

    public void OnDash(InputAction.CallbackContext _ctx)
    {
        dashTriggered = _ctx.action.triggered;
    }

    void Start()
    {
        rBod = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        if (dashTriggered && !dashPressed)
        {
            if ((currentState == MovementState.still || currentState == MovementState.moving))
            {
                currentState = MovementState.dashing;
                dashTimer = dashDuration;
                dashPressed = true;
            }
        }
        else if (!dashTriggered && dashPressed)
        {
            dashPressed = false;
        }


    }

    private void FixedUpdate()
    {

        if (currentState == MovementState.moving)
        {
            if (moveDir != Vector2.zero)
            {
                if (speed < maxVel)
                {
                    speed += acc;
                }
                lastDir = moveDir;
            }
            else if (moveDir == Vector2.zero)
            {
                currentState = MovementState.still;
            }
            rBod.velocity = moveDir * speed;
        }
        else if (currentState == MovementState.dashing)
        {
            rBod.velocity = lastDir * dashSpeed;
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                speed = maxVel;
                if (moveDir != Vector2.zero)
                {
                    currentState = MovementState.moving;
                }
                else
                {
                    currentState = MovementState.still;
                }
            }
        }
        else if (currentState == MovementState.still)
        {
            speed -= decc;
            speed = Mathf.Max(speed, 0);
            rBod.velocity = lastDir * speed;
        }
        else if (currentState == MovementState.knockBacked)
        {
            rBod.velocity = knockBackDir * knockBackSpeed;
            knockBackTimer -= Time.deltaTime;
            if (knockBackTimer <= 0)
            {
                currentState = MovementState.still;
                rBod.velocity = Vector2.zero;
            }
        }

    }


    void KnockBack(Vector3 colPos)
    {
        currentState = MovementState.knockBacked;

        knockBackDir = (transform.position - colPos).normalized;
        knockBackTimer = knockBackDuration;
    }

    void Respawn()
    {
        transform.position = spawnPoint.position;
        rBod.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            KnockBack(collision.transform.position);
        }
    }

}
