using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIGameSettings : MonoBehaviour {

    public Dropdown gameMode;
    public Dropdown mapSelect;
    public Dropdown weapon1;
    public Dropdown weapon2;

    static ChangeableInputField timeLimit;
    static ChangeableInputField scoreLimit;

    static Toggle medkitSpawn;
    static Toggle grenadeSpawn;
    static Toggle weaponSpawn;

	// Use this for initialization
	void Start () {
        InputSetup();
        ToggleSetup();
        DropdownSetup();
    }

    void DropdownSetup() {
        //gameMode.captionText.text = SettingsManager.singleton.GameModeList[SettingsManager.singleton.GameModeIndex];
        gameMode.value = SettingsManager.singleton.GameModeIndex;

        //mapSelect.captionText.text = SettingsManager.singleton.LevelName;

        mapSelect.value = SettingsManager.singleton.LevelIndex;
        //weapon1.captionText.text = WeaponManager.weapon[SettingsManager.singleton.SpawnWeapon1].name;

        //weapon2.captionText.text = WeaponManager.weapon[SettingsManager.singleton.SpawnWeapon2].name;
        weapon1.value = SettingsManager.singleton.SpawnWeapon1;
        weapon2.value = SettingsManager.singleton.SpawnWeapon2;
    }

    #region Setup
        private void InputSetup() {
        ChangeableInputField[] inputs = GetComponentsInChildren<ChangeableInputField>();

        foreach (var input in inputs) {
            if (input.IsType("timeLimit")) timeLimit = input;
            else if (input.IsType("scoreLimit")) scoreLimit = input;
        }

        timeLimit.SetText(SettingsManager.singleton.TimeLimitMin.ToString());
        scoreLimit.SetText(SettingsManager.singleton.ScoreToWin.ToString());
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
    public void GameModeSelect(int id) {
        SettingsManager.singleton.GameModeIndex = id;
    }
    public void MapSelect(int id) {
        SettingsManager.singleton.LevelIndex = id;
    }
    public void Weapon1Set(int id) {
        SettingsManager.singleton.SpawnWeapon1 = id;
    }
    public void Weapon2Set(int id) {
        SettingsManager.singleton.SpawnWeapon2 = id;
    }
    public void TimeLimit(string min) {
        if (min != "") {
            SettingsManager.singleton.TimeLimitMin = int.Parse(min);
        }
    }
    public void ScoreLimit(string score) {
        if (score != "") { 
            SettingsManager.singleton.ScoreToWin = int.Parse(score); 
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
        DebugManager.allWeapon = value;
    }
    public void AllAmmoToggle(bool value) {
        DebugManager.allAmmo = value;
    }
    public void AllGrenadeToggle(bool value) {
        DebugManager.allGrenade = value;
    }
    public void AllFuelToggle(bool value) {
        DebugManager.allFuel = value;
    }
    #endregion
}
