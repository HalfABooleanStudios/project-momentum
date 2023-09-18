using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Agent agent;
    public ResetableVar<float> mass;

    // Start is called before the first frame update
    void Awake()
    {
        mass = new(agent.mass);
    }

    public void Kill() {  }
}
