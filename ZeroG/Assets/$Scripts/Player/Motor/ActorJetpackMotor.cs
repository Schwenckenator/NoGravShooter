using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class ActorJetpackMotor : MonoBehaviour, IActorMotor {

    public AudioSource jetpackAudioSource;

    public AudioClip soundJetpackBurn;
    public float volumeJetpackBurn;
    public AudioClip soundJetpackEmpty;
    public float volumeJetpackEmpty;
    public AudioClip soundJetpackShutoff;
    public float volumeJetpackShutoff;

    private bool jetpackSoundWasPlayed = false;
    private bool playJetSound = false;
    private IControllerInput input;
    private Rigidbody rigidbody;
    private PlayerResources playerResource;
    private float fuelSpend;

	// Use this for initialization
	void Start () {
        IActorStats stats = gameObject.GetInterface<IActorStats>();
        fuelSpend = stats.fuelSpend;

        rigidbody = GetComponent<Rigidbody>();
        input = gameObject.GetInterface<IControllerInput>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
