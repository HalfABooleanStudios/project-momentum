using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbsorbStat
{
    public DamageType damageType;
    public float playerPercent;
    public float armourPercent;
}

[CreateAssetMenu(fileName="New Armour Type", menuName="Armour Type")]
public class Armour : ScriptableObject
{
    public new string name;
    [Multiline(5)]
    public string description;
    public int durability;
    public int weight;
    public AbsorbStat[] absorbStats;

    public bool isBroken = false;

    public static Armour Instantiate(Armour original)
    {
        Armour newArmour = new()
        {
            name = original.name,
            description = original.description,
            durability = original.durability,
            absorbStats = original.absorbStats
        };
        return newArmour;
    }

    public AbsorbStat FindAbsorbStat(DamageType damageType)
    {
        foreach (AbsorbStat absorbStat in absorbStats)
        {
            if (absorbStat.damageType==damageType) return absorbStat;
        }
        return absorbStats[0];
    }

    public void Damage(int amount, DamageType damageType)
    {
        durability -= Mathf.RoundToInt(amount * FindAbsorbStat(damageType).armourPercent);
        if (durability <= 0) isBroken = true;
    }
}