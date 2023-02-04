using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{

    enum MovementState { still, moving, dashing, knockBacked}
    MovementState currentState;

    public float acc;
    public float decc;
    public float maxVel;
    float speed = 0;
    Vector2 moveDir;
    Vector2 lastDir;
    Rigidbody2D rBod;

    
    float dashTimer;
    public float dashDuration;
    public float dashSpeed;
    bool dashTriggered;
    bool dashPressed;

    public void OnMove(InputAction.CallbackContext _ctx)
    {
        if (currentState == MovementState.still || currentState == MovementState.moving)
        {
            currentState = MovementState.moving;
            moveDir = _ctx.ReadValue<Vector2>().normalized;
        }

        print("hey");
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
            if (speed < maxVel && moveDir != Vector2.zero)
            {
                speed += acc;
            }
            else if (speed > 0 && moveDir == Vector2.zero)
            {
                speed -= decc;
                if (speed <= 0)
                {
                    currentState = MovementState.still;
                }
            }
            rBod.velocity = moveDir * speed;
        }

        if (currentState == MovementState.dashing)
        {
            rBod.velocity = moveDir * dashSpeed;
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                if (moveDir != Vector2.zero)
                {
                    currentState = MovementState.moving;
                    speed = maxVel;
                }
                else
                {
                    currentState = MovementState.still;
                    rBod.velocity = Vector2.zero;
                    speed = maxVel;
                }
                
            }
        }




    }


}
