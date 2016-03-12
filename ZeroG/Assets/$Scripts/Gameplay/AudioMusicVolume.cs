using UnityEngine;
using System.Collections;

public class AudioMusicVolume : MonoBehaviour {
    private static AudioSource source;
	public AudioClip[] soundtrack;
	private int currenttrack;
	private int prevtrack;
	// Use this for initialization
    void Awake() {
        source = GetComponent<AudioSource>();
    }

    void Start() {
        UpdateMusicVolume();
		if (soundtrack.Length > 1){
			source.clip = soundtrack[Random.Range(0, soundtrack.Length)];
		} else {
			source.clip = soundtrack[0];
		}
		source.Play();
    }
	
	void Update (){
		if (!source.isPlaying){
			if (soundtrack.Length > 1){
				do{
					currenttrack = Random.Range(0, soundtrack.Length);
					source.clip = soundtrack[currenttrack];
					source.Play();
				}while(currenttrack == prevtrack);
			} else {
				source.Play();
			}
		}
	}

    public static void UpdateMusicVolume() {
		source.volume = SettingsManager.instance.VolumeMusic;
    }
}
