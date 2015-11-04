using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour {

    public static WeaponManager singleton { get; private set; }
    public static List<Weapon> weapon = new List<Weapon>();

    static int maxStartingWeapons = 2;
    public int GetMaxStartingWeapons() { return maxStartingWeapons; }

    private int[] startingWeapons = new int[maxStartingWeapons];
    public int[] GetStartingWeapons() {
        return startingWeapons;
    }

    // Use this for initialization
    void Awake () {
        singleton = this;

		Weapon[] weapons = GetComponents<Weapon>();
        foreach (Weapon weap in weapons) {
            //Debug.Log(weap.ToString());
            weapon.Add(weap);
            
        }
        for (int i = 0; i < weapon.Count; i++) {
            weapon[i].id = i;
            weapon[i].Init();
            
        }
	}
}
