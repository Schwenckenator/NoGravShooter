using UnityEngine;
using System.Collections;

public class NewtonianJetpack : MonoBehaviour, IJetpackForce {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public float GetXForce() {
        throw new System.NotImplementedException();
    }

    public float GetYForce() {
        throw new System.NotImplementedException();
    }

    public float GetZForce() {
        throw new System.NotImplementedException();
    }
}
