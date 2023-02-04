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
    public Collider2D collider;
    bool thrown = false;
    public ToolType tool;
    public int Damage;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            StartCoroutine(throwing(Vector3.left));
    }
    public void Use()
    {
        switch (tool)
        {
            case ToolType.Axe:
                //Melee
                //If hit => Do Damage
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
        return this;
    }

    public void Throw(Transform parent)
    {
        if(transform.parent == parent)
        {
            StartCoroutine(throwing(Vector3.left));
        }
    }

    IEnumerator throwing(Vector2 Direction)
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
            if(Physics.Raycast(transform.position, direction, 5))
            {
                thrown = false;
                Debug.Log("Hit something");
                //Do damage to hit player
            }else
            {
                Debug.Log("keep mooving");
                transform.position += direction;
            }
            yield return new WaitForSeconds(.03f);

        }
    }
}
