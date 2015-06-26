﻿using UnityEngine;
using System.Collections;

public class GameClock : MonoBehaviour {

    // For display
    static ChangeableText clockText;
    static Canvas myCanvas;
    static int minsLeft;
    static int secsLeft;
    static float endTime;
    static bool enabled;

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
        
        enabled = true;
        _instance.StartCoroutine(DecrementTimer());
    }

    static IEnumerator DecrementTimer() {
        while (enabled) {
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
            enabled = false;
            secsLeft = 0;
            minsLeft = 0;
            
        }
    }

    static void UpdateText() {
        string time = string.Format("{0} : {1}", minsLeft.ToString("00"), secsLeft.ToString("00"));
        clockText.SetText(time);
    }

    void OnLevelWasLoaded(int level) {
        bool hideClock = GameManager.IsSceneMenu() || GameManager.IsSceneTutorial();
        myCanvas.enabled = !hideClock;

        if (hideClock) {
            StopAllCoroutines();
        }
    }

    public static void ShowClock(bool show) {
        myCanvas.enabled = show;
    }
}
