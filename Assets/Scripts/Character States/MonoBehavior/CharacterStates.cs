using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStates : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    private AttackData_SO baseAttackData;
    private RuntimeAnimatorController baseAnimator;

    [Header("Weapon")]
    public Transform weaponSlot;

    [HideInInspector]
    public bool isCritical;
    
    void Awake()
    {
        if(templateData != null)
            characterData = Instantiate(templateData);

        baseAttackData = Instantiate(attackData);
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;
    }

    #region Read from Data_SO
    public int MaxHealth
    {
        get{ if(characterData != null) return characterData.maxHealth; else return 0;}
        set{ characterData.maxHealth = value;}
    }
        public int CurrentHealth
    {
        get{ if(characterData != null) return characterData.currentHealth; else return 0;}
        set{ characterData.currentHealth = value;}
    }
        public int BaseDefence
    {
        get{ if(characterData != null) return characterData.baseDefence; else return 0;}
        set{ characterData.baseDefence = value;}
    }
        public int currentDefence
    {
        get{ if(characterData != null) return characterData.currentDefence; else return 0;}
        set{ characterData.currentDefence = value;}
    }
    #endregion

    #region Character Combat

    public void TakeDamage(CharacterStates attacker, CharacterStates defender)
    {
        //伤害值理论上要大于零
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.currentDefence, 0);
        //保证血量最小为零
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if(attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }

        //Update UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //经验Update
        if(CurrentHealth <= 0)
            attacker.characterData.UpdateExp(characterData.killPoint);
    }

    public void TakeDamage(int damage, CharacterStates defender)
    {
        int currentDamage = Mathf.Max(damage - defender.currentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        if(CurrentHealth <= 0)
            GameManager.Instance.playerStates.characterData.UpdateExp(characterData.killPoint);
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        if(isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击！" + coreDamage);
        }

        return (int)coreDamage;
    }

    #endregion

    #region Equip Weapon
    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipWeapon();
        EquipWeapon(weapon);
    }
    public void EquipWeapon(ItemData_SO weapon)
    {
        if(weapon.weaponPrefab != null)
            Instantiate(weapon.weaponPrefab, weaponSlot);

        attackData.ApplyWeaponData(weapon.weaponData);
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;
    }

    public void UnEquipWeapon()
    {
        if(weaponSlot.transform.childCount != 0)
        {
            for(int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }
        attackData.ApplyWeaponData(baseAttackData);
        GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
    }
    #endregion

    #region Apply Data Change
    public void ApplyHealth(int amount)
    {
        if(CurrentHealth + amount <= MaxHealth)
        {
            CurrentHealth += amount;
        }
        else
        {
            CurrentHealth = MaxHealth;
        }
    }
    #endregion
}
