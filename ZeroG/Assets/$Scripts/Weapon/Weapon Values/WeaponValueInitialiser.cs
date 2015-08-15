﻿using UnityEngine;
using System.Collections;

public class WeaponValueInitialiser : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);

        Weapon[] weapons = GetComponents<Weapon>();
        foreach (Weapon weap in weapons) {
            Debug.Log(weap.ToString());
            GameManager.weapon.Add(weap);
        }
	}
}
