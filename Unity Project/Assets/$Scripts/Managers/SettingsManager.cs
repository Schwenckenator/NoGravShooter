using UnityEngine;
using System.Collections;

public enum KeyBind { MoveForward, MoveBack, MoveLeft, MoveRight, RollLeft, RollRight, JetUp, JetDown, Reload, Grenade, Interact, GrenadeSwitch, StopMovement };

public class SettingsManager : MonoBehaviour {

    #region Instance
    //Here is a private reference only this class can access
    private static SettingsManager _instance;
    //This is the public reference that other classes will use
    public static SettingsManager instance {
        get {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<SettingsManager>();
            }
            return _instance;
        }
    }
    #endregion

    public static KeyCode[] keyBindings;
    const int SecondsInMinute = 60;
    

    private string[] levelList = { "FirstLevel", "DerilictShipScene", "SpaceStationScene" };
    private string[] gameModeList = { "DeathMatch", "Team DeathMatch", "Capture the Flag", "Extraction", "Skirmish", "Team Skirmish", "Elimination", "Infection" };

    public string[] LevelList { get { return levelList; } }
    public string[] GameModeList { get { return gameModeList; } }

    #region Networking Settings
    private string myServerName;
    public string MyServerName {
        get {
            return myServerName;
        }
        set {
            myServerName = value;
            DisplayServerName = value;
        }
    }
    public string DisplayServerName { get; set; }
    public string PlayerName { get; set; }
    public string PortNumStr { get; set; }
    public int PortNum { get; set; }
    public string IpAddress { get; set; }
    public string PasswordServer { get; set; }
    public string PasswordClient { get; set; }
    #endregion

    #region PlayerControlSettings
    public float MouseSensitivityX { get; set; }
    public float MouseSensitivityY { get; set; }
    public int MouseYDirection { get; set; }
    public float FieldOfView { get; set; }
    private int i_AutoPickup { get; set; } // Deal with the hungarian
    public bool AutoPickup { get; set; }
    #endregion

    #region GameSettings
    private int levelIndex;
    public int LevelIndex {
        get {
            return levelIndex;
        }
        set {
            levelIndex = value;
            LevelName = LevelList[value];
        }
    }
    public string LevelName { get; set; }

    private int gameModeIndex;
    public int GameModeIndex {
        get {
            return gameModeIndex;
        }
        set {
            gameModeIndex = value;
            GameModeIndexDisplay = value;
        }
    }
    private int gameModeIndexDisplay;
    public int GameModeIndexDisplay {
        get { return gameModeIndexDisplay; }
        set {
            gameModeIndexDisplay = value;
            GameModeName = GameModeList[value];
        }
    }
    public string GameModeName { get; set; }

    public int SpawnWeapon1 { get; set; }
    public int SpawnWeapon2 { get; set; }

    private int _timeLimitMin;
    public int TimeLimitMin {
        get {
            return _timeLimitMin;
        }
        set {
            value = Mathf.Max(value, 0);
            _timeLimitMin = value;
            TimeLimitSec = value * SecondsInMinute;
        }
    }
    public int TimeLimitSec { get; protected set;}

    private int _scoreToWin;
    public int ScoreToWin {
        get {
            return _scoreToWin;
        }
        set {
            value = Mathf.Max(value, 0);
            _scoreToWin = value;
        }
    }

    // Bonus spawning
    public int MedkitCanSpawn { get; set; }
    public int GrenadeCanSpawn { get; set; }
    public int WeaponCanSpawn { get; set; } 
	
	//Colour setting
    public float ColourR { get; set; }
    public float ColourG { get; set; }
    public float ColourB { get; set; }
    #endregion

    #region PlayerPrefsKeys

    // Default values
    private string defaultPlayerName = "Player";
    private string defaultIpAddress = "127.0.0.1";
    private string defaultPortNumStr = "25000";

    private float defaultMouseSensitivity   = 0.5f;
    private int defaultMouseYDirection      = -1;
    private float defaultFieldOfView        = 70;

    private int laserRifleIndex = 0;
    private int noWeaponIndex = 7;

    private int i_False = 0;
    private int i_True = 1;

    #endregion

    void Awake() {
        RetrieveKeyBinds();
        RetrieveSettings();
    }

    void RetrieveKeyBinds() {
        keyBindings = new KeyCode[System.Enum.GetNames(typeof(KeyBind)).Length];

        keyBindings[(int)KeyBind.MoveForward]   = (KeyCode)PlayerPrefs.GetInt("bindMoveForward",    (int)KeyCode.W);
        keyBindings[(int)KeyBind.MoveBack]      = (KeyCode)PlayerPrefs.GetInt("bindMoveBack",       (int)KeyCode.S);
        keyBindings[(int)KeyBind.MoveLeft]      = (KeyCode)PlayerPrefs.GetInt("bindMoveLeft",       (int)KeyCode.A);
        keyBindings[(int)KeyBind.MoveRight]     = (KeyCode)PlayerPrefs.GetInt("bindMoveRight",      (int)KeyCode.D);
        keyBindings[(int)KeyBind.StopMovement]  = (KeyCode)PlayerPrefs.GetInt("bindStopMovement",   (int)KeyCode.X);

        keyBindings[(int)KeyBind.RollLeft]      = (KeyCode)PlayerPrefs.GetInt("bindRollLeft",       (int)KeyCode.Q);
        keyBindings[(int)KeyBind.RollRight]     = (KeyCode)PlayerPrefs.GetInt("bindRollRight",      (int)KeyCode.E);
        keyBindings[(int)KeyBind.JetUp]         = (KeyCode)PlayerPrefs.GetInt("bindJetUp",          (int)KeyCode.Space);
        keyBindings[(int)KeyBind.JetDown]       = (KeyCode)PlayerPrefs.GetInt("bindJetDown",        (int)KeyCode.LeftShift);

        keyBindings[(int)KeyBind.Reload]        = (KeyCode)PlayerPrefs.GetInt("bindReload",         (int)KeyCode.R);
        keyBindings[(int)KeyBind.Grenade]       = (KeyCode)PlayerPrefs.GetInt("bindGrenade",        (int)KeyCode.G);
        keyBindings[(int)KeyBind.Interact]      = (KeyCode)PlayerPrefs.GetInt("bindInteract",       (int)KeyCode.F);
        keyBindings[(int)KeyBind.GrenadeSwitch] = (KeyCode)PlayerPrefs.GetInt("bindGrenadeSwitch",  (int)KeyCode.H);
        
    }

    public void SaveKeyBinds() {
        PlayerPrefs.SetInt("bindMoveForward",   (int)SettingsManager.keyBindings[(int)KeyBind.MoveForward]);
        PlayerPrefs.SetInt("bindMoveBack",      (int)SettingsManager.keyBindings[(int)KeyBind.MoveBack]);
        PlayerPrefs.SetInt("bindMoveLeft",      (int)SettingsManager.keyBindings[(int)KeyBind.MoveLeft]);
        PlayerPrefs.SetInt("bindMoveRight",     (int)SettingsManager.keyBindings[(int)KeyBind.MoveRight]);
        PlayerPrefs.SetInt("bindStopMovement",  (int)SettingsManager.keyBindings[(int)KeyBind.StopMovement]);

        PlayerPrefs.SetInt("bindRollLeft",      (int)SettingsManager.keyBindings[(int)KeyBind.RollLeft]);
        PlayerPrefs.SetInt("bindRollRight",     (int)SettingsManager.keyBindings[(int)KeyBind.RollRight]);
        PlayerPrefs.SetInt("bindJetUp",         (int)SettingsManager.keyBindings[(int)KeyBind.JetUp]);
        PlayerPrefs.SetInt("bindJetDown",       (int)SettingsManager.keyBindings[(int)KeyBind.JetDown]);

        PlayerPrefs.SetInt("bindReload",        (int)SettingsManager.keyBindings[(int)KeyBind.Reload]);
        PlayerPrefs.SetInt("bindGrenade",       (int)SettingsManager.keyBindings[(int)KeyBind.Grenade]);
        PlayerPrefs.SetInt("bindInteract",      (int)SettingsManager.keyBindings[(int)KeyBind.Interact]);
        PlayerPrefs.SetInt("bindGrenadeSwitch", (int)SettingsManager.keyBindings[(int)KeyBind.GrenadeSwitch]);
    }

    /// <summary>
    /// Retrieves settings from PlayerPrefs and stores in properties
    /// </summary>
    void RetrieveSettings() {
        // Network Settings
        MyServerName = PlayerPrefs.GetString("ServerName");
        PlayerName = PlayerPrefs.GetString("PlayerName", defaultPlayerName);
        PortNumStr = PlayerPrefs.GetString("PortNumStr", defaultPortNumStr);
        IpAddress = PlayerPrefs.GetString("IpAddress", defaultIpAddress);
        PasswordServer = PlayerPrefs.GetString("Password", "");
        PasswordClient = "";

        //Control Settings
        MouseSensitivityX = PlayerPrefs.GetFloat("xMouseSensitivity", defaultMouseSensitivity);
        MouseSensitivityY = PlayerPrefs.GetFloat("yMouseSensitivity", defaultMouseSensitivity);
        MouseYDirection = PlayerPrefs.GetInt("MouseYDirection", defaultMouseYDirection);
        FieldOfView = PlayerPrefs.GetFloat("FieldOfView", defaultFieldOfView);
        i_AutoPickup = PlayerPrefs.GetInt("AutoPickup", i_False);

        //Game Settings
        LevelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
        GameModeIndex = PlayerPrefs.GetInt("GameModeIndex", 0);
        SpawnWeapon1 = PlayerPrefs.GetInt("SpawnWeapon1", laserRifleIndex);
        SpawnWeapon2 = PlayerPrefs.GetInt("SpawnWeapon2", noWeaponIndex);
        MedkitCanSpawn = PlayerPrefs.GetInt("MedkitCanSpawn", i_True);
        GrenadeCanSpawn = PlayerPrefs.GetInt("GrenadeCanSpawn", i_True);
        WeaponCanSpawn = PlayerPrefs.GetInt("WeaponCanSpawn", i_True);
		
		//Colour setting
		ColourR = PlayerPrefs.GetFloat("ColourR", 0.6f);
		ColourG = PlayerPrefs.GetFloat("ColourG", 0.6f);
		ColourB = PlayerPrefs.GetFloat("ColourB", 0.6f);

        ScoreToWin = PlayerPrefs.GetInt("ScoreToWin", 20);
        TimeLimitMin = PlayerPrefs.GetInt("TimeLimitMin", 15);

        ConvertSettingsIntToBool();
    }
    
    /// <summary>
    /// Saves current settings to PlayerPrefs
    /// </summary>
    public void SaveSettings() {
        
        ConvertSettingsBoolToInt();

        // Network Settings
        PlayerPrefs.SetString("ServerName", MyServerName);
        PlayerPrefs.SetString("PlayerName", PlayerName);
        PlayerPrefs.SetString("PortNumStr", PortNumStr);
        PlayerPrefs.SetString("IpAddress", IpAddress);
        PlayerPrefs.SetString("Password", PasswordServer);

        // Control Settings
        PlayerPrefs.SetFloat("xMouseSensitivity", MouseSensitivityX);
        PlayerPrefs.SetFloat("yMouseSensitivity", MouseSensitivityY);
        PlayerPrefs.SetInt("MouseYDirection", MouseYDirection);
        PlayerPrefs.SetFloat("FieldOfView", FieldOfView);
        PlayerPrefs.SetInt("AutoPickup", i_AutoPickup);
        
        //Game Settings
        PlayerPrefs.SetInt("LevelIndex", LevelIndex); 
        PlayerPrefs.SetInt("GameModeIndex", GameModeIndex);
        PlayerPrefs.SetInt("SpawnWeapon1", SpawnWeapon1);
        PlayerPrefs.SetInt("SpawnWeapon2", SpawnWeapon2);
        PlayerPrefs.SetInt("MedkitCanSpawn", MedkitCanSpawn);
        PlayerPrefs.SetInt("GrenadeCanSpawn", GrenadeCanSpawn);
        PlayerPrefs.SetInt("WeaponCanSpawn", WeaponCanSpawn);
		
		//Colour setting
		PlayerPrefs.SetFloat("ColourR", ColourR);
		PlayerPrefs.SetFloat("ColourG", ColourG);
		PlayerPrefs.SetFloat("ColourB", ColourB);

        PlayerPrefs.SetInt("ScoreToWin", ScoreToWin);
        PlayerPrefs.SetInt("TimeLimitMin", TimeLimitMin);
    }

    /// <summary>
    ///  Returns -1 on error
    /// </summary>
    public void ParsePortNumber() {
        try {
            PortNum = int.Parse(PortNumStr);
        } catch {
            PortNum = -1;
        }
    }

    public bool[] GetAllowedBonuses() {
        // Needs order: Medkit, Grenade, weapon
        bool[] temp = {(MedkitCanSpawn == 1),(GrenadeCanSpawn == 1),(WeaponCanSpawn == 1)} ;
        return temp;
    }

    private void ConvertSettingsIntToBool() {
        AutoPickup = (i_AutoPickup == 1);
    }
    private void ConvertSettingsBoolToInt() {
        i_AutoPickup = AutoPickup ? 1 : 0;
    }

    public void ClearPasswordClient() {
        PasswordClient = "";
    }

    public Color GetPlayerColour() {
        return new Color(ColourR, ColourG, ColourB, 1);
    }

    public bool IsTeamGameMode() {
        // Team : Team Deathmatch, Capture the Flag, Extraction, Team Skirmish
        // No Team: Deathmatch, Skirmish, Elimination, Infection <- There are teams but no choice.
        return (GameModeIndexDisplay == 1 || GameModeIndexDisplay == 2 || GameModeIndexDisplay == 3 || GameModeIndexDisplay == 5);
    }
}
