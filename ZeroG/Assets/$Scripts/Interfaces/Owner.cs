using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public interface IOwnable {

    NetworkIdentity owner { get; set; }
}
