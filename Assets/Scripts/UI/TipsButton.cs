using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsButton : MonoBehaviour
{
    public GameObject TipsImage;
    public bool isActive = false;

    public void ClickTipsButtton()
    {
        if(isActive)
        {
            TipsImage.SetActive(true);
            isActive = false;
        }
        else
        {
            TipsImage.SetActive(false);
            isActive = true;
        }

    }
}
