using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleScript : MonoBehaviour
{
	bool has_plant = false;
	bool is_complete = false;
	
	public GameObject tree_object;
	public Sprite dug_hole_sprite;
	
	public bool TryToPlant() {
		if (has_plant) return false;
		if (!is_complete) return false;
		
		has_plant = true;
		
		Instantiate(tree_object, new Vector3(transform.position.x, transform.position.y + 0.5f, 0), Quaternion.identity);
		return true;
		//print("plant!!!!!!!");
	}
	
	public void Grow() {
		GameObject hole_sprite = transform.GetChild(0).gameObject;
		float s = hole_sprite.transform.localScale.x;
		
		if (s < 0.7f) {
			float new_s = s + 0.1f;
			hole_sprite.transform.localScale = new Vector3(new_s, new_s, new_s);
		}
		else {
			is_complete = true;
			hole_sprite.GetComponent<SpriteRenderer>().sprite = dug_hole_sprite;
		}

        AudioManager.instance.Play("Dig");
    }
	
	// Start is called before the first frame update
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
