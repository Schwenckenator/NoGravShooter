﻿using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class FloatingRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(RandomRotate());
	}

    IEnumerator RandomRotate() {
        float delay = Random.Range(0.5f, 1.5f);
        yield return new WaitForSeconds(delay);

        Vector3 random = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        rigidbody.AddTorque(random);
    }
}
