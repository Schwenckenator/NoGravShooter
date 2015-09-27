using UnityEngine;
using System.Collections;

public class GameClock : MonoBehaviour {

    // For display
    static ChangeableText clockText;
    static Canvas myCanvas;
    static int minsLeft;
    static int secsLeft;
    static float endTime;
    static bool counting;

    static GameClock _instance;

    void Awake() {
        DontDestroyOnLoad(this);
        _instance = this;
        clockText = GetComponentInChildren<ChangeableText>();
        myCanvas = GetComponent<Canvas>();
    }
    void Start() {
        ShowClock(false);
    }
    
    public static void SetEndTime(float endTime) {
        _instance.gameObject.SetActive(true);
        _instance.StopAllCoroutines();
        GameClock.endTime = endTime;
        minsLeft = Mathf.FloorToInt((endTime - Time.time) / 60);
        secsLeft = Mathf.FloorToInt((endTime - Time.time) - (minsLeft * 60));
        
        counting = true;
        _instance.StartCoroutine(DecrementTimer());
    }

    static IEnumerator DecrementTimer() {
        while (counting) {
            //Debug.Log("Tick");
            UpdateText();
            yield return new WaitForSeconds(1f);
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

    static void UpdateText() {
        string time = string.Format("{0} : {1}", minsLeft.ToString("00"), secsLeft.ToString("00"));
        clockText.SetText(time);
    }

    void OnLevelWasLoaded() {
        Debug.Log("On level was loaded call in GameClock.");
        bool showClock = GameManager.instance.IsUseTimer && !(GameManager.IsSceneMenu() || GameManager.IsSceneTutorial());
        myCanvas.enabled = showClock;

        if (!showClock) {
            StopAllCoroutines();
        }
    }

    public static void ShowClock(bool show) {
        myCanvas.enabled = show && GameManager.instance.IsUseTimer;
    }
}
