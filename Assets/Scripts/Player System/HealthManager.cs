using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {
    public int health, maxHealth;
    public Armour activeArmour;

    public void AddHealth(int amount) {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }
    
    public void RemoveHealth(int amount) {
        health -= amount;
        if (health <= 0) gameObject.GetComponent<PlayerManager>().Kill();
    }

    public void RemoveHealth(float amount) { RemoveHealth(Mathf.RoundToInt(amount)); }
    
    public void Heal(int amount) { AddHealth(amount); }
    public void Damage(int amount, DamageType damageType) {
        RemoveHealth(amount * activeArmour.FindAbsorbStat(damageType).playerPercent);
        activeArmour.Damage(amount, damageType);
    }
}
