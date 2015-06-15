using UnityEngine;
using System.Collections;

public class PlayerGUI : MonoBehaviour {

    public Texture empty;
    public Texture fullFuel;
    public Texture fullHealth;
    public Texture fullHeat;
    public Texture crosshair;

    public Texture bloodyScreen; // Needs to be public

    public Texture[] EMPradar;
    public Texture[] EMPcursor;
    public Texture[] EMPstats;

    public Texture SniperScope;
    public Texture SniperScopeBorder;
    public bool showSniperScope = false;

    public GUIStyle customGui;
    public Texture2D radar;
    public Texture2D playericon;
    public Texture2D enemyicon;
    public Texture2D itemicon;
    public Texture2D allyicon;

    public int detectionRadius;
    public bool detectItems;



    private PlayerResources playerResource;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {
        if (!GameManager.instance.IsPlayerSpawned()) return; // No Player, no gui

    }
}
