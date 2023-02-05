using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// items:
// - shovel
// - seed bag
// - fertilizer
// - water gun
// - axe

public class Tree : MonoBehaviour {
	//- constants ----------
	
	public SpriteRenderer sprite_renderer;
	public SpriteRenderer wish_sprite_renderer;
	public GameObject root;
	public GameObject WishCloud;
	
	public Sprite[] sprites;
	public Sprite[] trunk_sprites;
	public Sprite[] cut_top_sprites;
	
	public Sprite[] wish_sprites;
	
	public GameObject cut_tree_top_object;
	
	enum WishKind {
		Water,
		Fertilize,
		//Axe,
		Spray,
	}
	
	//----------------------
	
	WishKind active_wish = WishKind.Water;
	int active_wishes_remaining = 0;
	
	float grown_percentage = 0f;
	float next_wish_timer = 0;
	
	float growing_speed = 0;
	
	// Start is called before the first frame update
	void Start() {
		next_wish_timer = Random.Range(4f, 8f);
		ScoreManager.instance.AddTree(this);
	}
	
	float shake_amount = 0f;
	
	void ReceiveTreatment(WishKind kind) {
		if (active_wishes_remaining > 0 && kind == active_wish) {
			shake_amount = 0.5f;
			active_wishes_remaining--;
			growing_speed = 1;
			
			if (active_wishes_remaining == 0) {
				// fulfilled the wish!
				next_wish_timer = Random.Range(4f, 8f);
			}
		}
	}
	
	
	// when you axe, this goes to 1. When this goes to 0, reset HP.
	float reset_hp_timer = 0;
	int hp = 5;
	//float fallen_over_ratio = 0;
	
	//int GetMaxHP() {
	//	return 5 + (int)(grown_percentage * 5f);
	//}
	
	public int GetSpriteIndex() { return (int)(grown_percentage * sprites.Length); }
	
	public void OnAxe() {
		//print("OnAxe");
		reset_hp_timer = 1;
		hp--;
		//ReceiveTreatment(WishKind.Axe);
		if (hp == 0) {
			GameObject top = Instantiate(cut_tree_top_object, transform.position, Quaternion.identity);
			top.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-3f, 3f), ForceMode2D.Impulse);
			top.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-3f, 3f), 1f), ForceMode2D.Impulse);
			top.GetComponent<SpriteRenderer>().sprite = cut_top_sprites[GetSpriteIndex()];
			sprite_renderer.sprite = trunk_sprites[GetSpriteIndex()];
		}
		
		shake_amount = 1;
	}
	
	public void OnSpray() {
		ReceiveTreatment(WishKind.Spray);
	}
	
	public void OnFertilize() {
		ReceiveTreatment(WishKind.Fertilize);
	}
	
	public void OnWater() {
		ReceiveTreatment(WishKind.Water);
	}
	
	//float test_timer = 1f;
	
	
	void FixedUpdate() {
		wish_sprite_renderer.sprite = null;
		if (active_wishes_remaining > 0) {
			wish_sprite_renderer.sprite = wish_sprites[(int)active_wish];
		}
		
		next_wish_timer -= Time.fixedDeltaTime;
		reset_hp_timer -= Time.fixedDeltaTime;
		
		if (hp > 0 && reset_hp_timer < 0) {
			hp = 5; //GetMaxHP();
		}
		
		WishCloud.SetActive(hp > 0 && active_wishes_remaining > 0);
		
		shake_amount *= 0.9f;
		growing_speed *= 0.9f;
		
		/*if (Time.time > 2f) {
			test_timer -= Time.fixedDeltaTime;
			if (test_timer < 0) {
				OnAxe();
				test_timer = 0.5f;
			}
		}*/
		
		int sprite_index = GetSpriteIndex();
		
		if (hp <= 0) {
			// dead tree
			//fallen_over_ratio += 2f*Time.fixedDeltaTime;
			//if (fallen_over_ratio > 1) fallen_over_ratio = 1;
			
			//root.transform.localRotation = Quaternion.Euler(0, 0, fallen_over_ratio*90f);
			
			return;
		}
		
		if (active_wishes_remaining == 0 && sprite_index < 4) { // should grow?
			
			if (next_wish_timer < 0) {
				active_wish = (WishKind)Random.Range(0, System.Enum.GetNames(typeof(WishKind)).Length);
				//active_wish = WishKind.Axe;
				active_wishes_remaining = 1;
			}
			
			sprite_renderer.sprite = sprites[sprite_index % sprites.Length];
			
			//grown_percentage += 0.15f*Time.fixedDeltaTime;
			grown_percentage += 1.25f*growing_speed*Time.fixedDeltaTime;
			//print("grown_percentage " + grown_percentage);
			
			float s = 0.2f*grown_percentage + 1f;
			root.transform.localScale = new Vector3(s, s, 1);	
			WishCloud.transform.localPosition = new Vector3(0.5f, grown_percentage * 3.2f + 1.5f, 0);
		}
		
		root.transform.localRotation = Quaternion.Euler(0, 0, 20f * shake_amount * Mathf.Sin(Time.time*25f));
		
		//if (grown_percentage > 1) grown_percentage = 0; // reset
	}
	
	void Update() {
	}
}
