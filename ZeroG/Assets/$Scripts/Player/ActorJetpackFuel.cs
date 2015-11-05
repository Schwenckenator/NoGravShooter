using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ActorJetpackFuel : NetworkBehaviour {

    public Texture[] empRadar;
    public Texture[] empStats;
    public Texture[] empCursor;

    public AudioClip soundJetpackRecharge;
    public float volumeJetpackRecharge;
    public AudioClip soundJetpackEmpty;
    public float volumeJetpackEmpty;
    public AudioClip soundJetpackShutoff;
    public float volumeJetpackShutoff;

    public int maxFuel = 150;
    private static int defaultMaxFuel = 150;
    public float fuelRecharge = 50; // per second
    public float maxRechargeWaitTime = 1.0f;

    public AudioSource jetpackAudio;

    private float fuel;
    private float rechargeWaitTime;

    private bool isRecharging = false;
    private bool isJetpackDisabled = false;
    private bool glitched = false;

    // Use this for initialization
    void Awake () {
        fuel = maxFuel;
        rechargeWaitTime = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer) return;

        if (isRecharging) {
            RechargeFuel(fuelRecharge);
        }
    }
    public float GetMaxFuel() {
        return maxFuel;
    }
    public float GetFuel() {
        return fuel;
    }
    /// <summary>
    /// Checks itself to see if there is fuel available
    /// Returns false if fuel empty
    /// </summary>
    /// <param name="spentFuel"></param>
    /// <param name="forceSpend"></param>
    /// <returns></returns>
    public bool SpendFuel(float spentFuel, bool forceSpend = false) {
        if (DebugManager.IsAllFuel()) {
            return true;
        }

        isRecharging = true;
        if (isJetpackDisabled && !forceSpend) {
            return false;
        }
        fuel -= spentFuel;
        if (fuel < 0) {
            fuel = 0;
            DisableJetpack(true);
            rechargeWaitTime = maxRechargeWaitTime * 2;
            StartCoroutine(PlayJetpackEmptySound());
            return false;
        }
        rechargeWaitTime = maxRechargeWaitTime;
        return true;
    }

    private void RechargeFuel(float charge) {
        if (rechargeWaitTime > 0) {
            rechargeWaitTime -= Time.deltaTime;
        } else {
            glitched = false;
            fuel += charge * Time.deltaTime;
            if (fuel > maxFuel) {
                fuel = maxFuel;
                isRecharging = false;
                DisableJetpack(false);
                StartCoroutine("StopRechargeSound");
            } else {
                PlayRechargeSound();
            }
        }
    }

    void PlayRechargeSound() {
        if (!jetpackAudio.isPlaying) {
            jetpackAudio.volume = volumeJetpackRecharge;
            jetpackAudio.clip = soundJetpackRecharge;
            jetpackAudio.Play();
        }
    }
    IEnumerator PlayJetpackEmptySound() {
        jetpackAudio.volume = volumeJetpackEmpty;
        jetpackAudio.clip = soundJetpackEmpty;
        jetpackAudio.Play();
        yield return new WaitForSeconds(1.25f);
        jetpackAudio.Stop();
        jetpackAudio.volume = volumeJetpackShutoff;
        jetpackAudio.PlayOneShot(soundJetpackShutoff);
    }

    IEnumerator StopRechargeSound() {

        while (jetpackAudio.volume > 0.05f) {
            if (jetpackAudio.clip == soundJetpackRecharge) {
                jetpackAudio.volume /= 1 + (3 * Time.deltaTime);
            } else {
                break;
            }

            yield return null;
        }
    }
    public bool IsJetpackDisabled() {
        return isJetpackDisabled;
    }
    public void EMPglitch() {
        glitched = true;
    }
    void OnGUI() {
        if (!glitched) return;
        //if (!GameManager.IsPlayerMenu() && //NetworkView.isMine) {
        //    GUI.depth = 0;
        //    GUI.DrawTexture(new Rect(20 + Random.Range(-10, 11), Screen.height - 240 + Random.Range(-10, 11), 220, 220), empRadar[Random.Range(0, 5)]);
        //    GUI.DrawTexture(new Rect(Screen.width / 2 - 55 / 2, Screen.height / 2 - 45 / 2, 55, 45), empCursor[Random.Range(0, 4)]);
        //    GUI.DrawTexture(new Rect(Screen.width - 330, Screen.height - 285 + Random.Range(-5, 6), 330, 285), empStats[Random.Range(0, 4)]);
        //}
    }

    void DisableJetpack(bool disable) {
        isJetpackDisabled = disable;
        UIPlayerHUD.JetpackDisabled(disable);
    }

    public static int GetDefaultMaxFuel() {
        return defaultMaxFuel;
    }
}
