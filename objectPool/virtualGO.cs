﻿using UnityEngine;
using System.Collections;


public class virtualGO : TTMonoBehaviour {
	
	EventListener listener;

	AudioSource source;

	void OnEnable()
	{
		source = gameObject.GetComponent<AudioSource> ();
//		Debug.Log ("AUSOURCE ENABLED");
	}
	
	void OnDisable()
	{
//		StopAllCoroutines();
//		Debug.Log ("AUSOURCE DISABLED");
	}

	public void RecycleToPool()
	{
		//Recycle this pooled instance
		gameObject.Recycle();
	
	}
	
	public void setTransform (Transform _listener)
	{
		listener = _listener.GetComponent<EventListener> ();
	}
}
