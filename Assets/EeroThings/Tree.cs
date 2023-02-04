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
		if (next_wish_timer < 0 && active_wish == WishKind.None) {
			active_wish = (WishKind)Random.Range(0, System.Enum.GetNames(typeof(WishKind)).Length);
		}
		
		// should grow?
		
		WishCloud.SetActive(active_wish != WishKind.None);
		
		if (active_wish == WishKind.None && grown_percentage < 1f) {
			int sprite_index = (int)(grown_percentage*50f * sprites.Length);
			sprite_renderer.sprite = sprites[sprite_index % sprites.Length];
			
			grown_percentage += 0.02f*Time.fixedDeltaTime;
			
			float s = grown_percentage*grown_percentage;
			root.transform.localScale = new Vector3(grown_percentage, grown_percentage, 1);
			
			WishCloud.transform.localPosition = new Vector3(0, grown_percentage * 3f + 1f, 0);
		}
	}
	
	void Update() {
	}
}
