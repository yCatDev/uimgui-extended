using UnityEngine;

namespace UImGui.VR
{
    public class ImguiVRManipulator : MonoBehaviour
    {
        [SerializeField] private Transform leftManipulatorTransform, rightManipulatorTransform;
        [SerializeField] private Transform cursor;
        [SerializeField] private LineRenderer cursorLineRenderer;

        private void LateUpdate()
        {
            var virtualXRInput = UImGuiUtility.VRContext.VirtualXRInput;
            var worldSpaceTransformer = UImGuiUtility.VRContext.WorldSpaceTransformer;

            worldSpaceTransformer.GetControllerTransforms(
                virtualXRInput.LeftControllerPosition.ReadValue<Vector3>(),
                virtualXRInput.LeftControllerRotation.ReadValue<Quaternion>(),
                out var leftPositionWS,
                out var leftRotationWS);
            leftManipulatorTransform.SetPositionAndRotation(leftPositionWS, leftRotationWS);
            
            worldSpaceTransformer.GetControllerTransforms(
                virtualXRInput.RightControllerPosition.ReadValue<Vector3>(),
                virtualXRInput.RightControllerRotation.ReadValue<Quaternion>(),
                out var rightPositionWS,
                out var rightRotationWS);
            rightManipulatorTransform.SetPositionAndRotation(rightPositionWS, rightRotationWS);

            cursor.position = worldSpaceTransformer.WorldSpaceCursorPosition;

            cursorLineRenderer.SetPosition(0,
                virtualXRInput.HandCursorMode == HandCursorMode.Left ? leftPositionWS : rightPositionWS);
            cursorLineRenderer.SetPosition(1, worldSpaceTransformer.WorldSpaceCursorPosition);
        }
    }
}