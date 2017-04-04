using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixAdd : MonoBehaviour {


	public AudioMixerGroup[] gr;
	public AudioMixer mx;



	// Use this for initialization
	void Start () {

		gr = mx.FindMatchingGroups("Master");
		this.gameObject.GetComponent<AudioSource> ().outputAudioMixerGroup = gr [0];
		Debug.Log (gr[0].name);


	//	this.gameObject.GetComponent<AudioSource> ().outputAudioMixerGroup = gr.outputAudioMixerGroup;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
