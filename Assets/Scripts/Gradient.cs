using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gradient : MonoBehaviour {
	
	public Color colorStart = Color.red;  
	public Color colorEnd = Color.green;  
	public  float duration = 1.0f;  
	public  Renderer rend;

	void Start() {
		rend = GetComponent<Renderer>();
	}

	void Update ()
	{ 
		float lerp = Mathf.PingPong (Time.time, duration) / duration;  
		rend.material.color = Color.Lerp (colorStart, colorEnd, lerp);  
	}
}