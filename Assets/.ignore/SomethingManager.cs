using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomethingManager : MonoBehaviour
{
    private static SomethingManager instance;
    public static SomethingManager Instance { get {
        if (instance!=null) { return instance; }
        SomethingManager kbmObject = FindObjectOfType<SomethingManager>();
        if (kbmObject!=null) { return kbmObject; }
        GameObject go = new();
        go.AddComponent<SomethingManager>();
        return go.GetComponent<SomethingManager>();
    } }

    void Awake()
    {
        if (instance == null) { instance = this; }
        if (instance != this) { Destroy(gameObject); }
    }
}
