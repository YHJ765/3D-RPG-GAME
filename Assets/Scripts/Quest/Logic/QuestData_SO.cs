using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data") ]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    public class QuestRequire
    {
        public string name;
        public int requireAmount;
        public int currentAmount;
    }

    public string questName;
    [TextArea]
    public string description;
    
    public bool isStarted;
    public bool isComplete;
    public bool isFinished;

    public List<QuestRequire> questRequires = new List<QuestRequire>();
    public List<InventoryItem> rewards = new List<InventoryItem>();

    public void CheckQuestProgress()
    {
        var finishRequires = questRequires.Where(r => r.requireAmount <= r.currentAmount);
        isComplete = finishRequires.Count() == questRequires.Count();

        if(isComplete)
        {
            Debug.Log("任务完成");
        }
    }

    public void GiveRewards()
    {
        foreach(var reward in rewards)
        {
            if(reward.amount < 0)
            {
                int requireCount = Mathf.Abs(reward.amount);

                //此处为任务物品交付功能
                //背包中任务物品不为空
                if(InventoryManager.Instance.QuestItemInBag(reward.itemData) != null)
                {
                    //背包中的任务物品不够交付
                    if(InventoryManager.Instance.QuestItemInBag(reward.itemData).amount <= requireCount)
                    {
                        requireCount -= InventoryManager.Instance.QuestItemInBag(reward.itemData).amount;
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount = 0;
                        
                        //剩余数量扣取快捷栏中
                        if(InventoryManager.Instance.QuestItemInAction(reward.itemData) != null)
                            InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                    }
                    //背包中的物品足够交付
                    else
                    {
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount -= requireCount;
                    }
                }
                //扣除快捷栏上的任务物品
                else
                {
                    InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                }
            }
            //获得奖励
            else
            {
                InventoryManager.Instance.inventoryData.AddItem(reward.itemData, reward.amount);
            }

            InventoryManager.Instance.inventoryUI.RefreshUI();
            InventoryManager.Instance.actionUI.RefreshUI();
        }
    }

    //当前任务需要 收集/消灭 的目标列表
    public List<string> RequireTargetName()
    {
        List<string> targetNameList = new List<string>();

        foreach(var require in questRequires)
        {
            targetNameList.Add(require.name);
        }
        return targetNameList;
    }
}
