using System;
using UnityEngine.InputSystem;

namespace UImGui.VR
{
    [Serializable]
    public class VRConfiguration
    {
        public InputActionAsset vrInputAsset;
        public HandCursorMode handCursorMode = HandCursorMode.Right;
        public WorldSpaceTransformerConfig worldSpaceConfig;
    }
}