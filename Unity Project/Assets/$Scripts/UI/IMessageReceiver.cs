using UnityEngine;
using System.Collections;

public interface IMessageReceiver {

    void SubmitMessage(string message, bool clearBefore = false);
}
