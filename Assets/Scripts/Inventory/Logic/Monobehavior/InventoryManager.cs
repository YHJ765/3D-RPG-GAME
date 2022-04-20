using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }

    //TODO:最后添加模版保存
    [Header("Inventory Data")]    
    public InventoryData_SO inventoryData;
    public InventoryData_SO actionData;
    public InventoryData_SO equipmentData;

    [Header("ContainerS")]
    public ContainerUI inventoryUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    [Header("Drag Canvas")]
    public Canvas dragCanvas;
    public DragData currentDrag;

    void Start() 
    {
        inventoryUI.RefreshUI(); 
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();   
    }

    #region 检查拖拽物体是否在每一个Slot范围内
    public bool CheckInInventoryUI(Vector3 position)
    {
        for(int i = 0; i < inventoryUI.slotHolders.Length; i++)
        {
            RectTransform t = inventoryUI.slotHolders[i].transform as RectTransform; //类型转换

            if(RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }

        }
        return false;
    }

    public bool CheckInActionUI(Vector3 position)
    {
        for(int i = 0; i < actionUI.slotHolders.Length; i++)
        {
            RectTransform t = actionUI.slotHolders[i].transform as RectTransform; //类型转换

            if(RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }

        }
        return false;
    }

    public bool CheckInEquipmentUI(Vector3 position)
    {
        for(int i = 0; i < equipmentUI.slotHolders.Length; i++)
        {
            RectTransform t = equipmentUI.slotHolders[i].transform as RectTransform; //类型转换

            if(RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }

        }
        return false;
    }

    #endregion
}
