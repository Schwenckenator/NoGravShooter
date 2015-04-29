using UnityEngine;
using System.Collections;

public interface IChangeable {
    // "C" Is changeable
    string type { get; }
    bool IsType(string otherType);

}
