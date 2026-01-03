using ImGuiNET;
using System;
using System.Collections.Generic;
using UImGui.Assets;
using UImGui.VR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace UImGui.Platform
{
    /// <summary>
    /// Platform bindings for ImGui in Unity for VR based on Input System`s setup 
    /// </summary>
    internal sealed class VRPlatform : PlatformBase
    {
        private readonly List<char> _textInput = new();

        private readonly List<KeyControl> _keyControls = new();

        private Keyboard _keyboard;

        public VRPlatform(CursorShapesAsset cursorShapes, IniSettingsAsset iniSettings)
            : base(cursorShapes, iniSettings)
        {
        }

        private static void UpdateMouse(ImGuiIOPtr io, VirtualXRInput virtualXRInput,
            WorldSpaceTransformer worldSpaceTransformer)
        {
            var mouseScreenPosition = worldSpaceTransformer.GetCursorPosition(virtualXRInput);
            io.MousePos = Utils.ScreenToImGui(mouseScreenPosition);

            var mouseScroll = virtualXRInput.Scroll.ReadValue<Vector2>();
            io.MouseWheel = mouseScroll.y;
            io.MouseWheelH = mouseScroll.x;

            io.MouseDown[0] = virtualXRInput.PressButton.IsPressed();
            io.MouseDown[1] = virtualXRInput.SecondaryPressButton.IsPressed();
            io.MouseDown[2] = false; // TODO: Middle scroll button, maybe we need this...
        }

        private static void UpdateXRControllers(ImGuiIOPtr io, VirtualXRInput virtualXRInput)
        {
            io.BackendFlags |= ImGuiBackendFlags.HasGamepad;
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;

            io.AddKeyAnalogEvent(ImGuiKey.GamepadFaceDown, virtualXRInput.PrimaryButton.IsPressed(),
                virtualXRInput.PrimaryButton.ReadValue<float>()); 
            io.AddKeyAnalogEvent(ImGuiKey.GamepadFaceRight, virtualXRInput.SecondaryButton.IsPressed(),
                virtualXRInput.SecondaryButton.ReadValue<float>());

            var thumbstickVector = virtualXRInput.Thumbstick.ReadValue<Vector2>();
            io.AddKeyAnalogEvent(ImGuiKey.GamepadDpadUp, thumbstickVector.y > 0.5f, thumbstickVector.y);
            io.AddKeyAnalogEvent(ImGuiKey.GamepadDpadDown, thumbstickVector.y < -0.5f, -thumbstickVector.y);
            io.AddKeyAnalogEvent(ImGuiKey.GamepadDpadLeft, thumbstickVector.x < -0.5f, -thumbstickVector.x);
            io.AddKeyAnalogEvent(ImGuiKey.GamepadDpadRight, thumbstickVector.x > 0.5f, thumbstickVector.x);
        }

        private void SetupKeyboard(Keyboard keyboard)
        {
            if (_keyboard != null)
            {
                _keyboard.onTextInput -= _textInput.Add;
            }

            _keyboard = keyboard;
            _keyControls.Clear();

            // Map and store new keys by assigning io.KeyMap and setting value of array.
            _keyboard.onTextInput += _textInput.Add;
        }

        private void UpdateKeyboard(ImGuiIOPtr io, Keyboard keyboard)
        {
            if (keyboard == null)
            {
                return;
            }

            // BUG: mod key make everything slow. Go to line
            for (int keyIndex = 0; keyIndex < Keyboard.KeyCount; keyIndex++)
            {
                Key key = (Key)keyIndex;
                if (TryMapKeys(key, out ImGuiKey imguikey))
                {
                    KeyControl keyControl = keyboard[key];
                    io.AddKeyEvent(imguikey, keyControl.IsPressed());
                }
            }

            io.KeyShift = keyboard[Key.LeftShift].isPressed || keyboard[Key.RightShift].isPressed;
            io.KeyCtrl = keyboard[Key.LeftCtrl].isPressed || keyboard[Key.RightCtrl].isPressed;
            io.KeyAlt = keyboard[Key.LeftAlt].isPressed || keyboard[Key.RightAlt].isPressed;
            io.KeySuper = keyboard[Key.LeftMeta].isPressed || keyboard[Key.RightMeta].isPressed;

            // Text input.
            for (int i = 0, iMax = _textInput.Count; i < iMax; ++i)
            {
                io.AddInputCharacter(_textInput[i]);
            }

            _textInput.Clear();
        }

        private bool TryMapKeys(Key key, out ImGuiKey imguikey)
        {
            static ImGuiKey KeyToImGuiKeyShortcut(Key keyToConvert, Key startKey1, ImGuiKey startKey2)
            {
                int changeFromStart1 = (int)keyToConvert - (int)startKey1;
                return startKey2 + changeFromStart1;
            }

            imguikey = key switch
            {
                >= Key.F1 and <= Key.F12 => KeyToImGuiKeyShortcut(key, Key.F1, ImGuiKey.F1),
                >= Key.Numpad0 and <= Key.Numpad9 => KeyToImGuiKeyShortcut(key, Key.Numpad0, ImGuiKey.Keypad0),
                >= Key.A and <= Key.Z => KeyToImGuiKeyShortcut(key, Key.A, ImGuiKey.A),
                >= Key.Digit1 and <= Key.Digit9 => KeyToImGuiKeyShortcut(key, Key.Digit1, ImGuiKey._1),
                Key.Digit0 => ImGuiKey._0,
                // BUG: mod keys make everything slow. 
                // Key.LeftShift or Key.RightShift => ImGuiKey.ModShift,
                // Key.LeftCtrl or Key.RightCtrl => ImGuiKey.ModCtrl,
                // Key.LeftAlt or Key.RightAlt => ImGuiKey.ModAlt,
                Key.LeftWindows or Key.RightWindows => ImGuiKey.ModSuper,
                Key.ContextMenu => ImGuiKey.Menu,
                Key.UpArrow => ImGuiKey.UpArrow,
                Key.DownArrow => ImGuiKey.DownArrow,
                Key.LeftArrow => ImGuiKey.LeftArrow,
                Key.RightArrow => ImGuiKey.RightArrow,
                Key.Enter => ImGuiKey.Enter,
                Key.Escape => ImGuiKey.Escape,
                Key.Space => ImGuiKey.Space,
                Key.Tab => ImGuiKey.Tab,
                Key.Backspace => ImGuiKey.Backspace,
                Key.Insert => ImGuiKey.Insert,
                Key.Delete => ImGuiKey.Delete,
                Key.PageUp => ImGuiKey.PageUp,
                Key.PageDown => ImGuiKey.PageDown,
                Key.Home => ImGuiKey.Home,
                Key.End => ImGuiKey.End,
                Key.CapsLock => ImGuiKey.CapsLock,
                Key.ScrollLock => ImGuiKey.ScrollLock,
                Key.PrintScreen => ImGuiKey.PrintScreen,
                Key.Pause => ImGuiKey.Pause,
                Key.NumLock => ImGuiKey.NumLock,
                Key.NumpadDivide => ImGuiKey.KeypadDivide,
                Key.NumpadMultiply => ImGuiKey.KeypadMultiply,
                Key.NumpadMinus => ImGuiKey.KeypadSubtract,
                Key.NumpadPlus => ImGuiKey.KeypadAdd,
                Key.NumpadPeriod => ImGuiKey.KeypadDecimal,
                Key.NumpadEnter => ImGuiKey.KeypadEnter,
                Key.NumpadEquals => ImGuiKey.KeypadEqual,
                Key.Backquote => ImGuiKey.GraveAccent,
                Key.Minus => ImGuiKey.Minus,
                Key.Equals => ImGuiKey.Equal,
                Key.LeftBracket => ImGuiKey.LeftBracket,
                Key.RightBracket => ImGuiKey.RightBracket,
                Key.Semicolon => ImGuiKey.Semicolon,
                Key.Quote => ImGuiKey.Apostrophe,
                Key.Comma => ImGuiKey.Comma,
                Key.Period => ImGuiKey.Period,
                Key.Slash => ImGuiKey.Slash,
                Key.Backslash => ImGuiKey.Backslash,
                _ => ImGuiKey.None
            };

            return imguikey != ImGuiKey.None;
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (device is Keyboard keyboard)
            {
                // Keyboard layout change, remap main keys.
                if (change == InputDeviceChange.ConfigurationChanged)
                {
                    SetupKeyboard(keyboard);
                }

                // Keyboard device changed, setup again.
                if (Keyboard.current != _keyboard)
                {
                    SetupKeyboard(Keyboard.current);
                }
            }
        }

        #region Overrides of PlatformBase

        public override bool Initialize(ImGuiIOPtr io, UIOConfig config, string platformName)
        {
            InputSystem.onDeviceChange += OnDeviceChange;
            base.Initialize(io, config, platformName);

            io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;

            unsafe
            {
                PlatformCallbacks.SetClipboardFunctions(PlatformCallbacks.GetClipboardTextCallback,
                    PlatformCallbacks.SetClipboardTextCallback);
            }

            SetupKeyboard(Keyboard.current);

            return true;
        }

        public override void Shutdown(ImGuiIOPtr io)
        {
            base.Shutdown(io);
            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        public override void PrepareFrame(ImGuiIOPtr io, Rect displayRect)
        {
            base.PrepareFrame(io, displayRect);

            //UpdateKeyboard(io, Keyboard.current);
            UpdateMouse(io, UImGuiUtility.VRContext.VirtualXRInput, UImGuiUtility.VRContext.WorldSpaceTransformer);
            UpdateCursor(io, ImGui.GetMouseCursor());
            UpdateXRControllers(io, UImGuiUtility.VRContext.VirtualXRInput);
        }

        #endregion
    }
}