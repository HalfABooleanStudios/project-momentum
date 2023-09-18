using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New KeyBind", menuName="KeyBind")]
public class KeyBind : ScriptableObject
{
    public new string name;
    public KeyCode keyCode;
}
