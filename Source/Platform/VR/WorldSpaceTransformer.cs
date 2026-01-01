using System.Numerics;
using ImGuiNET;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace UImGui.Platform
{
    public class WorldSpaceTransformer
    {
        private readonly WorldSpaceTransformerConfig _config;
        private readonly GameObject _virtualCamera;
        private Vector3 _virtualCameraDirection;
        private bool _needToFollowRotation;
        
        public float DistanceToCamera;
        public Matrix4x4 LocalToWorldMatrix;
        
        private float _followingT;
        private Vector3 _virtualPosition;
        private Vector3 _startDirection;
        private Vector3 _targetDirection;


        public WorldSpaceTransformer(WorldSpaceTransformerConfig config)
        {
            _config = config;

            _virtualCamera = new GameObject("IMGUI Virtual Camera");
            DistanceToCamera = config.initialDistanceFromCamera;

            _virtualCameraDirection = config.camera.transform.forward;
            _virtualPosition = config.camera.transform.position;
            
            var vPosition = _virtualPosition + _virtualCameraDirection * DistanceToCamera;
            var vRotation = config.camera.transform.rotation;
            
            _virtualCamera.transform.SetPositionAndRotation(vPosition, vRotation);
        }

        public void Update()
        {
            UpdateVirtualCameraTransform();
            UpdateLocalToWorldMatrix();
        }

        private void UpdateLocalToWorldMatrix()
        {
            var virtualCameraPosition = _virtualCamera.transform.position;
            var virtualCameraRotation = _virtualCamera.transform.rotation;

            var drawData = ImGui.GetDrawData(); //TODO: This looks bad, maybe we can safely use Screen class for getting display size 
            
            float w = drawData.DisplaySize.x;
            float h = drawData.DisplaySize.y;
            
            float pixelsPerUnit = 1000f; 
            float scale = 1f / pixelsPerUnit;
            
            Vector3 offset = virtualCameraRotation * new Vector3(-w * 0.5f * scale, h * 0.5f * scale, 0);

            LocalToWorldMatrix = Matrix4x4.TRS(
                virtualCameraPosition + offset,
                virtualCameraRotation,
                new Vector3(scale, -scale, scale)
            );
        }

        private float EaseOutCubic(float x)
        {
            return 1 - Mathf.Pow(1 - x, 3);
        }
        
        private void UpdateVirtualCameraTransform()
        {
            var currentDirection = _virtualCamera.transform.forward;
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
                    _targetDirection = cameraDirection + diff * 0.34f;
                }
            }

            if (_needToFollowRotation)
            {
                _followingT += _config.followingSpeed * Time.deltaTime;
                
                _virtualCameraDirection = Vector3.Slerp(_startDirection, _targetDirection, EaseOutCubic(_followingT));
                
                _virtualCamera.transform.rotation = Quaternion.LookRotation(_virtualCameraDirection);
                
                if (_followingT >= 1f)
                    _needToFollowRotation = false;
            }
            
            _virtualCamera.transform.position = _virtualPosition + _virtualCameraDirection * DistanceToCamera;
        }
    }
}