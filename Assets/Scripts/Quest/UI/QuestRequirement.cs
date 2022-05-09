using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuestRequirement : MonoBehaviour
{
    private Text requireName;
    private Text progressNumber;

    public Slider progressBar;

    void Awake()
    {
        requireName = GetComponent<Text>();
        progressNumber = transform.GetChild(1).GetComponent<Text>();
    }

    public void SetupRequirement(string name, int amount, int currentAmount)
    {
        requireName.text = name;
        progressNumber.text = currentAmount.ToString() + " / " + amount.ToString();
        progressBar.minValue = 0;
        progressBar.maxValue = amount;
        progressBar.value = currentAmount;
    }

    public void SetupRequirement(string name, bool isFinished)
    {
        if(isFinished)
        {
            requireName.text = name;
            progressNumber.text = "完成";
            progressBar.value = progressBar.maxValue;
        }
    }

}
