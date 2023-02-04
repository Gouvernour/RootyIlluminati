using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {
	//- constants ----------
	
	public SpriteRenderer sprite_renderer;
	public SpriteRenderer wish_sprite_renderer;
	public GameObject root;
	public GameObject WishCloud;
	
	public Sprite[] sprites;
	
	public Sprite[] wish_sprites;
	
	enum WishKind {
		None,
		Water,
		Fertilize,
		Axe,
	}
	
	//----------------------
	
	WishKind active_wish = WishKind.None;
	
	float grown_percentage = 0f;
	float next_wish_timer = 3f;
	
	// Start is called before the first frame update
	void Start() {
	}
	
	void ReceiveTreatment(WishKind kind) {
		if (kind == active_wish) {
			next_wish_timer = 3f;
		}
	}
	
	public void OnAxe() {
		ReceiveTreatment(WishKind.Axe);
	}
	
	public void OnFertilize() {
		ReceiveTreatment(WishKind.Fertilize);
	}
	
	public void OnWater() {
		ReceiveTreatment(WishKind.Water);
	}
		
	void FixedUpdate() {
		wish_sprite_renderer.sprite = wish_sprites[(int)active_wish];
		
		next_wish_timer -= Time.fixedDeltaTime;
		
		
		WishCloud.SetActive(active_wish != WishKind.None);
		
		// should grow?
		if (active_wish == WishKind.None && grown_percentage < 1f) {
			
			if (next_wish_timer < 0) {
				active_wish = (WishKind)Random.Range(0, System.Enum.GetNames(typeof(WishKind)).Length);
			}
			
			int sprite_index = (int)(grown_percentage * sprites.Length);
			sprite_renderer.sprite = sprites[sprite_index % sprites.Length];
			
			grown_percentage += 0.15f*Time.fixedDeltaTime;
			//grown_percentage += 0.02f*Time.fixedDeltaTime;
			
			float s = 0.4f*grown_percentage + 0.2f;
			root.transform.localScale = new Vector3(s, s, 1);
			
			WishCloud.transform.localPosition = new Vector3(0, grown_percentage * 3.2f + 0.9f, 0);
		}
		
		//if (grown_percentage > 1) grown_percentage = 0; // reset
	}
	
	void Update() {
	}
}
