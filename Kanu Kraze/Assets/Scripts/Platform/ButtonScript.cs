﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public BasePlatformScript RelatedPlatform;
    public GameObject Instruction;

    public CameraState ZoomAdjust = CameraState.Inactive;

    private const float CAMERA_ZOOM = 6.12F;
    private float _waitState = 0f;

    public bool Pressed = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (ZoomAdjust != CameraState.Inactive)
        {
            if (ZoomAdjust == CameraState.ZoomingOut)
            {
                Camera.main.orthographicSize += 0.1f;
                if (Camera.main.orthographicSize > 12)
                {
                    ZoomAdjust = CameraState.Waiting;
                }
            }
            else if (ZoomAdjust == CameraState.Waiting)
            {
                _waitState += Time.deltaTime;
                if (_waitState > 2.5f)
                {
                    ZoomAdjust = CameraState.ZoomingIn;
                    _waitState = 0;
                }
            }
            else if (ZoomAdjust == CameraState.ZoomingIn)
            {
                Camera.main.orthographicSize -= 0.1f;
                if (Camera.main.orthographicSize <= CAMERA_ZOOM)
                {
                    Camera.main.orthographicSize = CAMERA_ZOOM;
                    ZoomAdjust = CameraState.Inactive;
                }
            }
        }
    }

    internal void Press()
    {
        RelatedPlatform.Activate();
        ZoomAdjust = CameraState.ZoomingOut;
        Instruction.SetActive(false);
    }
}

public enum CameraState
{
    Inactive,
    ZoomingOut,
    Waiting,
    ZoomingIn
}
