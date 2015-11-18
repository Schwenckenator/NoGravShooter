using UnityEngine;
using System.Reflection;

[assembly:AssemblyVersion ("0.1.*")]
public class VersionNumber : MonoBehaviour{
    public bool ShowVersionInformation = true;

    Rect position = new Rect (0, 0, 100, 20);
    
    private string version;
    public string Version {
        get {
            if (version == null) {
                version = Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
            }
            return version;
        }
    }

    void Start (){
	    // Log current version in log file
        Debug.Log (string.Format ("Currently running version is {0}", Version));
	    position.y = 1f;
        position.x = Screen.width - position.width - 10f;

    }

    void OnGUI (){
        if (!ShowVersionInformation) return;
    
        GUI.contentColor = Color.white;
        GUI.Label (position, string.Format ("v{0}", Version));
    }
}