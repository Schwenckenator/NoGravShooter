using UnityEngine;
using System.Collections;

public class SettingsManager : MonoBehaviour {
    public enum KeyBind { MoveForward, MoveBack, MoveLeft, MoveRight, RollLeft, RollRight, JetUp, JetDown, Reload, Grenade, Interact, GrenadeSwitch };
    public static KeyCode[] keyBindings;
    const int SecondsInMinute = 60;
    

    private string[] levelList = { "FirstLevel", "DerilictShipScene", "SpaceStationScene" };
    private string[] gameModeList = { "DeathMatch", "Team DeathMatch", "Capture the Flag", "Extraction", "Skirmish", "Team Skirmish", "Elimination", "Infection" };

    public string[] LevelList { get { return levelList; } }
    public string[] GameModeList { get { return gameModeList; } }

    #region Networking Settings
    public string ServerName { get; set; }
    public string PlayerName { get; set; }
    public string PortNumStr { get; set; }
    public int PortNum { get; set; }
    public string IpAddress { get; set; }
    public string Password { get; set; }
    #endregion

    #region PlayerControlSettings
    public float xMouseSensitivity { get; set; }
    public float yMouseSensitivity { get; set; }
    public int MouseYDirection { get; set; }
    public float FieldOfView { get; set; }
    public int AutoPickup { get; set; } 
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
            GameModeName = GameModeList[value];
        }
    }
    public string GameModeName { get; set; }
    public int SpawnWeapon1 { get; set; }
    public int SpawnWeapon2 { get; set; }

    private int timeLimitMin;
    public int TimeLimitMin {
        get {
            return timeLimitMin;
        }
        set {
            timeLimitMin = value;
            TimeLimitSec = value * SecondsInMinute;
        }
    }
    public int TimeLimitSec { get; protected set;}

    public int ScoreToWin { get; set; }

    // Bonus spawning
    public int MedkitCanSpawn { get; set; }
    public int GrenadeCanSpawn { get; set; }
    public int WeaponCanSpawn { get; set; } 
    #endregion


    void Awake() {
        GameManager.testMode = false;

        RetrieveKeyBinds();
        RetrieveSettings();
        
    }

    void RetrieveKeyBinds() {
        keyBindings = new KeyCode[System.Enum.GetNames(typeof(SettingsManager.KeyBind)).Length];

        keyBindings[(int)SettingsManager.KeyBind.MoveForward]   = (KeyCode)PlayerPrefs.GetInt("bindMoveForward",    (int)KeyCode.W);
        keyBindings[(int)SettingsManager.KeyBind.MoveBack]      = (KeyCode)PlayerPrefs.GetInt("bindMoveBack",       (int)KeyCode.S);
        keyBindings[(int)SettingsManager.KeyBind.MoveLeft]      = (KeyCode)PlayerPrefs.GetInt("bindMoveLeft",       (int)KeyCode.A);
        keyBindings[(int)SettingsManager.KeyBind.MoveRight]     = (KeyCode)PlayerPrefs.GetInt("bindMoveRight",      (int)KeyCode.D);

        keyBindings[(int)SettingsManager.KeyBind.RollLeft]      = (KeyCode)PlayerPrefs.GetInt("bindRollLeft",       (int)KeyCode.Q);
        keyBindings[(int)SettingsManager.KeyBind.RollRight]     = (KeyCode)PlayerPrefs.GetInt("bindRollRight",      (int)KeyCode.E);
        keyBindings[(int)SettingsManager.KeyBind.JetUp]         = (KeyCode)PlayerPrefs.GetInt("bindJetUp",          (int)KeyCode.Space);
        keyBindings[(int)SettingsManager.KeyBind.JetDown]       = (KeyCode)PlayerPrefs.GetInt("bindJetDown",        (int)KeyCode.LeftShift);

        keyBindings[(int)SettingsManager.KeyBind.Reload]        = (KeyCode)PlayerPrefs.GetInt("bindReload",         (int)KeyCode.R);
        keyBindings[(int)SettingsManager.KeyBind.Grenade]       = (KeyCode)PlayerPrefs.GetInt("bindGrenade",        (int)KeyCode.G);
        keyBindings[(int)SettingsManager.KeyBind.Interact]      = (KeyCode)PlayerPrefs.GetInt("bindInteract",       (int)KeyCode.F);
        keyBindings[(int)SettingsManager.KeyBind.GrenadeSwitch] = (KeyCode)PlayerPrefs.GetInt("bindGrenadeSwitch",  (int)KeyCode.H);
    }

    public void SaveKeyBinds() {
        PlayerPrefs.SetInt("bindMoveForward",   (int)SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveForward]);
        PlayerPrefs.SetInt("bindMoveBack",      (int)SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveBack]);
        PlayerPrefs.SetInt("bindMoveLeft",      (int)SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveLeft]);
        PlayerPrefs.SetInt("bindMoveRight",     (int)SettingsManager.keyBindings[(int)SettingsManager.KeyBind.MoveRight]);


        PlayerPrefs.SetInt("bindRollLeft",      (int)SettingsManager.keyBindings[(int)SettingsManager.KeyBind.RollLeft]);
        PlayerPrefs.SetInt("bindRollRight",     (int)SettingsManager.keyBindings[(int)SettingsManager.KeyBind.RollRight]);
        PlayerPrefs.SetInt("bindJetUp",         (int)SettingsManager.keyBindings[(int)SettingsManager.KeyBind.JetUp]);
        PlayerPrefs.SetInt("bindJetDown",       (int)SettingsManager.keyBindings[(int)SettingsManager.KeyBind.JetDown]);

        PlayerPrefs.SetInt("bindReload",        (int)SettingsManager.keyBindings[(int)SettingsManager.KeyBind.Reload]);
        PlayerPrefs.SetInt("bindGrenade",       (int)SettingsManager.keyBindings[(int)SettingsManager.KeyBind.Grenade]);
        PlayerPrefs.SetInt("bindInteract",      (int)SettingsManager.keyBindings[(int)SettingsManager.KeyBind.Interact]);
        PlayerPrefs.SetInt("bindGrenadeSwitch", (int)SettingsManager.keyBindings[(int)SettingsManager.KeyBind.GrenadeSwitch]);
    }

    /// <summary>
    /// Retrieves settings from PlayerPrefs and stores in properties
    /// </summary>
    void RetrieveSettings() {
        // Network Settings
        ServerName = PlayerPrefs.GetString("ServerName");
        PlayerName = PlayerPrefs.GetString("PlayerName", "Player");
        PortNumStr = PlayerPrefs.GetString("PortNumStr", "25000");
        IpAddress = PlayerPrefs.GetString("IpAddress", "127.0.0.1");
        Password = PlayerPrefs.GetString("Password", "");

        //Control Settings
        xMouseSensitivity = PlayerPrefs.GetFloat("xMouseSensitivity", 0.5f);
        yMouseSensitivity = PlayerPrefs.GetFloat("yMouseSensitivity", 0.5f);
        MouseYDirection = PlayerPrefs.GetInt("MouseYDirection", -1);
        FieldOfView = PlayerPrefs.GetFloat("FieldOfView", 70);
        AutoPickup = PlayerPrefs.GetInt("AutoPickup", 0);

        //Game Settings
        LevelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
        GameModeIndex = PlayerPrefs.GetInt("GameModeIndex", 0);
        SpawnWeapon1 = PlayerPrefs.GetInt("SpawnWeapon1", 0);
        SpawnWeapon2 = PlayerPrefs.GetInt("SpawnWeapon2", 7);
        MedkitCanSpawn = PlayerPrefs.GetInt("MedkitCanSpawn", 1);
        GrenadeCanSpawn = PlayerPrefs.GetInt("GrenadeCanSpawn", 1);
        WeaponCanSpawn = PlayerPrefs.GetInt("WeaponCanSpawn", 1);

        ScoreToWin = PlayerPrefs.GetInt("ScoreToWin", 20);
        TimeLimitMin = PlayerPrefs.GetInt("TimeLimitMin", 15);
    }
    
    /// <summary>
    /// Saves current settings to PlayerPrefs
    /// </summary>
    public void SaveSettings() {
        // Network Settings
        PlayerPrefs.SetString("ServerName", ServerName);
        PlayerPrefs.SetString("PlayerName", PlayerName);
        PlayerPrefs.SetString("PortNumStr", PortNumStr);
        PlayerPrefs.SetString("IpAddress", IpAddress);
        PlayerPrefs.SetString("Password", Password);

        // Control Settings
        PlayerPrefs.SetFloat("xMouseSensitivity", xMouseSensitivity);
        PlayerPrefs.SetFloat("yMouseSensitivity", yMouseSensitivity);
        PlayerPrefs.SetInt("MouseYDirection", MouseYDirection);
        PlayerPrefs.SetFloat("FieldOfView", FieldOfView);
        PlayerPrefs.SetInt("AutoPickup", AutoPickup);
        
        //Game Settings
        PlayerPrefs.SetInt("LevelIndex", LevelIndex); 
        PlayerPrefs.SetInt("GameModeIndex", GameModeIndex);
        PlayerPrefs.SetInt("SpawnWeapon1", SpawnWeapon1);
        PlayerPrefs.SetInt("SpawnWeapon2", SpawnWeapon2);
        PlayerPrefs.SetInt("MedkitCanSpawn", MedkitCanSpawn);
        PlayerPrefs.SetInt("GrenadeCanSpawn", GrenadeCanSpawn);
        PlayerPrefs.SetInt("WeaponCanSpawn", WeaponCanSpawn);

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

}
