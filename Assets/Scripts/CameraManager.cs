using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera idleCam;
    [SerializeField] private CinemachineVirtualCamera followCam;

    private void Awake()
    {
        switchToIdleCam();
    }

    public void switchToIdleCam()
    {
        idleCam.enabled = true;
        followCam.enabled = false;
    }

    public void switchToFollowCam(Transform followtransform)
    {
        followCam.Follow = followtransform;
        idleCam.enabled = false;
        followCam.enabled = true;
    }
}
