using UnityEngine;
using System.Collections;

public class ActorGrenades : MonoBehaviour {

    private int[] grenades;                     // Id is alphabetical
    private int grenadeTypes = 3;               // Black Hole, EMP, Frag
    private static int currentGrenadeType = 0;  //      0       1     2

    //NetworkView //NetworkView;

    // Use this for initialization
    void Awake () {
        //NetworkView = GetComponent<//NetworkView>();
        Reset();
	}

    public void Reset() {
        grenades = new int[grenadeTypes];
        for (int i = 0; i < grenadeTypes; i++) {
            grenades[i] = 0;
        }
        //if (!//NetworkView.isMine) {
        //    this.enabled = false;
        //}
    }
	
	// Update is called once per frame
	void Update () {
        if (InputKey.GetKeyDown(KeyBind.GrenadeSwitch)) {
            ChangeGrenade();
        }
    }
    public bool CanThrowGrenade() {

        if (DebugManager.allGrenade) grenades[currentGrenadeType]++;

        if (grenades[currentGrenadeType] > 0) {
            grenades[currentGrenadeType]--;
            return true;
        } else {
            return false;
        }
    }
    public int GetCurrentGrenadeCount() {
        return grenades[currentGrenadeType];
    }
    public int GetCurrentGrenadeType() {
        return currentGrenadeType;
    }
    public void ChangeGrenade() {
        currentGrenadeType++;
        currentGrenadeType %= grenadeTypes;
        for (int i = 0; i < grenadeTypes; i++) {
            if (grenades[currentGrenadeType] == 0) {
                currentGrenadeType++;
                currentGrenadeType %= grenadeTypes;
            }
        }
    }
    public void ChangeGrenade(int typeOfGrenade) {
        currentGrenadeType = typeOfGrenade;
    }
    public void PickUpGrenades(int amount, int grenadeId) {
        grenades[grenadeId] += amount;
    }
}
