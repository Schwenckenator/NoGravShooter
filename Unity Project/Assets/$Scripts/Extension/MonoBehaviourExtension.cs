using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class MonoBehaviourExtension {

    public static T GetInterface<T>(this GameObject inObj) where T : class {
        if (!typeof(T).IsInterface) {
            Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
            return null;
        }
        return inObj.GetComponent(typeof(T)) as T;
    }
}

