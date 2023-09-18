using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovementInfo
{
    [Header("Movement")]
    public float peakMomentum;
    public float runForce;
    public float haltForce;
    public float wallrunStuckForce;
    [Header("Jumping")]
    public float jumpMomentum;
    public float jumpTime;
    public int jumpCount;
    [Header("Variances")]
    public float sprintBoost;
    public float sneakSpeed;
    public float wallrunJumpMultiplier;
}

[CreateAssetMenu(fileName="New Agent", menuName="Agent")]
public class Agent : ScriptableObject
{
    public new string name;
    [Multiline(5)] public string description;
    public float mass;
    public MovementInfo movementInfo;
    public Armour loadoutArmour;
}