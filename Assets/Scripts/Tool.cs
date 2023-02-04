using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ToolType
{
    Axe,
    WaterGun,
    BugSpray
}
public class Tool : MonoBehaviour
{
    CapsuleCollider2D col;
    
    Vector3 throwVector = Vector3.left;
    public Transform _parent;
    public float rayastDistance;
    bool thrown = false;
    public ToolType tool;
    public int Damage;
    RaycastHit2D hit;
    [SerializeField] Quaternion StandardRotation = Quaternion.identity;

    private void Start()
    {
        if(col == null)
        {
            col = gameObject.AddComponent<CapsuleCollider2D>();
            col.isTrigger = true;
        }
    }

    public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //    StartCoroutine(Throwing(throwVector));
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    PickUp(transform);
        //    _parent = hit.transform;
        //    throwVector *= -1;
        //}
    }
    public void Use(Vector2 direction)
    {
        print("Use");
        switch (tool)
        {
            case ToolType.Axe:
                //Melee
                hit = Physics2D.Raycast(origin: transform.position, direction: transform.forward, rayastDistance);
                //If hit => Do Damage
                if(hit && hit.collider.transform.tag == "Player")
                {
                    hit.transform.gameObject.GetComponent<Movement>().Killed();
                }else if(hit && hit.collider.transform.tag == "Tree")
                {
                    hit.transform.gameObject.GetComponent<Tree>().OnAxe();
                }
                break;
            case ToolType.WaterGun:
                //Water
                //If hit == Player Do Damage
                //Else if hit == tree => Give Water
                break;
            case ToolType.BugSpray:
                //Pesticide
                break;
            default:
                break;
        }
    }

    public void Throw(Vector2 Direction)
    {
        transform.parent = null;
    }

    public void Drop()
    {
        transform.parent = null;

    }

    public Tool PickUp(Transform player)
    {
        transform.SetPositionAndRotation(transform.position, StandardRotation);
        transform.SetParent(player);
        _parent = player;
        return this;
    }

    public void Throw(Transform parent, Vector2 direction)
    {
        if(transform.parent == parent)
        {
            StartCoroutine(Throwing(direction));
        }
    }

    IEnumerator Throwing(Vector2 Direction)
    {
        Vector3 direction = (Vector3)Direction;
        thrown = true;
        switch (Direction.x)
        {
            case 1:
                direction = Vector3.right;
                break;
            case -1:
                direction = Vector3.left;
                break;
            default:
                switch (Direction.y)
                {
                    case -1:
                    direction = Vector3.down;
                        break;
                    case 1:
                    direction = Vector3.up;
                        break;
                    default:
                        break;
                }
                break;
        }
        while(thrown)
        {
            hit  = Physics2D.Raycast(origin: transform.position, direction: transform.forward, rayastDistance);
            if (hit && hit.collider.gameObject.transform != _parent && hit.collider.gameObject != gameObject)
            {

                thrown = false;
                //Do damage to hit player
            }else
            {
                transform.Rotate(new Vector3(0,0,3));
                transform.position += direction * Time.deltaTime * 10;
            }
            yield return null;

        }
    }
}
