using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameClock : MonoBehaviour {

    public Text clockText;

    public bool debug = false;
    private static Logger log;

    static Canvas myCanvas;
    static int minsLeft;
    static int secsLeft;
    static bool counting;

    static GameClock singleton;

    void Awake() {
        singleton = this;
        log = new Logger(debug);
        myCanvas = GetComponent<Canvas>();
        ShowClock(false);
    }
    
    public static void SetEndTime(int seconds) {
        log.Log("Set End Time: " + seconds.ToString());

        NetworkInfoWrapper.singleton.SecondsLeft = seconds;

        ClockPrep();
    }

    public static void ClockPrep() {
        ShowClock(true);
        singleton.StopAllCoroutines();
        counting = true;
        singleton.StartCoroutine(singleton.DecrementTimer());
    }

    IEnumerator DecrementTimer() {
        while (counting) {
            //Debug.Log("Tick");
            UpdateText();
            yield return new WaitForSeconds(1f);
            NetworkInfoWrapper.singleton.SecondsLeft--;
            int seconds = NetworkInfoWrapper.singleton.SecondsLeft;

            minsLeft = Mathf.FloorToInt((seconds) / 60);
            secsLeft = Mathf.FloorToInt((seconds) % 60);

            if (seconds > 0) continue;

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
