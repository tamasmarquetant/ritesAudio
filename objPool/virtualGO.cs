using UnityEngine;
using System.Collections;


public class virtualGO : TTMonoBehaviour {
	
	EventListener listener;

	AudioSource source;

	void OnEnable()
	{

		source = gameObject.GetComponent<AudioSource> ();

//		Debug.Log ("AUSOURCE ENABLED");
//		StartCoroutine(PlayTheSound());
	}
	
	void OnDisable()
	{
//		transformer.gameObject

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


//	public IEnumerator PlayTheSound()
//	{
//
//		source.Play ();
//		var waitTime = (float)source.clip.length;
//
//		yield return new WaitForSeconds(waitTime);
//				
//		//Recycle this pooled bullet instance
//		gameObject.Recycle();
//
//		yield return null;
//	}

}
