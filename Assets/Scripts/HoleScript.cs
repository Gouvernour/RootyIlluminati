using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleScript : MonoBehaviour
{
	bool has_plant = false;
	bool is_complete = false;
	
	public GameObject tree_object;
	
	public void TryToPlant() {
		if (has_plant) return;
		if (!is_complete) return;
		
		has_plant = true;
		
		Instantiate(tree_object, new Vector3(transform.position.x, transform.position.y + 0.5f, 0), Quaternion.identity);
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
		}
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
