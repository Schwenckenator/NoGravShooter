using UnityEngine;
using System.Collections;

public class ProjectileOwnerName : MonoBehaviour {

    private NetworkPlayer projectileOwner;

    public NetworkPlayer ProjectileOwner {
        get { return projectileOwner; }
        set { 
            projectileOwner = value;
            //Debug.Log(value.ToString());
        }
    }
}
