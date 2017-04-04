using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public static class AudioExtension
	
{

	/* overload method for chipChop() for the case when you need all chopped parts in an string array */
	
	public static string[] chipChopper (this GameObject _obj, int chopAt){
		
		string bar = _obj.name.ToString(); // assign object name to string bar variable 
		string[] foo = bar.Chop (chopAt); 
		//foreach (string f in foo) //use if all chopped string parts are needed
		//Debug.Log (string.Format ("[{0}]", f)); //use if all chopped string parts are needed
		return foo;
		
	}
	/* overload method for chipChop() for the case when you only need one particular chopped parts */
	
	public static string chipChop (this GameObject _obj, int chopAt, int choppedElement){
		
		string bar = _obj.name.ToString(); // assign object name to string bar variable 
		string[] foo = bar.Chop (chopAt);
		return foo[choppedElement];
		
	}
	
	public static string chipChopi (this AudioClip _obj, int chopAt, int choppedElement){
		
		string bar = _obj.name.ToString(); // assign object name to string bar variable 
		string[] foo = bar.Chop (chopAt);
		return foo[choppedElement];
		
	}


	public static void addEventListenerAmBt(this GameObject _obj, string _audioPath){
		
		EventListenerAmBt eventListener;
		
		eventListener = _obj.AddComponent<EventListenerAmBt> ();
		eventListener.FillClipList (_audioPath);

	}
	
	public static void addEventListenerLdSc(this GameObject _obj, string _audioPath){
		
		EventListenerLdSc eventListener;
		
		eventListener = _obj.AddComponent<EventListenerLdSc> ();
		eventListener.FillClipList (_audioPath);

	}
	
	public static void addEventListenerSpCh(this GameObject _obj, string _audioPath){
		
		EventListenerSpCh eventListener;
		
		eventListener = _obj.AddComponent<EventListenerSpCh> ();
		eventListener.FillClipList (_audioPath);

	}

	public static void addEventListenerMnCh(this GameObject _obj, string _audioPath){
		
		EventListenerMnCh eventListener;
		
		eventListener = _obj.AddComponent<EventListenerMnCh> ();
		eventListener.FillClipList (_audioPath);

	}

	/ <summary>
	/ Switch statement to call various audio setup functions on GOs based on their characterType
	/ </summary>
	/ <param name="_charType">_char type.</param>
	public static void DefCharacter (this GameObject _toCreate, CharacterType.CharacterTypeEnum _charType)
	{
		//CharacterType _charType;				// type of character
		//int a = (int)_charType; 		// type cast the _charType enum to an int variable
		//int nmbOfSources;
		string audioPath;
		
		switch (_charType)
		{			
		case CharacterType.CharacterTypeEnum.amBt: // ambience object 
			audioPath = "ambiance"; // audioPath to load ambience audioclips for the scene
			_toCreate.addEventListenerAmBt(audioPath);
			break;
		case CharacterType.CharacterTypeEnum.ldSc: // landscape object
			audioPath = "ldSc"; // audioPath to load
			_toCreate.addEventListenerLdSc(audioPath);
			break;
		case CharacterType.CharacterTypeEnum.mnCh: // main character
			// add function
			break;
		case CharacterType.CharacterTypeEnum.spCh: // supplemetary character
			audioPath = "supplChar";
			_toCreate.addEventListenerSpCh(audioPath);
			break;
		case CharacterType.CharacterTypeEnum.epSd: // episode object
			audioPath = "Episode";
			// add function
			break;
		}
	}

	public static void addAudioSources (this GameObject gameO, int nmbOfSources, AudioMixer mx ) {

		//AudioSource source = gameO.AddComponent<AudioSource> (); // add required number of audioSources to gameObject

		for (int i=0; i < nmbOfSources; i++) {
			AudioSource source = gameO.AddComponent<AudioSource> (); // add required number of audioSources to gameObject
			source.playOnAwake = false;
			AudioMixerGroup[] gr;
			//AudioMixer[] mrG;
			//mrG = GameObject.FindObjectsOfType<AudioMixer>();
			gr = mx.FindMatchingGroups("Master");
			source.outputAudioMixerGroup = gr [0];
			//Debug.Log (gr[0].name); 
		}
	}

	/// <summary>
	/// Sets up the audio components on the calling gameObject
	/// </summary>
	/// <param name="_obj">_obj.</param>
	/// <param name="_soundType">_sound type.</param>

	public static void setupAudioManager(this GameObject _obj, int _soundType, string _audioPath, int nmbOfSources){

		AudioManager audioManager;

		audioManager = _obj.AddComponent<AudioManager> ();
		audioManager.SetupAudioManager (_soundType, nmbOfSources);  // ezt a funkciót meg lehet szüntetni, redundant! 
		audioManager.setUpAudioSlots (_audioPath); 
		audioManager.Play (2);
	}

	public static void setupAudioManagerExtended(this GameObject _obj, int _soundType, string _audioPath, int nmbOfSources){
		
		AudioManagerExtended audioManagerExtended;
		
		audioManagerExtended = _obj.AddComponent<AudioManagerExtended>();
		audioManagerExtended.SetupAudioManager (_soundType, nmbOfSources);  // ezt a funkciót meg lehet szüntetni, redundant! 
		audioManagerExtended.setUpAudioSlots (_audioPath); 
		audioManagerExtended.Play (2);

		switch (_soundType)
		{
			
		case 0: // ambience object
			nmbOfSources = 2;
			_obj.addAudioSources (nmbOfSources, MainLoop._mx); // calls AudioExtension for required AU sources

			// -------- create an empty object above the orig object for ambient sound sources ------
			// PrimitiveType primitiveType; 
			// GameObject obj = GameObject.CreatePrimitive (primitiveType);
			// transform.position ---> use extension method to only change vector3.x (magasság?) 
			//public static void SetPositionX(this Transform t, float newX)
			//{
			//	t.position = new Vector3(newX, t.position.y, t.position.z);
			//}
			//as seen in http://unitypatterns.com/extension-methods/

			break;
		case 1: // landscape object
			// nmbOfSources = 2; // no action 
			break;
		case 2: // main character
			nmbOfSources = 1;
			_obj.addAudioSources (nmbOfSources, MainLoop._mx); // calls AudioExtension for required AU sources
			break;
		case 3: // supplemetary character
			nmbOfSources = 1;
			_obj.addAudioSources (nmbOfSources, MainLoop._mx); // calls AudioExtension for required AU sources
			//goto case CharacterType.amBt;
			break;			
		}

	public static void blendBetween (this GameObject gameO) {
			
	}

	/// <summary>
	/// Plays the clip and returns onComplete.
	/// </summary>
	/// <returns>The clip.</returns>
	/// <param name="audioSource">Audio source.</param>
	/// <param name="audioclip">Audioclip.</param>
	/// <param name="onComplete">On complete.</param>
	public static IEnumerator playClip (this AudioSource audioSource, AudioClip audioclip, Action onComplete){
		
		audioSource.playClip (audioclip);
		
		while (audioSource.isPlaying)
			yield return null;

		onComplete ();
	}

	public static void playClip (this AudioSource audioSource, AudioClip audioclip) {
		
		audioSource.clip = audioclip;
		audioSource.Play ();
	}

	public static AudioSource addClip( this GameObject Obj, AudioClip clipToAdd, float volume){
		
		AudioSource srcAdd = Obj.AddComponent<AudioSource> ();
		
		srcAdd.clip = clipToAdd;
		srcAdd.volume = volume;
		return srcAdd;
	}
	
}

