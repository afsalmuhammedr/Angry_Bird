using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class SlingShotArea : MonoBehaviour
{
    [SerializeField] private LayerMask theRange;
    public bool IsWithinArea()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.mousePosition);

        if (Physics2D.OverlapPoint(touchPosition, theRange)){
            return true;
        }
        else
        {
            return false;
        }
    }
}
