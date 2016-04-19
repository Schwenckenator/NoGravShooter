using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
/// <summary>
/// Handles gameplay
/// </summary>
public class GameManager : NetworkBehaviour {

    static public bool IsTutorial() {
        return false; //TODO Make real
    }
}
