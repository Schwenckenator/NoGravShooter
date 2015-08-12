using UnityEngine;
using System.Collections;

public class AudioMusicVolume : MonoBehaviour {
    private static AudioSource source;
	// Use this for initialization
    void Awake() {
        source = GetComponent<AudioSource>();
    }

    void Start() {
        UpdateMusicVolume();
        source.Play();
    }

    public static void UpdateMusicVolume() {
        source.volume = SettingsManager.instance.VolumeMusic;
    }
}
