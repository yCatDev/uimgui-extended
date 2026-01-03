using System;
using UnityEngine;

namespace UImGui.VR
{
    [Serializable]
    public class WorldSpaceTransformerConfig
    {
        public ControllerInputMode controllerInputMode = ControllerInputMode.CalculateFromInputSystem;
        
        public Transform rightControllerMirrorTransform;
        public Transform leftControllerMirrorTransform;
        
        public Transform trackingSpace;
        public Camera camera;
        
        public float cursorSmoothTime = 0.1f;
        public float minDistanceFromCamera = 1f;
        public float maxDistanceFromCamera = 3.5f;
        public float followingSpeed = 7f;
        public float lookAngleThresholdToFollow = 25f;
        public float pixelsPerUnit = 1000f;
    }
}