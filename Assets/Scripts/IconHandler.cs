using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconHandler : MonoBehaviour
{
    [SerializeField] private Image[] icons;
    [SerializeField] private Color usedColor;

    public void UseShot(int usedBirds)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            if(usedBirds == i + 1)
            {
                icons[i].color = usedColor;
                return;
            }
        }
    }

}
