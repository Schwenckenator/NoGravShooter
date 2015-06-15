using UnityEngine;
using System.Collections;

public class RadarDot{
    public static int radarCenter = 110;
    public static int radarPadding = 20;

    static int radarDotArea = 95;
    static int defaultdotsize = 30;
    static int detectionRadius = 100;
    public static Transform myTransform;

    public GameObject Actor { get; private set; }
    public Vector3 Position { get; private set; } // X, right; Y, size; Z, forward;
    public string Type { get; private set; }
    public bool CanDisplay { get; private set; }
    
    private Vector3 ObjectPosition { get; set; }

    public RadarDot(GameObject actor, string type) {
        if (type == "ERROR") throw new RadarDotIncorrectTypeException();
        this.Actor = actor;
        this.Type = type;
        this.Position = Vector3.zero;
        this.CanDisplay = false;
    }
    public static string DotType(string tag, TeamColour team = TeamColour.None) {
        if (tag == "BonusPickup") {
            return "Item";
        }
        if (tag == "Player") {
            if (NetworkManager.MyPlayer().IsOnTeam(team)) {
                return "Ally";
            } else {
                return "Enemy";
            }
        }
        return "ERROR";
    }

    public void UpdatePosition() {
        ObjectPosition = myTransform.InverseTransformPoint(Actor.transform.position);
        float sqrMag = ObjectPosition.sqrMagnitude;

        if (sqrMag < detectionRadius * detectionRadius) {
            CanDisplay = true;
            float posX = ObjectPosition.x / detectionRadius * radarDotArea;
            float posY = (ObjectPosition.y / detectionRadius * defaultdotsize) + defaultdotsize;
            float posZ = -ObjectPosition.z / detectionRadius * radarDotArea;
            Position = new Vector3(posX, posY, posZ);
        }else{
            CanDisplay = false;
        }
    }

    public static bool IsBelowDefault(RadarDot dot) {
        return dot.Position.y < defaultdotsize;
    }
}

public class RadarDotIncorrectTypeException : System.Exception {
    public RadarDotIncorrectTypeException() { }
}
