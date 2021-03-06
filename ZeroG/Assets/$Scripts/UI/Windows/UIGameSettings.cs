﻿using UnityEngine;
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
        GameModeSelect(SettingsManager.instance.GameModeIndexServer);
        MapSelect(SettingsManager.instance.LevelIndex);
        Weapon1Set(SettingsManager.instance.SpawnWeapon1);
        Weapon2Set(SettingsManager.instance.SpawnWeapon2);
    }

    private void InputSetup() {
        ChangeableInputField[] inputs = GetComponentsInChildren<ChangeableInputField>();

        foreach (var input in inputs) {
            if (input.IsType("timeLimit")) timeLimit = input;
            else if (input.IsType("scoreLimit")) scoreLimit = input;
        }

        timeLimit.SetText(SettingsManager.instance.TimeLimitMin.ToString());
        scoreLimit.SetText(SettingsManager.instance.ScoreToWinServer.ToString());
    }
    
    private void ToggleSetup() {
        Toggle[] toggles = GetComponentsInChildren<Toggle>();
        medkitSpawn = toggles[0];
        grenadeSpawn = toggles[1];
        weaponSpawn = toggles[2];
        medkitSpawn.isOn = (SettingsManager.instance.MedkitCanSpawn == 1);
        grenadeSpawn.isOn = (SettingsManager.instance.GrenadeCanSpawn == 1);
        weaponSpawn.isOn = (SettingsManager.instance.WeaponCanSpawn == 1);
    }
    #endregion
    #region ButtonPushes
    public void SaveSettings() {
        SettingsManager.instance.SaveSettings();
        SettingsManager.instance.RelayGameMode();
    }
    public void GameModeSelect(int id) {
        SettingsManager.instance.GameModeIndexServer = id;
        gameMode.SetText(SettingsManager.instance.GameModeList[id]);
    }
    public void MapSelect(int id) {
        SettingsManager.instance.LevelIndex = id;
        mapSelect.SetText(SettingsManager.instance.LevelName);
    }
    public void Weapon1Set(int id) {
        SettingsManager.instance.SpawnWeapon1 = id;
        weapon1.SetText(GameManager.weapon[id].name);
    }
    public void Weapon2Set(int id) {
        SettingsManager.instance.SpawnWeapon2 = id;
        weapon2.SetText(GameManager.weapon[id].name);
    }
    public void TimeLimit(string min) {
        if (min != "") {
            SettingsManager.instance.TimeLimitMin = int.Parse(min);
        }
    }
    public void ScoreLimit(string score) {
        if (score != "") { 
            SettingsManager.instance.ScoreToWinServer = int.Parse(score); 
        }
    }
    public void MedkitToggle(bool value) {
        SettingsManager.instance.MedkitCanSpawn = value ? 1 : 0;
    }
    public void GrenadeToggle(bool value) {
        SettingsManager.instance.GrenadeCanSpawn = value ? 1 : 0;
    }
    public void WeaponToggle(bool value) {
        SettingsManager.instance.WeaponCanSpawn = value ? 1 : 0;
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
