using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindManager : MonoBehaviour
{
    private static KeyBindManager instance;
    public static KeyBindManager Instance { get {
        if (instance!=null) { return instance; }
        KeyBindManager kbmObject = FindObjectOfType<KeyBindManager>();
        if (kbmObject!=null) { return kbmObject; }
        GameObject go = new();
        go.AddComponent<KeyBindManager>();
        return go.GetComponent<KeyBindManager>();
    } }

    public Dictionary<string, KeyCode> keyBinds;

    void Awake()
    {
        if (instance == null) { instance = this; }
        if (instance != this) { Destroy(gameObject); }

        LoadKeyBinds();
    }

    void LoadKeyBinds()
    {
        keyBinds = new Dictionary<string, KeyCode>();
        KeyBind[] loadedKeyBinds = Resources.LoadAll<KeyBind>("KeyBinds");
        foreach (KeyBind loadedKeyBind in loadedKeyBinds)
        {
            keyBinds.Add(loadedKeyBind.name, loadedKeyBind.keyCode);
        }
        Debug.Log(keyBinds.Count);
    }
}
