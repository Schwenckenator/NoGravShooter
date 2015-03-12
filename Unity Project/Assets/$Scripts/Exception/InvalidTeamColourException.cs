using UnityEngine;
using System.Collections;

public class InvalidTeamColourException : System.Exception {

    public InvalidTeamColourException() { }

    public InvalidTeamColourException(string message) : base(message) { }

}
