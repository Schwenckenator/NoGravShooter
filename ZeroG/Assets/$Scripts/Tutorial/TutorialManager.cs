using UnityEngine;
using System.Collections;

public struct tutorialMessage {
    public string message;
    public float waitTime;
    public tutorialMessage(string str, float wait) {
        message = str;
        waitTime = wait;
    }
}

public class TutorialManager : MonoBehaviour {

    delegate void TutorialStep();
    private TutorialStep tutorialStep;

    private PlayerResources playerRes;
    private GameObject player;
    private Transform playerCamera;
    private IControllerInput playerInput;

    public GameObject[] flyingPlatforms;
    public GameObject[] walkingTiles;
    public GameObject lookingDot;

    private int dotLooks = 0;

	// Use this for initialization
	void Start () {
        GameManager.instance.SpawnActor();
        TutorialSetup();
        tutorialStep = SearchingForPlayer;
        Invoke("ClearScreen", 6f); // Time it takes to get through starting messages
	}
	// Update is called once per frame
	void Update () {
        tutorialStep();
	}
    #region message
    void DeployMessages(tutorialMessage[] input) {
        // Halt the old, begin the new.
        StopCoroutine("CoDeployMessages");
        StartCoroutine("CoDeployMessages", input);
    }
    /// <summary>
    /// Deploys a bunch of messages to the tutorial screen.
    /// Once here, it will send the messages for the display time. After with it will move to the next message.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    IEnumerator CoDeployMessages(tutorialMessage[] input) {
        foreach (tutorialMessage tutMessage in input) {
            ChatManager.TutorialChat(tutMessage.message);
            yield return new WaitForSeconds(tutMessage.waitTime);
        }
        yield return null;
    }
    #endregion

    void TutorialSetup() {
        // Build the messages
        tutorialMessage[] tuts = new tutorialMessage[7];
        string str = "Welcome to the SC1830 Utility Suit.\n\nCalibrating";
        for (int i = 0; i < 4; i++) {
            str += ".";
            tuts[i] = new tutorialMessage(str, 1f);
        }
        tuts[4] = new tutorialMessage("Welcome to the SC1830 Utility Suit.\n\nSuit Calibrated.", 2f);
        tuts[5] = new tutorialMessage("Welcome to the SC1830 Utility Suit.\n\nSuit Calibrated.\n\nRunning Tutorial Simulation.", 4f);
        tuts[6] = new tutorialMessage("\nMove your Mouse to look around.\n\nPlease look at the blue dot.", 5f);
        DeployMessages(tuts);
    }
    void ClearScreen() {
        Debug.Log("Clear screen called.");
        GetComponent<ScreenFadeInOut>().FadeToClear();
    }
    /// <summary>
    /// Finds the player Gameobject
    /// </summary>
    void SearchingForPlayer() {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            LookingSetup();
            tutorialStep = LookingTutorial;
        }
    }
    void LookingSetup() {
        playerInput = player.GetComponent(typeof(IControllerInput)) as IControllerInput;
        playerInput.canJump = false;
        playerInput.canWalk = false;

        playerCamera = player.transform.FindChild("CameraPos");

        lookingDot.transform.FindChild("DotMesh").GetComponent<Renderer>().material.color = new Color(0, 0, 200, 200);
        lookingDot.transform.position = new Vector3(-358f, -10.6f, 0f);
    }
    void LookingTutorial() {
        float minAngle = 1f;
        // If bigger than min angle, return
        if (Vector3.Angle(playerCamera.forward, lookingDot.transform.position - player.transform.position) > minAngle) return;

        // A dot was looked at
        Vector3 newDotPos = Vector3.zero;

        switch (++dotLooks) {
            case(1):
                newDotPos = new Vector3(-358f, -10.6f, -3f);
                break;
            case(2):
                newDotPos = new Vector3(-358f, -7.6f, 0f);
                break;
            case(3):
                newDotPos = new Vector3(-358f, -10.6f, 3f);
                break;
            case(4):
                newDotPos = new Vector3(-358f, -13.6f, 0f);
                break;
            default:
                // Done or broken
                newDotPos = new Vector3(-358f, -120f, 0f);
                break;
        }
        lookingDot.transform.position = newDotPos;

    }
}
