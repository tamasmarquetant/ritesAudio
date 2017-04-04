using UnityEngine;
using System.Collections;

// Singleton for setting the global audio volumes
// by BitsAlive
public class GlobalVolumeManager : MonoBehaviour {
	
	private static GlobalVolumeManager instance = null;
	public static GlobalVolumeManager Instance {
		get	{
			if (instance == null) {
				GameObject go = GameObject.Find("Global Volume Manager");
				if (go != null)
					instance = go.GetComponent<GlobalVolumeManager>();
				if (instance == null)
					instance = new GameObject("Global Volume Manager").AddComponent<GlobalVolumeManager>();
			}
			return instance;
		}
	}

	public float soundVolume = 0.5f;
	public float ambientVolume = 0.5f;
	public float musicVolume = 0.5f;
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
		}
}
