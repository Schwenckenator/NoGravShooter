using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIGameSettings : MonoBehaviour {

    static ChangeableText gameMode;
    static ChangeableText mapSelect;
    static ChangeableText weapon1;
    static ChangeableText weapon2;

    static ChangeableInputField timeLimit;
    static ChangeableInputField scoreLimit;

    static Toggle medkitSpawn;
    static Toggle grenadeSpawn;
    static Toggle weaponSpawn;

	// Use this for initialization
	void Start () {
        TextSetup();
        InputSetup();
        ToggleSetup();

        // Turn self off after initialsation
        gameObject.SetActive(false);
	}

    #region Setup
    private void TextSetup() {
        ChangeableText[] texts = GetComponentsInChildren<ChangeableText>();

        foreach (var text in texts) {
            if (text.IsType("gameModeSelect")) gameMode = text;
            else if (text.IsType("mapSelect")) mapSelect = text;
            else if (text.IsType("weaponSelect1")) weapon1 = text;
            else if (text.IsType("weaponSelect2")) weapon2 = text;

        }
        GameModeSelect(SettingsManager.singleton.GameModeIndexServer);
        MapSelect(SettingsManager.singleton.LevelIndex);
        Weapon1Set(SettingsManager.singleton.SpawnWeapon1);
        Weapon2Set(SettingsManager.singleton.SpawnWeapon2);
    }

    private void InputSetup() {
        ChangeableInputField[] inputs = GetComponentsInChildren<ChangeableInputField>();

        foreach (var input in inputs) {
            if (input.IsType("timeLimit")) timeLimit = input;
            else if (input.IsType("scoreLimit")) scoreLimit = input;
        }

        timeLimit.SetText(SettingsManager.singleton.TimeLimitMin.ToString());
        scoreLimit.SetText(SettingsManager.singleton.ScoreToWinServer.ToString());
    }
    
    private void ToggleSetup() {
        Toggle[] toggles = GetComponentsInChildren<Toggle>();
        medkitSpawn = toggles[0];
        grenadeSpawn = toggles[1];
        weaponSpawn = toggles[2];
        medkitSpawn.isOn = (SettingsManager.singleton.MedkitCanSpawn == 1);
        grenadeSpawn.isOn = (SettingsManager.singleton.GrenadeCanSpawn == 1);
        weaponSpawn.isOn = (SettingsManager.singleton.WeaponCanSpawn == 1);
    }
    #endregion
    #region ButtonPushes
    public void SaveSettings() {
        SettingsManager.singleton.SaveSettings();
        SettingsManager.singleton.RelayGameMode();
    }
    public void GameModeSelect(int id) {
        SettingsManager.singleton.GameModeIndexServer = id;
        gameMode.SetText(SettingsManager.singleton.GameModeList[id]);
    }
    public void MapSelect(int id) {
        SettingsManager.singleton.LevelIndex = id;
        mapSelect.SetText(SettingsManager.singleton.LevelName);
    }
    public void Weapon1Set(int id) {
        SettingsManager.singleton.SpawnWeapon1 = id;
        weapon1.SetText(GameManager.weapon[id].name);
    }
    public void Weapon2Set(int id) {
        SettingsManager.singleton.SpawnWeapon2 = id;
        weapon2.SetText(GameManager.weapon[id].name);
    }
    public void TimeLimit(string min) {
        if (min != "") {
            SettingsManager.singleton.TimeLimitMin = int.Parse(min);
        }
    }
    public void ScoreLimit(string score) {
        if (score != "") { 
            SettingsManager.singleton.ScoreToWinServer = int.Parse(score); 
        }
    }
    public void MedkitToggle(bool value) {
        SettingsManager.singleton.MedkitCanSpawn = value ? 1 : 0;
    }
    public void GrenadeToggle(bool value) {
        SettingsManager.singleton.GrenadeCanSpawn = value ? 1 : 0;
    }
    public void WeaponToggle(bool value) {
        SettingsManager.singleton.WeaponCanSpawn = value ? 1 : 0;
    }
    public void AllWeaponToggle(bool value) {
        DebugManager.SetAllWeapon(value);
    }
    public void AllAmmoToggle(bool value) {
        DebugManager.SetAllAmmo(value);
    }
    public void AllGrenadeToggle(bool value) {
        DebugManager.SetAllGrenade(value);
    }
    public void AllFuelToggle(bool value) {
        DebugManager.SetAllFuel(value);
    }
    #endregion
}
