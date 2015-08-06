using UnityEngine;
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

    public Color RedTeamLimit;
    public Color BlueTeamLimit;

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
    public Color LimitTeamColour(TeamColour team, Color input) {
        if (team == TeamColour.Red && !GameManager.IsSceneTutorial()) 
            return LimitRedTeamColour(input);
        if (team == TeamColour.Blue && !GameManager.IsSceneTutorial()) 
            return LimitBlueTeamColour(input);
        return input;
    }
    private Color LimitRedTeamColour(Color input) {
        Color newColour = new Color(0, 0, 0, 1);
        
        //Max the main colour
        newColour.r = Mathf.Max(input.r, RedTeamLimit.r*2);

        //Lessen the supporting colours
        newColour.g = Mathf.Min(input.g, RedTeamLimit.g*2);
        newColour.b = Mathf.Min(input.b, RedTeamLimit.b*2);
        
        return newColour;
    }
    private Color LimitBlueTeamColour(Color input) {
        Color newColour = new Color(0, 0, 0, 1);

        //Lessen the supporting colours
        newColour.r = Mathf.Min(input.r, BlueTeamLimit.r*2);
        newColour.g = Mathf.Min(input.g, BlueTeamLimit.g*2);

        //Max the main colour
        newColour.b = Mathf.Max(input.b, BlueTeamLimit.b*2);

        return newColour;
    }
}
