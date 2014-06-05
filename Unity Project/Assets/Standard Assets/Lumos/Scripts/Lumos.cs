// Copyright (c) 2012 Rebel Hippo Inc. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main class for Lumos functionality.
/// </summary>
public partial class Lumos : MonoBehaviour
{
	/// <summary>
	/// The services that Lumos provides.
	/// </summary>
	public enum service { SystemInfo, Logs, Events, Feedback, Realtime };

	/// <summary>
	/// The version.
	/// </summary>
	public const string version = "0.9.1";

	/// <summary>
	/// An instance of this class.
	/// </summary>
	public static Lumos instance { get; private set; }

	/// <summary>
	/// Displays detailed information about WWW requests / responses when true.
	/// </summary>
	public static bool debug { get; set; }

	/// <summary>
	/// A unique string that identifies the application.
	/// </summary>
	public static string appId { get; private set; }

	static string _playerId;
	/// <summary>
	/// A unique string that identifies the player.
	/// </summary>
	public static string playerId {
		get {
			if (_playerId == null) {
				_playerId = PlayerPrefs.GetString(LumosUtil.playerIdPrefsKey);
			}

			return _playerId;
		}
		private set {
			_playerId = value;
			PlayerPrefs.SetString(LumosUtil.playerIdPrefsKey, value);
		}
	}

	/// <summary>
	/// Whether a player ID has been previously saved.
	/// </summary>
	public static bool hasPlayer {
		get { return PlayerPrefs.HasKey(LumosUtil.playerIdPrefsKey); }
	}

	static uint _timerInterval = 30;
	/// <summary>
	/// The interval (in seconds) at which queued data is sent to the server.
	/// </summary>
	public static uint timerInterval {
		get { return _timerInterval; }
		set { _timerInterval = value; }
	}

	/// <summary>
	/// Whether the data sending timer is paused.
	/// </summary>
	static bool timerPaused;

	static List<service> _servicesAvailable = new List<service>();
	/// <summary>
	/// Which services are operational and available to use.
	/// </summary>
	public static List<service> servicesAvailable {
		get { return _servicesAvailable; }
		set { _servicesAvailable = value; }
	}

	#region Inspector Settings

	public string secretKey;
	public bool runInEditor;
	public bool recordPresetEvents;
	public bool recordErrors;
	public bool recordWarnings;
	public bool recordLogs;

	#endregion

	/// <summary>
	/// Initializes a new instance of this class.
	/// </summary>
	Lumos () {}

	/// <summary>
	/// Sets up Lumos.
	/// </summary>
	void Awake ()
	{
		// Prevent multiple instances of Lumos from existing.
		// Necessary because DontDestroyOnLoad keeps the object between scenes.
		if (instance != null) {
			Lumos.Log("Destroying duplicate game object instance.");
			Destroy(gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(this);
		appId = secretKey.Split('-')[0];

		if (appId == null || appId == "") {
			Lumos.Remove("Secret key not set.");
			return;
		}

		if (recordPresetEvents) {
			gameObject.AddComponent<LumosEvents>();
		}

		// Set up debug log redirect.
		Application.RegisterLogCallback(LumosLogs.Record);
	}

	/// <summary>
	/// Extra setup that needs to occur after Awake.
	/// </summary>
	void Start ()
	{
		if (hasPlayer) {
			Lumos.Log("Using existing player " + playerId);
			LumosApp.Ping();
		} else {
			playerId = LumosUtil.GeneratePlayerId();
			Lumos.Log("Using new player " + playerId);
			LumosApp.Ping(true);
		}

		RunRoutine(SendQueuedRoutine());
	}

	void OnGUI ()
	{
		LumosFeedback.OnGUI();
	}

	void OnLevelWasLoaded ()
	{
		SendQueued();
	}

	/// <summary>
	/// Sends queued data.
	/// Currently the only data that accumulates is debug logs.
	/// </summary>
	public void SendQueued ()
	{
		LumosLogs.Send();
		LumosEvents.Send();
	}

	/// <summary>
	/// Sends queued data on an interval.
	/// </summary>
	IEnumerator SendQueuedRoutine ()
	{
		yield return new WaitForSeconds((float)timerInterval);

		if (!timerPaused) {
			SendQueued();
		}

		RunRoutine(SendQueuedRoutine());
	}

	/// <summary>
	/// Pauses the queued data send timer.
	/// </summary>
	public static void PauseTimer ()
	{
		timerPaused = true;
	}

	/// <summary>
	/// Resumes the queued data send timer.
	/// </summary>
	public static void ResumeTimer ()
	{
		timerPaused = false;
	}

	/// <summary>
	/// Executes a coroutine.
	/// </summary>
	/// <param name="routine">The coroutine to execute.</param>
	public static Coroutine RunRoutine (IEnumerator routine)
	{
		if (instance == null) {
			Lumos.LogError("Lumos game object must be instantiated " +
			               "before its methods can be called.");
			return null;
		}

		return instance.StartCoroutine(routine);
	}

	/// <summary>
	/// Destroys the instance so that it cannot be used.
	/// This will be called at the start of the game if it's determined that
	/// information cannot be sent to the server properly.
	/// </summary>
	/// <param name="reason">The reason why the instance is unusable.</param>
	public static void Remove (string reason)
	{
		if (instance != null) {
			Debug.LogWarning("[Lumos] " + reason +
			                 " No information will be recorded.");
			Destroy(instance.gameObject);
		}
	}
}

