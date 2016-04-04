using UnityEngine;
using System.Collections;

public interface IOwnable {

    LobbyPlayer owner { get; set; }
}
