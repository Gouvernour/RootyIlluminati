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
    public float rayastDistance = .8f;
    bool thrown = false;
    public ToolType tool;
    public int Damage = 1;
    RaycastHit2D[] hits;
    [SerializeField] Quaternion StandardRotation = Quaternion.identity;
	
	
	public Sprite[] eero_sprite_per_tool_type;
	public Collider2D eero_collider;
	Vector3 eero_parent_prev_frame_position;
	float eero_localScale_start;
	float eero_tool_shake_amount = 0;
	
	public GameObject hole;

    float dissapearTimer;

    SpriteRenderer rend;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
		rend.sprite = eero_sprite_per_tool_type[(int)tool];
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
		if (_parent && !thrown ) {
			eero_tool_shake_amount *= 0.9f;
			
			Vector3 delta = _parent.gameObject.GetComponent<Movement>().GetLastDir();
			float len = Vector3.Magnitude(delta);
			if (len > 0.001f) {
				Vector3 dir = delta / len;
				transform.position = Vector3.Lerp(transform.position, _parent.position + dir * 0.8f + new Vector3(0, -0.1f, 0), 40f*Time.fixedDeltaTime);
				transform.localScale = eero_localScale_start * (dir.x > 0 ? new Vector3(-1, 1, 1) : new Vector3(-1, -1, 1));
				
				float theta = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
				theta += 90f * eero_tool_shake_amount * (dir.x > 0 ? -1f : 1f);
				transform.localRotation = Quaternion.Euler(0, 0, theta);
			}
			eero_parent_prev_frame_position = _parent.position;

            if (tool == ToolType.WaterGun)
            {
                if (delta.x > 0)
                {
                    transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 90);
                    transform.GetChild(1).localRotation = Quaternion.Euler(0, 0, 90);
                }
                else
                {
                    transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, -90);
                    transform.GetChild(1).localRotation = Quaternion.Euler(0, 0, -90);
                }

                float a = Mathf.Atan2((delta / len).y, (delta / len).x);
                var main = transform.GetChild(0).GetComponent<ParticleSystem>().main;
                main.startRotation = a - Mathf.PI/2;
                main = transform.GetChild(1).GetComponent<ParticleSystem>().main;
                main.startRotation = a - Mathf.PI / 2;
            }
		}

        if (rend.enabled == false)
        {
            dissapearTimer -= Time.deltaTime;
            if (dissapearTimer <= 0)
            {
                rend.enabled = true;
            }
        }

        
	}
	
    public void Use(Vector2 direction)
    {
		Collider2D[] colliders = new Collider2D[10];
		ContactFilter2D contactFilter = new ContactFilter2D();
		int colliderCount = eero_collider.OverlapCollider(contactFilter.NoFilter(), colliders);
        
		//print("Use ");
		
        switch (tool)
        {
            case ToolType.Axe:
				eero_tool_shake_amount = 1;
				
				for (int i=0; i<colliderCount; i++) {
					if (colliders[i].gameObject.tag == "Tree") {
						colliders[i].gameObject.GetComponent<Tree>().OnAxe();
					}else if (colliders[i].gameObject.tag == "Player" && colliders[i].transform != _parent)
                    {
                        colliders[i].gameObject.GetComponent<Movement>().Killed();
                    }
				}
				
                _parent.GetChild(0).GetComponent<Animator>().Play("Chop");

                AudioManager.instance.Play("Axe_Chopping");

                GetComponent<SpriteRenderer>().enabled = false;
                dissapearTimer = 0.6f;

                break;
            case ToolType.WaterGun:
                //Water

                //hits = Physics2D.RaycastAll(transform.position, direction, rayastDistance*7.5f);
                hits = Physics2D.CircleCastAll(transform.position, 0.4f, direction, rayastDistance*7.5f);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.transform != _parent)
                    {
                        if (hit.collider.gameObject.tag == "Tree")
                        {
                            hit.collider.gameObject.GetComponent<Tree>().OnWater();
                        }
                        if (hit.collider.gameObject.tag == "Player")
                        {
                            hit.collider.gameObject.GetComponent<Movement>().KnockBack(direction);
                        }

                    }
                }
                //If hit == Player Do Damage
                //Else if hit == tree => Give Water
                break;
				
			case ToolType.Shovel:
				eero_tool_shake_amount = -1;
				
				bool found_hole = false;
				for (int i=0; i<colliderCount; i++) {
					if (colliders[i].gameObject.tag == "Holee") {
						colliders[i].gameObject.GetComponent<HoleScript>().Grow();

                        found_hole = true;
					}
				}
				if (!found_hole) {
					Vector3 new_hole_pos = transform.position;
					new_hole_pos.y -= 1f;
					Instantiate(hole, new_hole_pos, Quaternion.identity);
				}
				
				break;
				
			case ToolType.SeedBag:
				eero_tool_shake_amount = -1;
				
				for (int i=0; i<colliderCount; i++) {
					if (colliders[i].gameObject.tag == "Holee") {
						if (colliders[i].gameObject.GetComponent<HoleScript>().TryToPlant()) {
							break;
						}
					}
				}
				
				break;
            //case ToolType.BugSpray:
            //    //Pesticide
            //    break;
			
			case ToolType.Fertilizer:
				for (int i = 0; i < colliderCount; i++) {
					if (colliders[i].gameObject.tag == "Tree") {
						colliders[i].gameObject.GetComponent<Tree>().OnFertilize();
					}
					else if (colliders[i].gameObject.tag == "Playyer")
					{
						//colliders[i].gameObject.GetComponent<Movement>().Killed();
					}
				}
				break;
			
            case ToolType.BugSpray:

                AudioManager.instance.Play("Spray");
                for (int i = 0; i < colliderCount; i++)
                {
                    if (colliders[i].gameObject.tag == "Tree")
                    {
                        colliders[i].gameObject.GetComponent<Tree>().OnSpray();
                    }
                    else if (colliders[i].gameObject.tag == "Player")
                    {
                        //colliders[i].gameObject.GetComponent<Movement>().Killed();
                    }
                }
                break;
            default:
				print("TODO!!!!!!!!!!!!!");
                break;
        }
    }



    public void Drop()
    {
        _parent = null;
    }

    public Tool PickUp(Transform player)
    {
        if (thrown)
            return null;

        transform.SetPositionAndRotation(transform.position, StandardRotation);
        //transform.SetParent(player);
        _parent = player;

        _parent.GetChild(0).GetComponent<Animator>().Play("Pick");


        AudioManager.instance.Play("Pick_Up");

        return this;
    }

    public void Throw(Transform parent, Vector2 direction)
    {
        if (_parent == parent)
        {
            StartCoroutine(Throwing(direction));

            _parent.GetChild(0).GetComponent<Animator>().Play("Throw");

            AudioManager.instance.Play("Item_Throw");
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
            //hits = Physics2D.RaycastAll(transform.position, direction, rayastDistance);
            hits = Physics2D.CircleCastAll(transform.position, 0.4f, direction, rayastDistance);
            foreach (RaycastHit2D h in hits)
            {
                if (h && h.collider.gameObject.transform != _parent && h.collider.gameObject != gameObject 
                    && h.collider.gameObject.tag != "Tool" && h.collider.gameObject.tag != "Holee")
                {
                    thrown = false;
                    //Do damage to hit player
                    if (h.collider.gameObject.tag == "Player")
                    {
                        h.collider.gameObject.GetComponent<Movement>().KnockBack(direction);

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
                    if(h.collider.gameObject.tag == "Wall")
                    {
                        transform.position -= direction * Time.deltaTime * 500;
                    }

                    _parent = null;
                }
                else
                {
                    transform.Rotate(new Vector3(0, 0, 3));
                    transform.position += direction * Time.deltaTime * 15;
                }
            }
            
            yield return null;

        }
    }


}
