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
    void update()
    {
        progressBar.value = 50;
    }
}
