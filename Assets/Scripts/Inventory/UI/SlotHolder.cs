using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType {BAG, WEAPON, ARMOR, ACTION}
public class SlotHolder : MonoBehaviour
{
    public SlotType slotType;
    public ItemUI itemUI;

    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
                itemUI.Bag = InventoryManager.Instance.inventoryData;
                break;
            case SlotType.WEAPON:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                //装备武器 切换武器
                if(itemUI.Bag.items[itemUI.Index].itemData != null)
                {
                    GameManager.Instance.playerStates.ChangeWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    //如果武器耐久度不足则卸载装备（尚未实现，等以后需要再开发）
                    GameManager.Instance.playerStates.UnEquipWeapon();
                }
                break;
            case SlotType.ARMOR:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                break;
            case SlotType.ACTION:
                itemUI.Bag = InventoryManager.Instance.actionData;
                break;

        }

        var item = itemUI.Bag.items[itemUI.Index];
        itemUI.SetupItemUI(item.itemData, item.amount);
    }
}
