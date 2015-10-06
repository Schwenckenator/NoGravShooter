using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Radar : MonoBehaviour {
    #region Instance
    //Here is a private reference only this class can access
    private static Radar _instance;
    //This is the public reference that other classes will use
    public static Radar instance {
        get {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<Radar>();
            }
            return _instance;
        }
    }
    #endregion

    public GUIStyle radarGui;
    public Texture2D radar;
    public Texture2D playericon;
    public Texture2D enemyicon;
    public Texture2D itemicon;
    public Texture2D allyicon;

    public int detectionRadius;
    public bool detectItems;

    List<RadarDot> dots = new List<RadarDot>();
    List<RadarDot> deadDots = new List<RadarDot>();

    NetworkView networkView;
    void Start() {
        networkView = GetComponent<NetworkView>();
    }

    void OnGUI() {
        if (UIManager.IsCurrentMenuWindow(Menu.PlayerHUD)) {
            DrawRadar();
        }
    }

    public void ActorsChanged() {
        networkView.RPC("GatherDots", RPCMode.Others);
        GatherDots();
    }
    [RPC]
    void GatherDots() {
        dots.Clear();
        ChatManager.DebugMessage("Gathering Dots.");
        GameObject[] actors = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in actors) {
            if (obj.GetComponent<NetworkView>().isMine) {
                RadarDot.myTransform = obj.transform;
                RadarDot newDot = new RadarDot(obj, "Me");
                dots.Add(newDot);
            } else {
                string dotType = RadarDot.DotType(obj.tag, obj.GetComponent<ActorTeam>().GetTeam());
                RadarDot newDot = new RadarDot(obj, dotType);
                dots.Add(newDot);
            }
        }
        if (!detectItems) return;
        GameObject[] items = GameObject.FindGameObjectsWithTag("BonusPickup");
        foreach (GameObject obj in items) {
            string dotType = RadarDot.DotType(obj.tag);
            RadarDot newDot = new RadarDot(obj, dotType);
            dots.Add(newDot);
        }
    }

    void DrawRadar() {
        GUI.Box(new Rect(
            RadarDot.radarPadding, //Left
            Screen.height - (RadarDot.radarCenter * 2 + RadarDot.radarPadding), //Top
            RadarDot.radarCenter * 2, //width
            RadarDot.radarCenter * 2), // height
            radar, radarGui);

        List<RadarDot> before = new List<RadarDot>();
        RadarDot player = null;
        List<RadarDot> after = new List<RadarDot>();

        foreach (RadarDot dot in dots) {
            if (dot.Actor == null || !dot.Actor.activeInHierarchy) {
                // If dead add to list for removal
                deadDots.Add(dot);
                continue;
            }
            dot.UpdatePosition();
            if (!dot.CanDisplay) continue;

            if (dot.Type == "Me") {
                player = dot;
            } else {
                if (RadarDot.IsBelowDefault(dot)) {
                    before.Add(dot);
                } else {
                    after.Add(dot);
                }
            }
        }
        foreach (RadarDot dot in before) {
            DrawDot(dot);
        }
        DrawDot(player);
        foreach (RadarDot dot in after) {
            DrawDot(dot);
        }


        //Remove dead dots
        foreach (RadarDot dot in deadDots) {
            dots.Remove(dot);
        }
    }

    private void DrawDot(RadarDot dot) {
        Texture2D icon = GetIconType(dot.Type);

        GUI.Box(new Rect(RadarDot.radarCenter + RadarDot.radarPadding + dot.Position.x - dot.Position.y / 2, Screen.height - (RadarDot.radarCenter + RadarDot.radarPadding) + dot.Position.z - dot.Position.y / 2, dot.Position.y, dot.Position.y), icon, radarGui);
    }
    Texture2D GetIconType(string type) {
        switch (type) {
            case "Enemy":
                return enemyicon;
            case "Item":
                return itemicon;
            case "Ally":
                return allyicon;
            default:
                return playericon;
        }
    }
}
