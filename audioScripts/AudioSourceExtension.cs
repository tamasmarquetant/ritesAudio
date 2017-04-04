using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public static class AudioSourceExtensions
{
	
	public static void playClip( this AudioSource audioSource, AudioClip audioClip )
	{
		audioSource.clip = audioClip;
		audioSource.Play();
	}
	
	
	public static IEnumerator playLoop( this AudioSource audioSource, AudioClip audioClip )
	{
		audioSource.loop = true;
		audioSource.playClip( audioClip );
//		Debug.Log ("SOURCE STARTS PLAYING");
		
		while( audioSource.isPlaying )
			yield return null;

	}
		
	public static void playRandomClip( this AudioSource audioSource, AudioClip[] clips )
	{
		int clipIndex = UnityEngine.Random.Range( 0, clips.Length );
		audioSource.playClip( clips[clipIndex] );
	}
		
	public static IEnumerator fadeOutWithFixedDuration( this AudioSource audioSource, AudioClip audioClip, float duration, Action onComplete )
	{
		audioSource.playClip( audioClip );
		
		var startingVolume = audioSource.volume;
		
		// fade out the volume
		while( audioSource.volume > 0.0f )
		{
			audioSource.volume -= Time.deltaTime * startingVolume / duration;
			yield return null;
		}
		
		// done fading out
		if( onComplete != null )
			onComplete();
	}

	public static IEnumerator PlayWithFades( this AudioSource audioSource, AudioClip audioClip, float fadeDuration, Action onComplete, Action startFadeout, float Volume = 1.0f )
	{
		audioSource.playClip( audioClip );
		
		var startingVolume = audioSource.volume;
		var startedClipAtTime = Time.time;
		var startFadeOutAtTime = audioClip.length - fadeDuration;
		
		if( onComplete != null )
			onComplete();
		
		yield return null;
		
	}
	
	public static IEnumerator FadeIn(this AudioSource a, float timeToFade, Action onComplete, float targetVolume = 1.0f) 
	{
		float startTime = Time.time;
		float elapsedTime = 0f;

		do {
			elapsedTime = Time.time - startTime;
			a.volume = targetVolume * elapsedTime / timeToFade;
			yield return null;
		} while ((elapsedTime < timeToFade) && (a.isPlaying));
		a.volume = targetVolume;
		
		// done fading in
		if( onComplete != null )
			onComplete();
	}
	
	
	public static IEnumerator FadeOut(this AudioSource a, float timeToFade, Action onStart, Action onComplete) {

		if (a) {

			// start fading out
			if( onStart != null )
				onStart();

//			Debug.Log ("We are WithInFadeOutMethod");

			float startTime = Time.time;
			float elapsedTime = 0f;
			var startingVolume = a.volume;

			do {
					elapsedTime = Time.time - startTime;
					a.volume = startingVolume * (1f - elapsedTime / timeToFade);
//				Debug.Log ("ELAPSED TIME: " + elapsedTime);
					yield return null;
			} while ((elapsedTime < timeToFade) && (a.isPlaying));
			a.volume = 0f;

			// done fading out
			if (onComplete != null)
					onComplete ();
	}
	}

	public static void FadeOutMethod (this AudioSource a, float timeToFade) {
		
		if (a) {
			
			//			Debug.Log ("We are WithInFadeOutMethod");
			
			float startTime = Time.time;
			float elapsedTime = 0f;
			var startingVolume = a.volume;
			
			do {
				elapsedTime = Time.time - startTime;
				a.volume = startingVolume * (1f - elapsedTime / timeToFade);
				//				Debug.Log ("ELAPSED TIME: " + elapsedTime);
//				yield return null;
			} while ((elapsedTime < timeToFade) && (a.isPlaying));
			a.volume = 0f;
			
			// done fading out
		}
	}

}
