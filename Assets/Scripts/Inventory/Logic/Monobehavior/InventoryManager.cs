using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    //TODO:最后添加模版保存
    [Header("Inventory Data")]    
    public InventoryData_SO inventoryData;

    [Header("ContainerS")]
    public ContainerUI inventoryUI;

    void Start() 
    {
        inventoryUI.RefreshUI();    
    }
}
