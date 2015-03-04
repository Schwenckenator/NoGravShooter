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

    public void AssignColour(GameObject player) {
        PlayerColour playerColour = player.GetComponent<PlayerColour>();

        playerColour.AssignPlayerColour();
    }

}
