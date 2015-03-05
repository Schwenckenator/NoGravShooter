﻿using UnityEngine;
using System.Collections;

public class PlayerColourManager : MonoBehaviour {

    #region Instance
    //Here is a private reference only this class can access
    private static PlayerColourManager _instance;
    //This is the public reference that other classes will use
    public static PlayerColourManager instance {
        get {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<PlayerColourManager>();
            }
            return _instance;
        }
    }
    #endregion

    public Color RedTeamLimit = new Color(0.85f, 0.5f, 0.75f); // 216, 127, 191
    public Color BlueTeamLimit = new Color(0.5f, 0.75f, 0.85f); // 127, 191, 216

    public void AssignColour(GameObject player) {
        PlayerColour playerColour = player.GetComponent<PlayerColour>();

        playerColour.AssignPlayerColour();
    }
    
    /// <summary>
    /// If team == Team.None, return input colour with no change
    /// </summary>
    /// <param name="team"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public Color LimitTeamColour(Team team, Color input) {
        if (team == Team.Red) 
            return LimitRedTeamColour(input);
        if (team == Team.Blue) 
            return LimitBlueTeamColour(input);
        return input;
    }
    private Color LimitRedTeamColour(Color input) {
        Color newColour = new Color(0, 0, 0, 1);
        
        //Max the main colour
        newColour.r = Mathf.Max(input.r, RedTeamLimit.r);

        //Lessen the supporting colours
        newColour.g = Mathf.Min(input.g, RedTeamLimit.g);
        newColour.b = Mathf.Min(input.b, RedTeamLimit.b);
        
        return newColour;
    }
    private Color LimitBlueTeamColour(Color input) {
        Color newColour = new Color(0, 0, 0, 1);

        //Lessen the supporting colours
        newColour.r = Mathf.Min(input.r, BlueTeamLimit.r);
        newColour.g = Mathf.Min(input.g, BlueTeamLimit.g);

        //Max the main colour
        newColour.b = Mathf.Max(input.b, BlueTeamLimit.b);

        return newColour;
    }
}
