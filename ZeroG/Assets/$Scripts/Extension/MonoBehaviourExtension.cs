using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class MonoBehaviourExtension {

    public static T GetInterface<T>(this GameObject gameobject) where T : class {
        if (!typeof(T).IsInterface) {
            Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
            return null;
        }
        return gameobject.GetComponent(typeof(T)) as T;
    }

    public static T[] GetInterfaces<T>(this GameObject gameobject) where T : class {
        if(!typeof(T).IsInterface){
            Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
            return null;
        }

        Component[] components = gameobject.GetComponents(typeof(MonoBehaviour));
        return GetInterfacesInComponents<T>(components);
    }
    public static T GetInterfaceInChildren<T>(this GameObject gameobject) where T : class {
        if (!typeof(T).IsInterface) {
            Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
            return null;
        }

        return gameobject.GetComponentInChildren(typeof(T)) as T;
    }
    public static T[] GetInterfacesInChildren<T>(this GameObject gameobject) where T : class {
        if (!typeof(T).IsInterface) {
            Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
            return null;
        }
        Component[] components = gameobject.GetComponentsInChildren(typeof(MonoBehaviour));
        return GetInterfacesInComponents<T>(components);
    }

    #region helper methods
    private static T GetInterfaceInComponents<T>(Component[] components) where T : class {
        foreach (Component com in components) {
            if (com is T) {
                return com as T;
            }
        }
        return null;
    }

    private static T[] GetInterfacesInComponents<T>(Component[] components) where T : class {
        List<T> foundInterfaces = new List<T>();
        foreach (Component com in components) {
            T convertedComponent = com as T;
            if (convertedComponent != null) {
                foundInterfaces.Add(convertedComponent);
            }
        }

        return foundInterfaces.ToArray();
    }

    #endregion
}

