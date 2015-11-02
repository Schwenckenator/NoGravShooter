using UnityEngine;
using System.Collections;

public enum KeyBind { MoveForward, MoveBack, MoveLeft, MoveRight, RollLeft, RollRight, JetUp, JetDown, Reload, Grenade, GrenadeSwitch, Interact, StopMovement };

public class SettingsManager : MonoBehaviour {

    public static SettingsManager singleton { get; private set; }

    public static KeyCode[] keyBindings;
    const int SecondsInMinute = 60;
    

    private string[] levelList = { "Manufactorium", "DestroyedCruiser", "SpaceStation", "Arena", "TestZone", "AsteroidBunker" };
    private string[] gameModeList = { "DeathMatch", "Team DeathMatch", "Skirmish", "Team Skirmish", "Capture the Flag", "Extraction", "Elimination", "Infection" };
    private string[] publicgameModeList = { "DeathMatch", "Team DeathMatch", "Skirmish", "Team Skirmish", "Team DeathMatch", "Team Skirmish", "DeathMatch", "Skirmish" };
    private string[] admingameModeList = { "DeathMatch", "Team DeathMatch", "Skirmish", "Team Skirmish", "Capture the Flag", "Extraction", "Elimination", "Infection" };

    public string[] LevelList { get { return levelList; } }
    public string[] GameModeList { get { return gameModeList; } }

    // Seting ~~Server is the version saved in PlayerPrefs
    // Setting ~~Client is the version used for game logic, and displayed
    // Changing ~~Server automatically changed ~~Client for some properties

    #region Networking Settings
    #region ServerName
    private string serverNameServer;
    public string ServerNameServer {
        get {
            return serverNameServer;
        }
        set {
            serverNameServer = value;
            ServerNameClient = value;
        }
    }
    public string ServerNameClient { get; set; }
    #endregion

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
    private int i_AutoSpawn { get; set; }
    public bool AutoSpawn { get; set; }
    #endregion

    #region GameSettings
    #region LevelIndex
    private int levelIndex;
    public int LevelIndex {
        get {
            return levelIndex;
        }
        set {
            levelIndex = value;
            LevelName = levelList[value];
        }
    }
    public string LevelName { get; set; }
    #endregion
    #region GameMode
    private int gameModeIndex;
    public int GameModeIndexServer {
        get {
            return gameModeIndex;
        }
        set {
            gameModeIndex = value;
            GameModeIndexClient = value;
        }
    }

    private int gameModeIndexClient;
    public int GameModeIndexClient {
        get { return gameModeIndexClient; }
        set {
            gameModeIndexClient = value;
            GameModeName = GameModeList[value];
        }
    }
    public string GameModeName { get; set; }
    #endregion
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
    public int ScoreToWinServer {
        get {
            return _scoreToWin;
        }
        set {
            value = Mathf.Max(value, 0);
            _scoreToWin = value;
            ScoreToWinClient = value;
        }
    }
    public int ScoreToWinClient { get; set; }

    // Bonus spawning
    public int MedkitCanSpawn { get; set; }
    public int GrenadeCanSpawn { get; set; }
    public int WeaponCanSpawn { get; set; } 
	
	//Colour setting
    public float ColourR { get; set; }
    public float ColourG { get; set; }
    public float ColourB { get; set; }

    public float VolumeMaster { get; set; }
    public float VolumeMusic { get; set; }
    public float VolumeEffects { get; set; }
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

    //NetworkView //NetworkView;
    void Awake() {
        singleton = this;
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
        keyBindings[(int)KeyBind.GrenadeSwitch] = (KeyCode)PlayerPrefs.GetInt("bindGrenadeSwitch",  (int)KeyCode.H);
        keyBindings[(int)KeyBind.Interact]      = (KeyCode)PlayerPrefs.GetInt("bindInteract",       (int)KeyCode.F);
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
        ServerNameServer = PlayerPrefs.GetString("ServerName");
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
        i_AutoSpawn = PlayerPrefs.GetInt("AutoSpawn", i_False);

        //Game Settings
        LevelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
        GameModeIndexServer = PlayerPrefs.GetInt("GameModeIndex", 0);
        SpawnWeapon1 = PlayerPrefs.GetInt("SpawnWeapon1", laserRifleIndex);
        SpawnWeapon2 = PlayerPrefs.GetInt("SpawnWeapon2", noWeaponIndex);
        MedkitCanSpawn = PlayerPrefs.GetInt("MedkitCanSpawn", i_True);
        GrenadeCanSpawn = PlayerPrefs.GetInt("GrenadeCanSpawn", i_True);
        WeaponCanSpawn = PlayerPrefs.GetInt("WeaponCanSpawn", i_True);
		
		//Colour setting
		ColourR = PlayerPrefs.GetFloat("ColourR", 0.6f);
		ColourG = PlayerPrefs.GetFloat("ColourG", 0.6f);
		ColourB = PlayerPrefs.GetFloat("ColourB", 0.6f);

        // Audio Settings

        VolumeMaster = PlayerPrefs.GetFloat("VolumeMaster", 1.0f);
        VolumeMusic = PlayerPrefs.GetFloat("VolumeMusic", 1.0f);
        VolumeEffects = PlayerPrefs.GetFloat("VolumeEffects", 1.0f);

        ScoreToWinServer = PlayerPrefs.GetInt("ScoreToWin", 20);
        TimeLimitMin = PlayerPrefs.GetInt("TimeLimitMin", 15);

        ConvertSettingsIntToBool();
    }
    
    /// <summary>
    /// Saves current settings to PlayerPrefs
    /// </summary>
    public void SaveSettings() {
        
        ConvertSettingsBoolToInt();

        // Network Settings
        PlayerPrefs.SetString("ServerName", ServerNameServer);
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
        PlayerPrefs.SetInt("AutoSpawn", i_AutoSpawn);
        
        //Game Settings
        PlayerPrefs.SetInt("LevelIndex", LevelIndex); 
        PlayerPrefs.SetInt("GameModeIndex", GameModeIndexServer);
        PlayerPrefs.SetInt("SpawnWeapon1", SpawnWeapon1);
        PlayerPrefs.SetInt("SpawnWeapon2", SpawnWeapon2);
        PlayerPrefs.SetInt("MedkitCanSpawn", MedkitCanSpawn);
        PlayerPrefs.SetInt("GrenadeCanSpawn", GrenadeCanSpawn);
        PlayerPrefs.SetInt("WeaponCanSpawn", WeaponCanSpawn);
		
		//Colour setting
		PlayerPrefs.SetFloat("ColourR", ColourR);
		PlayerPrefs.SetFloat("ColourG", ColourG);
		PlayerPrefs.SetFloat("ColourB", ColourB);

        //Audio Setting
        PlayerPrefs.SetFloat("VolumeMaster", VolumeMaster);
        PlayerPrefs.SetFloat("VolumeMusic", VolumeMusic);
        PlayerPrefs.SetFloat("VolumeEffects", VolumeEffects);

        PlayerPrefs.SetInt("ScoreToWin", ScoreToWinServer);
        PlayerPrefs.SetInt("TimeLimitMin", TimeLimitMin);
    }

    /// <summary>
    ///  Returns -1 on error
    /// </summary>
    public void ParsePortNumber() {
        int temp = 0;
        if (int.TryParse(PortNumStr, out temp)) {
            PortNum = temp;
        } else {
            PortNum = -1;
        }
    }
    public bool[] GetAllowedBonuses() {
        // Needs order: Medkit, Grenade, weapon
        bool[] temp = {(MedkitCanSpawn == 1),(GrenadeCanSpawn == 1),(WeaponCanSpawn == 1)} ;
        return temp;
    }
    private void ConvertSettingsIntToBool() {
        AutoPickup = (i_AutoPickup == i_True);
        AutoSpawn = (i_AutoSpawn == i_True);
    }
    private void ConvertSettingsBoolToInt() {
        i_AutoPickup = AutoPickup ? i_True : i_False;
        i_AutoSpawn = AutoSpawn ? i_True : i_False;
    }

    public void ClearPasswordClient() {
        PasswordClient = "";
    }
    public Color GetPlayerColour() {
        return new Color(ColourR*2, ColourG*2, ColourB*2, 1);
    }
    public bool IsTeamGameMode() {
        // Team : Team Deathmatch, Capture the Flag, Extraction, Team Skirmish
        // No Team: Deathmatch, Skirmish, Elimination, Infection <- There are teams but no choice.
        return (GameModeIndexClient == 1 || GameModeIndexClient == 3 || GameModeIndexClient == 4 || GameModeIndexClient == 5);
    }

    #region RPCSettings
    public void RelayServerName() {
        //NetworkView.RPC("RPC_RelayServerName", RPCMode.OthersBuffered, this.ServerNameServer);
        UILobby.singleton.SetServerName();// Writes to lobby title
    }
    //[RPC]
    private void RPC_RelayServerName(string inServerName) {
        this.ServerNameClient = inServerName;
        UILobby.singleton.SetServerName(); // Writes to lobby title
    }

    public void RelayScoreToWin() {
        //NetworkView.RPC("RPC_RelayScoreToWin", RPCMode.OthersBuffered, this.ScoreToWinServer);
    }
    //[RPC]
    private void RPC_RelayScoreToWin(int inScoreToWin) {
        this.ScoreToWinClient = inScoreToWin;
    }
    
    public void RelayGameMode() {
        //NetworkView.RPC("RPC_RelayGameMode", RPCMode.AllBuffered, SettingsManager.instance.GameModeIndexServer);
    }
    //[RPC]
    void RPC_RelayGameMode(int index) {
        SettingsManager.singleton.GameModeIndexClient = index;
        NetworkManager.single.AssignMyPlayerToTeam();
        UIChat.UpdatePlayerLists();
    }
    #endregion
}
