using UnityEngine;
using UnityEngine.InputSystem;

namespace UImGui.VR
{
    public class VirtualXRInput
    {
        private readonly InputActionAsset _inputActionsAsset;
        private readonly HandCursorMode _handCursorMode;

        private InputActionMap _leftControllerMap;
        private InputActionMap _leftInteractionsMap;
        private InputActionMap _rightControllerMap;
        private InputActionMap _rightInteractionsMap;

        private InputAction _leftControllerPosition;
        private InputAction _leftControllerRotation;
        private InputAction _rightControllerPosition;
        private InputAction _rightControllerRotation;
        private InputAction _leftControllerPress;
        private InputAction _leftControllerGrab;
        private InputAction _leftControllerThumbstick;
        private InputAction _leftControllerPrimaryButton;
        private InputAction _leftControllerSecondaryButton;
        private InputAction _rightControllerPress;
        private InputAction _rightControllerGrab;
        private InputAction _rightControllerThumbstick;
        private InputAction _rightControllerPrimaryButton;
        private InputAction _rightControllerSecondaryButton;

        public InputAction LeftControllerPosition => _leftControllerPosition;
        public InputAction LeftControllerRotation => _leftControllerRotation;
        public InputAction RightControllerPosition => _rightControllerPosition;
        public InputAction RightControllerRotation => _rightControllerRotation;
        public InputAction LeftControllerPress => _leftControllerPress;
        public InputAction LeftControllerGrab => _leftControllerGrab;
        public InputAction LeftControllerThumbstick => _leftControllerThumbstick;
        public InputAction LeftControllerPrimaryButton => _leftControllerPrimaryButton;
        public InputAction LeftControllerSecondaryButton => _leftControllerSecondaryButton;
        public InputAction RightControllerPress => _rightControllerPress;
        public InputAction RightControllerGrab => _rightControllerGrab;
        public InputAction RightControllerThumbstick => _rightControllerThumbstick;
        public InputAction RightControllerPrimaryButton => _rightControllerPrimaryButton;
        public InputAction RightControllerSecondaryButton => _rightControllerSecondaryButton;

        public InputAction CursorPosition { get; private set; }
        public InputAction CursorRotation { get; private set; }
        public InputAction PrimaryButton { get; private set; }
        public InputAction SecondaryButton { get; private set; }
        public InputAction Thumbstick { get; private set; }
        public InputAction Scroll { get; private set; }
        public InputAction PressButton { get; private set; }
        public InputAction SecondaryPressButton { get; private set; }

        public HandCursorMode HandCursorMode => _handCursorMode;

        public VirtualXRInput(InputActionAsset inputActionAsset, HandCursorMode handCursorMode)
        {
            _handCursorMode = handCursorMode;
            _inputActionsAsset = inputActionAsset;

            InitializeInputActions();
        }

        private void InitializeInputActions()
        {
            _leftControllerMap = _inputActionsAsset.FindActionMap("XRI Left");
            _leftInteractionsMap = _inputActionsAsset.FindActionMap("XRI Left Interaction");
            _rightControllerMap = _inputActionsAsset.FindActionMap("XRI Right");
            _rightInteractionsMap = _inputActionsAsset.FindActionMap("XRI Right Interaction");

            _leftControllerPosition = _leftControllerMap.FindAction("Position");
            _leftControllerRotation = _leftControllerMap.FindAction("Rotation");

            _rightControllerPosition = _rightControllerMap.FindAction("Position");
            _rightControllerRotation = _rightControllerMap.FindAction("Rotation");

            _leftControllerPress = _leftInteractionsMap.FindAction("Activate");
            _leftControllerGrab = _leftInteractionsMap.FindAction("Select");
            _leftControllerThumbstick = _leftControllerMap.FindAction("Thumbstick");
            _leftControllerPrimaryButton = _leftInteractionsMap.FindAction("Primary Button");
            _leftControllerSecondaryButton = _leftInteractionsMap.FindAction("Secondary Button");

            _rightControllerPress = _rightInteractionsMap.FindAction("Activate");
            _rightControllerGrab = _rightInteractionsMap.FindAction("Select");
            _rightControllerThumbstick = _rightControllerMap.FindAction("Thumbstick");
            _rightControllerPrimaryButton = _rightInteractionsMap.FindAction("Primary Button");
            _rightControllerSecondaryButton = _rightInteractionsMap.FindAction("Secondary Button");

            _leftControllerPosition.Enable();
            _leftControllerRotation.Enable();
            _leftControllerPress.Enable();
            _leftControllerGrab.Enable();
            _leftControllerThumbstick.Enable();
            _leftControllerPrimaryButton.Enable();
            _leftControllerSecondaryButton.Enable();

            _rightControllerPosition.Enable();
            _rightControllerRotation.Enable();
            _rightControllerPress.Enable();
            _rightControllerGrab.Enable();
            _rightControllerThumbstick.Enable();
            _rightControllerPrimaryButton.Enable();
            _rightControllerSecondaryButton.Enable();

            switch (_handCursorMode)
            {
                case HandCursorMode.None:
                case HandCursorMode.Right:
                    CursorPosition = RightControllerPosition;
                    CursorRotation = RightControllerRotation;
                    
                    PrimaryButton = LeftControllerPrimaryButton;
                    SecondaryButton = LeftControllerSecondaryButton;
                    Thumbstick = LeftControllerThumbstick;
                    
                    Scroll = RightControllerThumbstick;
                    PressButton = RightControllerPress;
                    SecondaryPressButton = RightControllerGrab;
                    break;
                case HandCursorMode.Left:
                    CursorPosition = LeftControllerPosition;
                    CursorRotation = LeftControllerRotation;
                    
                    PrimaryButton = RightControllerPrimaryButton;
                    SecondaryButton = RightControllerSecondaryButton;
                    Thumbstick = RightControllerThumbstick;
                    
                    Scroll = LeftControllerThumbstick;
                    PressButton = LeftControllerPress;
                    SecondaryPressButton = LeftControllerGrab;
                    break;
            }
        }
    }
}