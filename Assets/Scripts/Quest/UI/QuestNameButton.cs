using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestNameButton : MonoBehaviour
{
    public Text questNameText;
    public QuestData_SO currentData;
    public Text questContentText;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(UpdateQuestContent);
    }

    void UpdateQuestContent()
    {
        questContentText.text = currentData.description;
        QuestUI.Instance.SetupRequireList(currentData);
    }

    public void SetupNameButton(QuestData_SO questData)
    {
        currentData = questData;

        if(currentData.isComplete)
            questNameText.text = questData.questName + "(完成)";
        else
            questNameText.text = questData.questName;
    }
}
