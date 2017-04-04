using UnityEngine;
using System.Collections;


public class virtualGOChordEvent : TTMonoBehaviour {
	
	EventListenerChord listener;

	public AudioSource source;

	void Awake () {
//		Debug.Log ("AUSOURCE AWAKE @ position: " + transform.position);
		source = gameObject.GetComponent<AudioSource> ();
	}

	void OnEnable()
	{
//		Debug.Log ("AUSOURCE ENABLED");

	}
	
	void OnDisable()
	{

		StopAllCoroutines();

		listener = null;

		source.FadeOutMethod (1.0f);

		gameObject.Recycle();

//		Debug.Log ("AUSOURCE DISABLED");

	}

	public void SetupNPlay (Transform _listener, AudioClip _clip) {

		listener = _listener.GetComponent<EventListenerChord> ();
		source.clip = _clip;
		StartCoroutine(PlayTheSound());

	}
	//Recycle this pooled instance from an external script
	public void RecycleToPool()
	{
		//Recycle this pooled instance
		gameObject.Recycle();
	
	}
		
	public void setTransform (Transform _listener)
	{

	}

	public IEnumerator PlayTheSound()
	{

		source.Play ();
		var waitTime = (float)source.clip.length;

		yield return new WaitForSeconds(waitTime-1.0f);

		yield return StartCoroutine (source.FadeOut (1.0f, () => {}, () => {}));
				
		//Recycle this pooled bullet instance
		gameObject.Recycle();

		yield return null;
	}
}
