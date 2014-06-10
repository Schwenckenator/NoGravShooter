﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineTerrainMeshes : MonoBehaviour {

	// Use this for initialization
	void Start () {
		foreach(Transform child in transform)
			child.position += transform.position;
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length-1];
		int index = 0;
		for( int i=0; i < meshFilters.Length; i++){
			if(meshFilters[i].sharedMesh == null) continue;

			combine[index].mesh = meshFilters[i].sharedMesh;
			combine[index++].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].renderer.enabled = false;
		}
		GetComponent<MeshFilter>().mesh = new Mesh();
		GetComponent<MeshFilter>().mesh.CombineMeshes(combine, false);
		//renderer.material = meshFilters[1].renderer.sharedMaterial;

		index = 0;
		Material[] mats = new Material[combine.Length];
		for(int i=0; i < meshFilters.Length; i++){
			if(meshFilters[i].renderer.sharedMaterial == null) continue;
			mats[index++] = meshFilters[i].renderer.sharedMaterial;
		}
		renderer.materials = mats;
	}
}
