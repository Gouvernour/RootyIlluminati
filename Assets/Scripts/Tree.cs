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
	
	public Sprite[] wish_sprites;
	
	enum WishKind {
		Water,
		Fertilize,
		Axe,
	}
	
	//----------------------
	
	WishKind active_wish = WishKind.Water;
	int active_wishes_remaining = 0;
	
	float grown_percentage = 0f;
	float next_wish_timer = 3f;
	
	// Start is called before the first frame update
	void Start() {
	}
	
	void ReceiveTreatment(WishKind kind) {
		if (active_wishes_remaining > 0 && kind == active_wish) {
			active_wishes_remaining--;
			
			if (active_wishes_remaining == 0) {
				// fulfilled the wish!
				next_wish_timer = 3f;
			}
		}
	}
	
	float axe_shake_amount = 0f;
	
	// when you axe, this goes to 1. When this goes to 0, reset HP.
	float reset_hp_timer = 0;
	int hp = 5;
	float fallen_over_ratio = 0;
	
	//int GetMaxHP() {
	//	return 5 + (int)(grown_percentage * 5f);
	//}
	
	public void OnSpray() {
	}
	
	public void OnAxe() {
		print("OnAxe");
		axe_shake_amount = 1;
		reset_hp_timer = 1;
		hp--;
		ReceiveTreatment(WishKind.Axe);
	}
	
	public void OnFertilize() {
		ReceiveTreatment(WishKind.Fertilize);
	}
	
	public void OnWater() {
		ReceiveTreatment(WishKind.Water);
	}
	
	float test_timer = 1f;
	
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
		
		WishCloud.SetActive(active_wishes_remaining > 0);
		
		axe_shake_amount *= 0.9f;
		
		/*if (Time.time > 2f) {
			test_timer -= Time.fixedDeltaTime;
			if (test_timer < 0) {
				OnAxe();
				test_timer = 0.5f;
			}
		}*/
		
		if (hp <= 0) {
			// dead tree
			fallen_over_ratio += 2f*Time.fixedDeltaTime;
			if (fallen_over_ratio > 1) fallen_over_ratio = 1;
			
			root.transform.localRotation = Quaternion.Euler(0, 0, fallen_over_ratio*90f);
			return;
		}
		
		if (active_wishes_remaining == 0 && grown_percentage < 1f) { // should grow?
			
			//if (next_wish_timer < 0) {
			//	active_wish = (WishKind)Random.Range(0, System.Enum.GetNames(typeof(WishKind)).Length);
			//	active_wishes_remaining = 3;
			//}
			
			int sprite_index = (int)(grown_percentage * sprites.Length);
			sprite_renderer.sprite = sprites[sprite_index % sprites.Length];
			
			grown_percentage += 0.15f*Time.fixedDeltaTime;
			//grown_percentage += 0.02f*Time.fixedDeltaTime;
			
			float s = 0.2f*grown_percentage + 1f;
			root.transform.localScale = new Vector3(s, s, 1);	
			WishCloud.transform.localPosition = new Vector3(0, grown_percentage * 3.2f + 0.9f, 0);
		}
		
		root.transform.localRotation = Quaternion.Euler(0, 0, 20f * axe_shake_amount * Mathf.Sin(Time.time*25f));
		
		//if (grown_percentage > 1) grown_percentage = 0; // reset
	}
	
	void Update() {
	}
}
