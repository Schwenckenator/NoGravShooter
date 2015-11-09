﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class WeaponManager : MonoBehaviour {

    public static WeaponManager singleton { get; private set; }
    public static List<Weapon> weapon;

    static int maxStartingWeapons = 2;
    public static int GetMaxStartingWeapons() { return maxStartingWeapons; }

    public SyncListInt GetStartingWeapons() {
        return NetworkInfoWrapper.singleton.startingWeapons;
    }

    // Use this for initialization
    void Start () {
        singleton = this;
        weapon = new List<Weapon>();

        Weapon[] weapons = GetComponents<Weapon>();
        int index = 0;
        foreach (Weapon weap in weapons) {
            weapon.Add(weap);
            weap.id = index++;
            weap.Init();
        }
	}

    public void ResetWeapons() {
        foreach (Weapon weap in weapon) {
            weap.ResetVariables();
        }
    }
}