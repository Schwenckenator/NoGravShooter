using UnityEngine;
using System.Collections;

public class ClientRunningServerCodeException : System.Exception {

    public ClientRunningServerCodeException() { }

    public ClientRunningServerCodeException(string message) : base (message) { }

}
