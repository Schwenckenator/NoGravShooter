using UnityEngine;
using System.Collections;

public class ActorMotorManager : MonoBehaviour {

    IActorMotor walkingMotor;
    IActorMotor jetpackMotor;

    IActorMotor currentMotor;

	// Use this for initialization
	void Start () {
        jetpackMotor = GetComponent<ActorJetpackMotor>();
        walkingMotor = GetComponent<ActorWalkingMotor>();

        currentMotor = jetpackMotor;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        currentMotor.Movement();
	}
}
