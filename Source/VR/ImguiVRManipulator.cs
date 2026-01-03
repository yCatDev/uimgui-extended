using System;
using UnityEngine;

namespace UImGui.VR
{
    public class ImguiVRManipulator : MonoBehaviour
    {
        [SerializeField] private UImGui imGui;
        [SerializeField] private UnityEngine.Renderer tintSphere;
        [SerializeField] private Transform leftManipulatorTransform, rightManipulatorTransform;
        [SerializeField] private Transform cursor;
        [SerializeField] private float cursorScaleDownOnPress = 0.8f;
        [SerializeField] private VRRayVisualizer rayVisualizer;
        [SerializeField] private UnityEngine.Renderer[] manipulatorRenderers;


        public UnityEngine.Renderer TintSphere => tintSphere;
        public UnityEngine.Renderer[] Renderers => manipulatorRenderers;

        private void Awake()
        {
            foreach (var renderer in Renderers)
            {
                renderer.enabled = false;
            }
        }

        private void LateUpdate()
        {
            if (!imGui.enabled) return;

            var virtualXRInput = UImGuiUtility.VRContext.VirtualXRInput;
            var worldSpaceTransformer = UImGuiUtility.VRContext.WorldSpaceTransformer;

            tintSphere.transform.position = worldSpaceTransformer.Camera.transform.position;

            worldSpaceTransformer.GetControllerTransforms(
                virtualXRInput, HandCursorMode.Left,
                out var leftPositionWS,
                out var leftRotationWS);
            leftManipulatorTransform.SetPositionAndRotation(leftPositionWS, leftRotationWS);

            worldSpaceTransformer.GetControllerTransforms(
                virtualXRInput, HandCursorMode.Right,
                out var rightPositionWS,
                out var rightRotationWS);
            rightManipulatorTransform.SetPositionAndRotation(rightPositionWS, rightRotationWS);

            cursor.SetPositionAndRotation(worldSpaceTransformer.WorldSpaceCursorPosition,
                Quaternion.LookRotation(worldSpaceTransformer.SurfaceNormal));
            cursor.localScale = Vector3.Lerp(Vector3.one, Vector3.one * cursorScaleDownOnPress,
                virtualXRInput.PressButton.ReadValue<float>());

            var cursorPositionWS =
                virtualXRInput.HandCursorMode == HandCursorMode.Left ? leftPositionWS : rightPositionWS;
            var cursorRotationWS =
                virtualXRInput.HandCursorMode == HandCursorMode.Left ? leftRotationWS : rightRotationWS;
            rayVisualizer.UpdateRay(cursorPositionWS, cursorRotationWS, worldSpaceTransformer.WorldSpaceCursorPosition);
        }
    }
}