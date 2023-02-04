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
    public float raycastDistance;
    bool thrown = false;
    public ToolType tool;
    public int Damage;
    RaycastHit2D[] hits;
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
                hits = Physics2D.RaycastAll(origin: transform.position, direction, raycastDistance);
                //If hit => Do Damage
                foreach (RaycastHit2D hit in hits)
                {
                    if(hit.collider.gameObject.tag == "Player")
                    {
                        hit.transform.gameObject.GetComponent<Movement>().Killed();
                    }
                    else if (hit && hit.transform.tag == "Tree")
                    {
                        hit.transform.gameObject.GetComponent<Tree>().OnAxe();
                    }
                }
                break;
            case ToolType.WaterGun:
                //Water
                hits = Physics2D.RaycastAll(origin: transform.position, direction, raycastDistance * 5);
                //If hit == Player Do Damage
                foreach(RaycastHit2D hit in hits)
                {
                    if(hit.collider.gameObject.tag == "Player")
                    {

                    }else if(hit.collider.gameObject.tag == "Tree")
                    {
                        hit.transform.gameObject.GetComponent<Tree>().OnWater();
                    }
                }
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
        if (thrown)
            return null;

        transform.SetPositionAndRotation(transform.position, StandardRotation);
        transform.SetParent(player);
        _parent = player;
        return this;
    }

    public void Throw(Transform parent, Vector2 direction)
    {
        transform.parent = null;
        if (transform.parent == parent)
        {
            transform.parent = null;
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
        while (thrown)
        {
            hits = Physics2D.RaycastAll(transform.position, direction, raycastDistance);
            foreach (RaycastHit2D h in hits)
            {
                if (h && h.collider.gameObject.transform != _parent && h.collider.gameObject != gameObject)
                {
                    thrown = false;
                    //Do damage to hit player
                    if (h.collider.gameObject.tag == "Player")
                    {
                        h.collider.GetComponent<Movement>().KnockBack(transform.position);

                    }
                }
                else
                {
                    transform.Rotate(new Vector3(0, 0, 3));
                    transform.position += direction * Time.deltaTime * 10;
                }
            }
            
            yield return null;

        }
    }


}
