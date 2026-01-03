using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UImGui.VR
{
    [Serializable]
    public class VRConfiguration
    {
        public InputActionAsset vrInputAsset;
        public HandCursorMode handCursorMode = HandCursorMode.Right;
        public ImguiVRManipulator vrManipulator;
        public WorldSpaceTransformerConfig worldSpaceConfig;
    }
}