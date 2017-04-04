	// http://dirigiballers.blogspot.hu/2013/03/unity-c-audiomanager-tutorial-part-1.html

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.Linq;


public class Singleton<T> : TTMonoBehaviour where T : TTMonoBehaviour 
{
	protected static T instance;
	
	//Returns the instance of this singleton
	public static T Instance {
		get {
			if (instance == null) {
				instance = (T)FindObjectOfType(typeof(T));
				if (instance == null) {
					GameObject container = new GameObject();
					container.name = typeof(T)+"Container";
					instance = (T)container.AddComponent(typeof(T));
				}
			}
			return instance;
		}
	}
}


public class AudioManager : Singleton<AudioManager> 
{

	[System.Serializable]
	public class ClipInfo
	{
		//ClipInfo used to maintain default audio source info
//		public AudioSource source { get; set; }
//		public float defaultVolume { get; set; }
//		public float startedPlaying { get; set; }
		public AudioSource source;
		public float defaultVolume;
		public float startedPlaying;
		public AudioClip clip;
//		public AudioClip clip { get; set; }
		public GameObject realGObject;
//		public GameObject realGObject { get; set;}

	}

	//EZT A CLASST MÉG NEM TUDJUK HASZNÁLNI !!!!
	public class AudioMixExtension
	{
		
		public AudioMixer audioMixer = Main._mx;
		
		public AudioMixerGroup[] AllMixerGroups {
			get{
				return audioMixer.FindMatchingGroups(string.Empty);
				;}
		}
	}

	public Dictionary<Transform, List<Coroutine<string>>> CoroutineList { get; set; }

	public List<ClipInfo> m_activeAudio;
	public List<ClipInfo> m_activeLoop;	
	public List<ClipInfo> m_activeScore;	
	
	public AudioMixer mx; 
	
	private float m_volumeMod, m_volumeMin;
	private bool m_VOfade; //used to fade to quiet for VO
	public bool ScoreVol;
	public float fadeTime;

	// our fade animation clip
	public AnimationClip fadeClip;
	
	void Awake() 
	{
		//		Debug.Log("AudioManager Initializing");
		
		
		try {
			transform.parent = GameObject.FindGameObjectWithTag("Player").transform;
			transform.localPosition = new Vector3(0, 0, 0);
		} catch {
			Debug.Log("Unable to find main camera to put audiomanager");
		}
		
		CoroutineList = new Dictionary<Transform, List<Coroutine<string>>> ();
		
		m_activeAudio = new List<ClipInfo>();
		m_activeLoop = new List<ClipInfo>();
		m_activeScore = new List<ClipInfo>();
		m_volumeMod = 1.0f;
		m_volumeMin = 0.2f;
		m_VOfade = false;
		//m_moreSoundPresent = false;
		m_activeVoiceOver = null;
		m_activeMusic = null;
		//m_devideVol = 1;

		fadeTime = 10.0f;
		
	}
	
	public void Starter(){
				
		mx = Main._mx;
		
	}

	/// <summary>
	/// Plays audio clips with fade-in/out. subscribe to Action startNext for cross-fade transitions.
	/// </summary>
	/// <returns>The audio clipswith fade.</returns>
	/// <param name="clip">Clip.</param>
	/// <param name="emitter">Emitter.</param>
	/// <param name="volume">Volume.</param>
	/// <param name="mixerGroup">Mixer group.</param>
	/// <param name="crossFadeTime">Cross fade time.</param>
	
	public IEnumerator PlayAudioClipswithFade (AudioSource _audioSource, AudioClip clip, Transform emitter, float volume, Action onComplete, Action startNext, float crossFadeTime = 3.0f )
	{
		
		var playedclip = clip;
		float m_volumeMod = volume;
		float timeToWait;
		
		// set up auSource on virtualGO gameobject 
		
		setSource(_audioSource, clip, volume);
		yield return null;
		
		// set maximum fade time
		if (clip.length <= fadeTime * 2) 
		{
			fadeTime = clip.length / 2;
		}
		else
		{
			fadeTime = 10.0f;
		}
		
		yield return null;
		
		
		// Start payback
		var PlayAsLoop = _audioSource.playLoop(playedclip);
		StartCoroutineWithRefandAddToDict (emitter, PlayAsLoop);

		var specialFadeInTime = 6.0f;
		
		// Start fade-in routine
		if (fadeTime > 0f )
		{
//			var FadeIn = AudioHelper.FadeAudioObject(_audioSource, 1.0f);
			var FadeIn = _audioSource.FadeIn(fadeTime - specialFadeInTime, () => {}, m_volumeMod);
			
			StartCoroutineWithRefandAddToDict (emitter, FadeIn);
		} else
			_audioSource.volume = m_volumeMod;
		
		yield return null;
		
		// calculate time to wait before fade-out && wait if necessary 
		timeToWait = _audioSource.clip.length - fadeTime - _audioSource.time + specialFadeInTime;
		
		if (timeToWait > 0f) {
			yield return new WaitForSeconds (timeToWait - 1.0f);
		} else {
			yield return null;
		}
		
		// fade-out volume on audioClip if fadeTime is set
		if (fadeTime > 0f){
			
			// call StartNext event to call next clips playback for crossfade
			if (startNext != null)
				startNext ();
			
			var FadeOut = _audioSource.FadeOut(fadeTime, () => {},() => {});
			
			while ((_audioSource.isPlaying) && (_audioSource.time + fadeTime < _audioSource.clip.length))
				yield return null;
			
			yield return StartCoroutine(FadeOut);
			
		} // no fade-out if fadeTime is null
		else {
			while (_audioSource.isPlaying)
				yield return null;
		}
		
		// call onComplete event when done fading out
		if (onComplete != null)
			onComplete ();
		
		yield return null;
		
	}

	//UNUSED 
	public void pauseFX() { 
		foreach (var audioClip in m_activeAudio) {
			try {
				if (audioClip.source != m_activeMusic) {
					audioClip.source.Pause();
				}
			} catch {
				continue;
			}
		}
	}

	//UNUSED
	public void unpauseFX() {
		foreach (var audioClip in m_activeAudio) {
			try {
				if (!audioClip.source.isPlaying) {
					audioClip.source.Play();
				}
			} catch {
				continue;
			}
		}
	}
	
	private AudioSource m_activeVoiceOver;
	
	
	private void setSource(AudioSource source, AudioClip clip, float volume) {
		//		source.rolloffMode = AudioRolloffMode.Logarithmic;
		source.dopplerLevel = 0.13f;
		//		source.minDistance = 15;
		source.maxDistance = 30;
		source.clip = clip;
		source.volume = volume;
	}
	
	private void setSourceCharacter(AudioSource source, AudioClip clip, float volume) {
		//		source.rolloffMode = AudioRolloffMode.Logarithmic;
		source.dopplerLevel = 0.13f;
		//		source.minDistance = 10;
		source.maxDistance = 15;
		source.clip = clip;
		source.volume = volume;
	}

	/// <summary>
	/// Starts Coroutine Coroutine<T> with a reference to the emitter object and adds coroutine to dictionary. These coroutines have a functionality to pause, uppause and to be killed.
	/// </summary>
	/// <param name="_transform">_transform.</param>
	/// <param name="coroutine">Coroutine.</param>
	/// 
	void StartCoroutineWithRefandAddToDict (Transform _transform, IEnumerator coroutine) 
	{
		var ToDictCoroutine = StartCoroutine<string> (coroutine);
		Add (_transform, ToDictCoroutine);	
	}

	/// <summary>
	/// Add the specified TransformID and coroutine to the coroutine dictionary
	/// </summary>
	public void Add(Transform TransformID, Coroutine<string> coroutine)
	{
		//Debug.Log ("Added: " + coroutine + " LockID: " + lockID);
		
		if (!CoroutineList.ContainsKey(TransformID))
		{
			CoroutineList.Add(TransformID, new List<Coroutine<string>>());
		}
		
		if (!CoroutineList[TransformID].Contains(coroutine))
		{
			CoroutineList[TransformID].Add(coroutine);
		}
	}
	
	/// <summary>
	/// Remove the specified coroutine and TransformID if empty from coroutine dictionary
	/// </summary>
	public bool Remove(Transform TransformID, Coroutine<string> coroutine)
	{
		bool ret = false;

		if (CoroutineList.ContainsKey(TransformID))
		{
			//Debug.Log ("REMOVED: " + coroutine + " LockID: " + lockID);
			if (CoroutineList[TransformID].Contains(coroutine))
			{
				ret = CoroutineList[TransformID].Remove(coroutine);
			}
			
			if (CoroutineList[TransformID].Count == 0)
			{
				ret = CoroutineList.Remove(TransformID);
			}
		}

		return ret;

	}

	public int CountAllPooledAudio()
	{
		int count = 0;
		foreach (var list in instance.CoroutineList.Values)
			count += list.Count;
		return count;
	}

//	void OnGUI ()
//	{
//		GUI.Label( new Rect(10, 10, 200, 20), ("m_activeAudio.Count " + m_activeAudio.Count.ToString()) ); 
//		GUI.Label( new Rect(10, 35, 200, 20), ("m_activeLoop.Count " + m_activeLoop.Count.ToString()) ); 
//		GUI.Label( new Rect(10, 55, 200, 20), ("m_activeScore.Count " + m_activeScore.Count.ToString()) ); 
//		GUI.Label( new Rect(10, 95, 200, 20), ("ScoreVol " + ScoreVol )); 
//	}

	private AudioSource CreateGOAndSource (AudioClip clip, Transform emitter, string mixerGroup)
	{
		//Create an empty game object
		GameObject movingSoundLoc = new GameObject ("Audio: " + clip.name);
		movingSoundLoc.transform.position = emitter.position;
		movingSoundLoc.transform.parent = emitter;
		
		//Create the source
		AudioSource source = movingSoundLoc.AddComponent<AudioSource> ();
		
		source.outputAudioMixerGroup = mx.FindMatchingGroups ("Master/" + mixerGroup.ToString ()) [0]; //GetMixerGroup(mixerGroup);
		return source;
	}

	public AudioSource Play(AudioSource source, AudioClip clip, Transform emitter, float volume, string mixerGroup) {
								
		setSourceCharacter(source, clip, volume);
		source.spatialBlend = 0.45f;
				
		source.Play();					
		return source;
	}
	
	public AudioSource PlayLoop (AudioClip clip, Transform emitter, float volume, string mixerGroup)
	{

			var source = CreateGOAndSource (clip, emitter,mixerGroup);
			setSource(source, clip, volume);
			source.loop = false;
			source.spatialBlend = 0.22f;	
			
			source.Play ();
			//Set the source as active
			m_activeLoop.Add (new ClipInfo{source = source, defaultVolume = volume, startedPlaying = Time.time, clip = clip, realGObject = emitter.gameObject});
			return source;
	}


	public IEnumerator stopSound(AudioSource toStop, float timeToFadeOut = 3.0f) {
	
			// fade-out of the AudioClip
			if (fadeTime > 0f)  
			{
				while ((toStop.isPlaying) && (toStop.time + fadeTime > toStop.clip.length)) // ha túlnyúlna a fade-out a clip hosszúságán, akkor hagyd normálisan kifadelődni
					yield return null;
				// egyébként pedig indítsd el a forced fade-out-ot
				yield return StartCoroutine(toStop.FadeOut(fadeTime, 
				                                                 () =>
				                                                 {
//					Debug.Log( "started forced FadeOut" );
				},() =>
				{
//					Debug.Log( "finished forced FadeOut" );
				}));
			}
			else {
				while (toStop.isPlaying)
					yield return null;
			}
	}

	private AudioSource m_activeMusic;

	public AudioSource PlayMusic(AudioClip music, float volume, string mixerGroup) 
		{
		m_activeMusic = PlayLoop(music, transform, volume, mixerGroup);
					
		return m_activeMusic;
		}

	private void updateActiveAudio() { 
		var toRemove = new List<ClipInfo>();
		try {
			if (!m_activeVoiceOver) {
				//m_volumeMod = 1.0f;
				m_VOfade = false;
			}
			foreach (var audioClip in m_activeAudio) {
				if (!audioClip.source) {
					toRemove.Add(audioClip);
				} 
				else if (audioClip.source != m_activeVoiceOver) {
				audioClip.source.volume = audioClip.defaultVolume * m_volumeMod; //* m_devideVol
				}
			}
		} 
		catch {
			Debug.Log("Error updating active audio clips");
			return;
		}
		//cleanup
		foreach (var audioClip in toRemove) {
			m_activeAudio.Remove(audioClip);
			Debug.Log ("REMOVED CLIP");
		}
	}
}


