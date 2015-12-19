using UnityEngine;

public class Logger {

    private bool isDebug;

    public Logger(bool isDebug) {
        this.isDebug = isDebug;
    }

    public void Log(object message, Object context = null) {
        if (isDebug) {
            Debug.Log(message, context);
        }
    }
    public void Exception(System.Exception exception, Object context = null) {
        Debug.LogException(exception, context);
    }
    public void Error(object message, Object context = null) {
        Debug.LogError(message, context);
    }
}
