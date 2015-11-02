//using UnityEngine;
//using System.Collections;

//public class WalkableMovingObject : MonoBehaviour {

//    //void OnCollisionEnter(Collision info){
//    //    Debug.Log("Collision Enter! " + Time.time.ToString());
//    //    if (info.collider.CompareTag("Player") && info.collider.GetComponent<//NetworkView>().isMine) {
//    //        Debug.Log("Set the Actor! " + Time.time.ToString());
//    //        actor = info.collider.transform;
//    //    }
//    //}

//    //void OnCollisionExit(Collision info){
//    //    Debug.Log("Collision exit. " + Time.time.ToString());
//    //    if(info.collider.CompareTag("Player") && info.collider.GetComponent<//NetworkView>().isMine){
//    //        Debug.Log("Unset actor. " + Time.time.ToString());
//    //        actor = null;
//    //    }
//    //}

//    //Transform actor;
//    //Vector3 oldPosition;
    
//    //void FixedUpdate() {
//    //    if (actor != null) {
//    //        Debug.Log("Moved the actor. " + Time.time.ToString());
//    //        Vector3 translation = transform.position - oldPosition;
//    //        //Vector3 translation = oldPosition - transform.position;
//    //        actor.transform.Translate(translation);
//    //    }
//    //    oldPosition = transform.position;
//    //}
//    void OnCollisionEnter(Collision info) {
//        Debug.Log("Collision Enter! " + Time.time.ToString());
//        if (info.collider.CompareTag("Player") && info.collider.GetComponent<//NetworkView>().isMine) {
            
//            actor = info.collider.transform;
//            actor.GetComponent<KeyboardInput>().canWalk = false;
//            actor.SetParent(transform);

//            localPos = actor.localPosition;
//        }
//    }

//    //void OnCollisionExit(Collision info) {
//    //    Debug.Log("Collision exit. " + Time.time.ToString());
//    //    if (info.collider.CompareTag("Player") && info.collider.GetComponent<//NetworkView>().isMine) {
//    //        FreeActor();
//    //    }
//    //}

//    private void FreeActor() {
//        actor.SetParent(null);
//        actor.GetComponent<KeyboardInput>().canWalk = true;
//        actor = null;
//    }

//    Transform actor;
//    Vector3 localPos;
//    Vector3 oldPosition;

//    void FixedUpdate() {
//        if (actor != null) {
//            if(InputConverter.GetKey(KeyBind.JetUp)){
//                FreeActor();
//            } else {
//                Debug.Log("Moved the actor. " + Time.time.ToString());
//                actor.localPosition = localPos;
//            }
            
//            //Vector3 translation = transform.position - oldPosition;
//            //actor.transform.Translate(translation);
//        }
//        oldPosition = transform.position;
//    }
//}
