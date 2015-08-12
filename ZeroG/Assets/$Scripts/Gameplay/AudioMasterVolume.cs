using UnityEngine;
using System.Collections;

public class AudioMasterVolume : MonoBehaviour {

    void Start() {
        UpdateMasterVolume();
    }

    public static void UpdateMasterVolume() {
        AudioListener.volume = SettingsManager.instance.VolumeMaster;
    }
}
