using System;
using UnityEngine;

namespace UImGui.VR
{
    [Serializable]
    public class WorldSpaceTransformerConfig
    {
        public Transform trackingSpace;
        public Camera camera;
        
        public float initialDistanceFromCamera = 1.5f;
        public float minDistanceFromCamera = 1f;
        public float maxDistanceFromCamera = 3.5f;
        public float followingSpeed = 7f;
        public float lookAngleThresholdToFollow = 25f;
        public float distanceThresholdToFollow = 0.5f;
        public float pixelsPerUnit = 1000f;
    }
}