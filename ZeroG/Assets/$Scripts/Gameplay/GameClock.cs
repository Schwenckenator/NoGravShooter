using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameClock : MonoBehaviour {

    public Text clockText;

    static Canvas myCanvas;
    static int minsLeft;
    static int secsLeft;
    static bool counting;

    static GameClock singleton;

    void Awake() {
        singleton = this;
        myCanvas = GetComponent<Canvas>();
        ShowClock(false);
    }
    
    public static void SetEndTime(int seconds) {
        Debug.Log("Set End Time: " + seconds.ToString());
        ShowClock(true);
        singleton.StopAllCoroutines();
        NetworkInfoWrapper.singleton.SecondsLeft = seconds;
        minsLeft = Mathf.FloorToInt((seconds) / 60);
        secsLeft = Mathf.FloorToInt((seconds) - (minsLeft * 60));
        
        counting = true;
        singleton.StartCoroutine(singleton.DecrementTimer());
    }

    IEnumerator DecrementTimer() {
        while (counting) {
            //Debug.Log("Tick");
            UpdateText();
            yield return new WaitForSeconds(1f);
            NetworkInfoWrapper.singleton.SecondsLeft--;
            secsLeft--;
            if (secsLeft >= 0) continue; // Seconds are fine
            
            // Decrement minutes
            secsLeft += 60;
            minsLeft--;

            if (minsLeft >= 0) continue;

            // Time out!
            counting = false;
            secsLeft = 0;
            minsLeft = 0;
            
        }
    }

    void UpdateText() {
        string time = string.Format("{0} : {1}", minsLeft.ToString("00"), secsLeft.ToString("00"));
        clockText.text = time;
    }

    public static void ShowClock(bool show) {
        myCanvas.enabled = show/* && GameManager.singleton.IsUseTimer*/;
    }

    public static bool TimeUp() {
        return NetworkInfoWrapper.singleton.SecondsLeft <= 0;
    }

    public static void ClientUpdateText() {
        ShowClock(true);
        singleton.UpdateText();
    }
}
