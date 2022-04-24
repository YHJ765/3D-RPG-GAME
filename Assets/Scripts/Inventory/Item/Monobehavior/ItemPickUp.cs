using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;
    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI();
            //装备武器
            // GameManager.Instance.playerStates.EquipWeapon(itemData);
            Destroy(gameObject);
        }    
    }
}
