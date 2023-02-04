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

    //hp and spawn
    bool dead;
    float respawnTimer;
    public float respawnDuration;
    [HideInInspector] public Transform spawnPoint;

    Tool currentTool;

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

    public void OnUse(InputAction.CallbackContext _ctx)
    {
        if (!currentTool)
            return;

        currentTool.Use(lastDir);
        currentTool = null;
    }

    public void OnThrow(InputAction.CallbackContext _ctx)
    {
        if (!currentTool)
            return;

        currentTool.Throw(transform, lastDir);
        currentTool = null;
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
                StartDash();
            }
        }
        else if (!dashTriggered && dashPressed)
        {
            dashPressed = false;
        }

        if (dead)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0)
            {
                Respawn();
                dead = false;
            }
        }

    }

    private void FixedUpdate()
    {
        if (dead)
        {
            return;
        }

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
                GetComponent<BoxCollider2D>().isTrigger = false;

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

    void StartDash()
    {
        currentState = MovementState.dashing;
        dashTimer = dashDuration;
        dashPressed = true;
        GetComponent<BoxCollider2D>().isTrigger = true;
    }


    public void Killed()
    {
        dead = true;
        respawnTimer = respawnDuration;
        rBod.velocity = Vector2.zero;
    }

    public void KnockBack(Vector3 colPos)
    {
        currentState = MovementState.knockBacked;

        knockBackDir = (transform.position - colPos).normalized;
        knockBackTimer = knockBackDuration;
    }

    void Respawn()
    {
        transform.position = spawnPoint.position;
        currentState = MovementState.still;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Killed();
            KnockBack(collision.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            GetComponent<BoxCollider2D>().isTrigger = false;
            rBod.velocity = Vector2.zero;
            currentState = MovementState.still;
        }
        else if (collision.gameObject.tag == "Tool")
        {
            currentTool = collision.GetComponent<Tool>().PickUp(this.transform);
        }
    }

}
