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
    bool thrown = false;
    public ToolType tool;
    public int Damage;
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

    public void throwing(Vector2 Direction)
    {
        Vector3 direction = Vector3.left;
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
        Collider2D col;
        RaycastHit2D hit = Physics.Raycast(transform.position, direction, 1f, );
    }
}
