using System.Numerics;
using ImGuiNET;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace UImGui.VR
{
    public class WorldSpaceTransformer
    {
        private readonly WorldSpaceTransformerConfig _config;
        private readonly GameObject _virtualObject;
        
        private float _followingT;
        private Vector3 _virtualPosition;
        private Vector3 _startDirection;
        private Vector3 _targetDirection;
        private Vector2 _lastCursorPosition = Vector2.zero;
        private Vector3 _virtualCameraDirection;
        private bool _needToFollowRotation;
        private Vector3 _lastWorldSpacePointPosition;
        
        public float DistanceToCamera;
        
        public Matrix4x4 LocalToWorldMatrix { get; private set; }
        public Matrix4x4 WorldToLocalMatrix { get; private set; }
        public Vector3 WorldSpaceCursorPosition => _lastWorldSpacePointPosition;
        public Vector3 SurfaceNormal => -_virtualCameraDirection;
        public Camera Camera => _config.camera;


        public WorldSpaceTransformer(WorldSpaceTransformerConfig config)
        {
            _config = config;

            _virtualObject = new GameObject("IMGUI Virtual Camera");
            
            float minDistance = Screen.height / (2f * Mathf.Tan(Mathf.Deg2Rad * _config.camera.fieldOfView / 2f));
            
            DistanceToCamera = minDistance / config.pixelsPerUnit;

            _virtualCameraDirection = config.camera.transform.forward;
            _virtualPosition = config.camera.transform.position;
            
            var vPosition = _virtualPosition + _virtualCameraDirection * DistanceToCamera;
            var vRotation = config.camera.transform.rotation;
            
            _virtualObject.transform.SetPositionAndRotation(vPosition, vRotation);
        }

        public void Update()
        {
            UpdateVirtualCameraTransform();
            UpdateLocalToWorldMatrix();
        }

        private void UpdateLocalToWorldMatrix()
        {
            var virtualCameraPosition = _virtualObject.transform.position;
            var virtualCameraRotation = _virtualObject.transform.rotation;

            var drawData = ImGui.GetDrawData(); //TODO: This looks bad, maybe we can safely use Screen class for getting display size 
            
            float w = drawData.DisplaySize.x;
            float h = drawData.DisplaySize.y;
            
            
            float scale = 1f / _config.pixelsPerUnit;
            
            Vector3 offset = virtualCameraRotation * new Vector3(-w * 0.5f * scale, h * 0.5f * scale, 0);

            LocalToWorldMatrix = Matrix4x4.TRS(
                virtualCameraPosition + offset,
                virtualCameraRotation,
                new Vector3(scale, -scale, scale)
            );

            WorldToLocalMatrix = LocalToWorldMatrix.inverse;
        }

        private float EaseOutCubic(float x)
        {
            return 1 - Mathf.Pow(1 - x, 3);
        }
        
        private void UpdateVirtualCameraTransform()
        {
            var currentDirection = _virtualObject.transform.forward;
            var cameraDirection = _config.camera.transform.forward;
                
            var targetPosition = _config.camera.transform.position;
            _virtualPosition = targetPosition;
            
            if (!_needToFollowRotation)
            {
                var dot = Vector3.Dot(cameraDirection, _virtualCameraDirection);
                var angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                
                if (angle >= _config.lookAngleThresholdToFollow)
                {
                    _needToFollowRotation = true;
                    _followingT = 0;
                    
                    _startDirection = currentDirection;
                    var diff = (cameraDirection - currentDirection).normalized;
                    _targetDirection = cameraDirection;// + diff * 0.34f;
                }
            }

            if (_needToFollowRotation)
            {
                _followingT += _config.followingSpeed * Time.deltaTime;
                
                _virtualCameraDirection = Vector3.Slerp(_startDirection, cameraDirection, EaseOutCubic(_followingT));
                
                _virtualObject.transform.rotation = Quaternion.LookRotation(_virtualCameraDirection);
                
                if (_followingT >= 1f)
                    _needToFollowRotation = false;
            }
            
            _virtualObject.transform.position = _virtualPosition + _virtualCameraDirection * DistanceToCamera;
        }

        public void GetControllerTransforms(Vector3 deviceCursorPosition, Quaternion deviceCursorRotation, out Vector3 position, out Quaternion rotation)
        {
            position = _config.trackingSpace.transform.TransformPoint(deviceCursorPosition);
            rotation = _config.trackingSpace.transform.rotation * deviceCursorRotation;
        }
        
        public Vector2 GetCursorPosition(Vector3 deviceCursorPosition, Quaternion deviceCursorRotation)
        {
            GetControllerTransforms(deviceCursorPosition, deviceCursorRotation, out var cursorPosition, out var cursorRotation);
            
            var cursorDirection = cursorRotation * Vector3.forward;
            
            var ray = new Ray(cursorPosition, cursorDirection);

            var plane = new Plane(_virtualCameraDirection, _virtualObject.transform.position);
            if (plane.Raycast(ray, out var enter))
            {
                var pointWorldSpace = ray.GetPoint(enter);
                if (_lastWorldSpacePointPosition.magnitude == 0)
                    _lastWorldSpacePointPosition = pointWorldSpace;
                
                float smoothFactor = Mathf.Exp(-Time.deltaTime / _config.cursorSmoothTime);
                _lastWorldSpacePointPosition = Vector3.Lerp(pointWorldSpace, _lastWorldSpacePointPosition, smoothFactor);
                
                var pointScreenSpace = WorldToLocalMatrix.MultiplyPoint3x4(_lastWorldSpacePointPosition);
                _lastCursorPosition = pointScreenSpace;
                
                _lastCursorPosition.x = Mathf.Clamp(_lastCursorPosition.x, 0, Screen.width);
                _lastCursorPosition.y = Mathf.Clamp(Screen.height - _lastCursorPosition.y, 0, Screen.height);
            }
            
            return _lastCursorPosition;
        }
    }
}