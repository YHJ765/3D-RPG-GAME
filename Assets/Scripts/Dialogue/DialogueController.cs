using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentData;
    bool canTalk = false;

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") && currentData != null)
        {
            canTalk = true;
        }    
    }

    void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
        }    
    }

    void Update()
    {
        if(canTalk && Input.GetMouseButtonDown(1))
        {
            OpenDialogue();
        }
    }

    void OpenDialogue()
    {
        //打开UI面板
        //传输对话内容信息
        DialogueUI.Instance.UpdateDialogueData(currentData);
        DialogueUI.Instance.UpdateMainDialogue(currentData.dialoguePieces[0]);
    }
}
