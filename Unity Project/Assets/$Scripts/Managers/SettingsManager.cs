using UnityEngine;
using System.Collections;

public class SettingsManager : MonoBehaviour {
    public enum KeyBind { MoveForward, MoveBack, MoveLeft, MoveRight, RollLeft, RollRight, JetUp, JetDown, Reload, Grenade, Interact, GrenadeSwitch };
    public static KeyCode[] keyBindings;

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
    public int LevelIndex { get; set; }
    public int GameModeIndex { get; set; }
    public int SpawnWeapon1 { get; set; }
    public int SpawnWeapon2 { get; set; }

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
        ServerName = PlayerPrefs.GetString("serverName");
        PlayerName = PlayerPrefs.GetString("playerName", "Player");
        PortNumStr = PlayerPrefs.GetString("portNumStr", "25000");
        IpAddress = PlayerPrefs.GetString("ipAddress", "127.0.0.1");
        Password = PlayerPrefs.GetString("password", "");

        //Control Settings
        xMouseSensitivity = PlayerPrefs.GetFloat("xMouseSensitivity", 0.5f);
        yMouseSensitivity = PlayerPrefs.GetFloat("yMouseSensitivity", 0.5f);
        MouseYDirection = PlayerPrefs.GetInt("mouseYDirection", -1);
        FieldOfView = PlayerPrefs.GetFloat("FieldOfView", 70);
        AutoPickup = PlayerPrefs.GetInt("autoPickup", 0);

        //Game Settings
        LevelIndex = PlayerPrefs.GetInt("levelIndex", 0);
        GameModeIndex = PlayerPrefs.GetInt("gameModeIndex", 0);
        SpawnWeapon1 = PlayerPrefs.GetInt("spawnWeapon1", 0);
        SpawnWeapon2 = PlayerPrefs.GetInt("spawnWeapon2", 7);
        MedkitCanSpawn = PlayerPrefs.GetInt("medkitCanSpawn", 1);
        GrenadeCanSpawn = PlayerPrefs.GetInt("grenadeCanSpawn", 1);
        WeaponCanSpawn = PlayerPrefs.GetInt("weaponCanSpawn", 1);
    }
    
    /// <summary>
    /// Saves current settings to PlayerPrefs
    /// </summary>
    public void SaveSettings() {
        // Network Settings
        PlayerPrefs.SetString("serverName", ServerName);
        PlayerPrefs.SetString("playerName", PlayerName);
        PlayerPrefs.SetString("portNumStr", PortNumStr);
        PlayerPrefs.SetString("ipAddress", IpAddress);
        PlayerPrefs.SetString("password", Password);

        // Control Settings
        PlayerPrefs.SetFloat("xMouseSensitivity", xMouseSensitivity);
        PlayerPrefs.SetFloat("yMouseSensitivity", yMouseSensitivity);
        PlayerPrefs.SetInt("mouseYDirection", MouseYDirection);
        PlayerPrefs.SetFloat("FieldOfView", FieldOfView);
        PlayerPrefs.SetInt("autoPickup", AutoPickup);
        
        //Game Settings
        PlayerPrefs.SetInt("levelIndex", LevelIndex); 
        PlayerPrefs.SetInt("gameModeIndex", GameModeIndex);
        PlayerPrefs.SetInt("spawnWeapon1", SpawnWeapon1);
        PlayerPrefs.SetInt("spawnWeapon2", SpawnWeapon2);
        PlayerPrefs.SetInt("medkitCanSpawn", MedkitCanSpawn);
        PlayerPrefs.SetInt("grenadeCanSpawn", GrenadeCanSpawn);
        PlayerPrefs.SetInt("weaponCanSpawn", WeaponCanSpawn);
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
