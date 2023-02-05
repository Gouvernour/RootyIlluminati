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

    //tools
    [HideInInspector] public Tool currentTool;
    bool useTriggered;
    bool usePressed;
    Tool toolBeneathYou;

    //hp and spawn
    bool dead;
    float respawnTimer;
    public float respawnDuration;
    [HideInInspector] public Transform spawnPoint;

    public Animator anim;

    public void OnMove(InputAction.CallbackContext _ctx)
    {
        if (currentState == MovementState.still || currentState == MovementState.moving)
        {
            currentState = MovementState.moving;
            moveDir = _ctx.ReadValue<Vector2>().normalized;

            if (moveDir.y < 0 && Mathf.Abs(moveDir.y) >= Mathf.Abs(moveDir.x)) anim.Play("RunFront1");
            else if (moveDir.y > 0 && Mathf.Abs(moveDir.y) >= Mathf.Abs(moveDir.x)) anim.Play("RunBack");
            else if (moveDir.x > 0 && name.Contains("Raccoon")) anim.Play("RunRight");
            else if (moveDir.x < 0 && name.Contains("Raccoon")) anim.Play("RunLeft");

            if (moveDir == Vector2.zero) anim.Play("Idle");

        }
    }

    public void OnDash(InputAction.CallbackContext _ctx)
    {
        dashTriggered = _ctx.action.triggered;
    }
	
	public Vector2 GetLastDir() { return lastDir; }
	
    public void OnUse(InputAction.CallbackContext _ctx)
    {
        useTriggered = _ctx.action.triggered;
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
        //dash things
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

        //tool use things
        if (useTriggered && !usePressed)
        {
            if (currentTool != null)
            {
                currentTool.Use(lastDir);
                if (currentTool.tool == ToolType.SeedBag)
                    ScoreManager.instance.TryPlant(gameObject);
                currentTool.Use(lastDir); 
                if (currentTool.tool == ToolType.WaterGun)
                {
                    currentTool.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                    currentTool.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                }
            }
            else
            {
                if (toolBeneathYou != null)
                {
                    currentTool = toolBeneathYou.PickUp(transform);
                }
            }

            usePressed = true;
        }
        else if (useTriggered && usePressed)
        {
            if (currentTool != null)
            {
                if (currentTool.tool == ToolType.WaterGun)
                {
                    currentTool.Use(lastDir);
                }
            }
                
        }
        else if (!useTriggered && usePressed)
        {
            usePressed = false;

            if (currentTool != null)
            {
                if (currentTool.tool == ToolType.WaterGun)
                {
                    currentTool.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                    currentTool.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
                }
            }
                
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
        anim.Play("Dead");
    }

    public void KnockBack(Vector3 dir)
    {
        currentState = MovementState.knockBacked;

        knockBackDir = dir;
        knockBackTimer = knockBackDuration;

        if (currentTool != null)
        {
            currentTool.Drop();
            currentTool = null;
        }
        
    }

    void Respawn()
    {
        transform.position = spawnPoint.position;
        currentState = MovementState.still;
        anim.Play("Idle");
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
            toolBeneathYou = collision.GetComponent<Tool>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Tool>() == toolBeneathYou)
        {
            toolBeneathYou = null;
        }
    }
}
