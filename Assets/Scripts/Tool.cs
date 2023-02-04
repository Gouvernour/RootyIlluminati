using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ToolType
{
    Axe,
    WaterGun,
    Fertilizer,
    Shovel,
    SeedBag,
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
    RaycastHit2D[] hits;
    [SerializeField] Quaternion StandardRotation = Quaternion.identity;
	
	
	public Collider2D eero_collider;
	public Sprite[] eero_sprite_per_tool_type;
	Vector3 eero_parent_prev_frame_position;
	float eero_localScale_start;
	float eero_tool_shake_amount = 0;
	
	public GameObject hole;
	
    private void Start()
    {
		GetComponent<SpriteRenderer>().sprite = eero_sprite_per_tool_type[(int)tool];
		eero_localScale_start = transform.localScale.x;
		
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
	
	public void FixedUpdate() {
		if (_parent) {
			eero_tool_shake_amount *= 0.9f;
			
			Vector3 delta = _parent.gameObject.GetComponent<Movement>().GetLastDir();
			float len = Vector3.Magnitude(delta);
			if (len > 0.001f) {
				Vector3 dir = delta / len;
				transform.position = Vector3.Lerp(transform.position, _parent.position + dir * 0.8f, 40f*Time.fixedDeltaTime);
				transform.localScale = eero_localScale_start * (dir.x > 0 ? new Vector3(-1, 1, 1) : new Vector3(-1, -1, 1));
				
				float theta = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
				theta += 90f * eero_tool_shake_amount * (dir.x > 0 ? -1f : 1f);
				transform.localRotation = Quaternion.Euler(0, 0, theta);
			}
			eero_parent_prev_frame_position = _parent.position;
		}
	}
	
    public void Use(Vector2 direction)
    {
		Collider2D[] colliders = new Collider2D[10];
		ContactFilter2D contactFilter = new ContactFilter2D();
		int colliderCount = eero_collider.OverlapCollider(contactFilter.NoFilter(), colliders);
        
		print("Use " + colliderCount);
		
        switch (tool)
        {
            case ToolType.Axe:
				eero_tool_shake_amount = 1;
				
				for (int i=0; i<colliderCount; i++) {
					if (colliders[i].gameObject.tag == "Tree") {
						colliders[i].gameObject.GetComponent<Tree>().OnAxe();
					}else if(colliders[i].gameObject.tag == "Playyer")
                    {
                        colliders[i].gameObject.GetComponent<Movement>().Killed();
                    }
				}
				
                break;
            case ToolType.WaterGun:
                //Water
                //hit = Physics2D.Raycast(origin: transform.position, direction, rayastDistance * 5);
                //If hit == Player Do Damage
                //Else if hit == tree => Give Water
                break;
				
			case ToolType.Shovel:
				eero_tool_shake_amount = 1;
				
				bool found_hole = false;
				for (int i=0; i<colliderCount; i++) {
					print("colliders[i].gameObject.tag " + colliders[i].gameObject.tag);
					if (colliders[i].gameObject.tag == "Holee") {
						GameObject hole_sprite = colliders[i].gameObject.transform.GetChild(0).gameObject;
						float s = hole_sprite.transform.localScale.x;
						if (s < 1) {
							float new_s = s + 0.1f;
							hole_sprite.transform.localScale = new Vector3(new_s, new_s, new_s);
						}
						found_hole = true;
					}
				}
				if (!found_hole) {
					GameObject h = Instantiate(hole, transform.position, Quaternion.identity);
					//h.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
				}
				
				break;
            //case ToolType.BugSpray:
            //    //Pesticide
            //    break;
            default:
				print("TODO!!!!!!!!!!!!!");
                break;
        }
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
        //transform.SetParent(player);
        _parent = player;
        return this;
    }

    public void Throw(Transform parent, Vector2 direction)
    {
        if(transform.parent == parent)
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
            hits = Physics2D.RaycastAll(transform.position, direction, rayastDistance);
            foreach (RaycastHit2D h in hits)
            {
                if (h && h.collider.gameObject.transform != _parent && h.collider.gameObject != gameObject)
                {
                    thrown = false;
                    //Do damage to hit player
                    if(h.collider.gameObject.tag == "Player")
                    {
                        switch (tool)
                        {
                            case ToolType.Axe:
                                h.collider.gameObject.GetComponent<Movement>().Killed();
                                break;
                            case ToolType.WaterGun:
                                break;
                            case ToolType.BugSpray:
                                break;
                            default:
                                break;
                        }
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
