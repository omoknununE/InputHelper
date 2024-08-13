using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Oculus.Interaction;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Management;
using Object = UnityEngine.Object;
using XR = UnityEngine.XR;


public struct PoseVelocity
{
    public Vector3 Position;
    public float Angular;
}

namespace XRInputUtility
{
    public static class InputUtility
    {
        private static readonly List<XR.XRNodeState> s_NodeStates = new();
        private static Transform m_RootTransform;
        private static PoseVelocity m_HeadVelocity;
        private static PoseVelocity m_LeftVelocity;
        private static PoseVelocity m_RightVelocity;
        private static Pose m_LocalHead;
        private static Pose m_LocalLeft;
        private static Pose m_LocalRight;
        private static Pose m_Root;
        private static bool m_UseOVR;

        public static Transform RootTransform => m_RootTransform;
        public static PoseVelocity HeadVelocity => m_HeadVelocity;
        public static PoseVelocity LeftVelocity => m_LeftVelocity;
        public static PoseVelocity RightVelocity => m_RightVelocity;
        public static Pose LocalHead => m_LocalHead;
        public static Pose LocalLeft => m_LocalLeft;
        public static Pose LocalRight => m_LocalRight;
        public static Pose Root => m_Root;
        public static Pose WorldHead { get; private set; }
        public static Pose WorldLeft { get; private set; }
        public static Pose WorldRight { get; private set; }
        public static bool IsHeadTracked { get; private set; }
        public static bool IsLeftTracked { get; private set; }
        public static bool IsRightTracked { get; private set; }
        public static bool IsRootTracked { get; private set; }
        public static ControllerState LeftController { get; private set; }
        public static ControllerState RightController { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var gameLoopHolder = new GameObject("InputUtility");
            Object.DontDestroyOnLoad(gameLoopHolder);
            var ct = gameLoopHolder.GetCancellationTokenOnDestroy();
            var loader = XRGeneralSettings.Instance.Manager.activeLoader;
            if (loader == null)
            {
                Debug.LogError("not using XRLoader ");
                Object.Destroy(gameLoopHolder);
                return;
            }
            m_UseOVR = loader.name == "Oculus Loader";
            Debug.LogError($"OVR 사용 : {m_UseOVR}");
            if (m_UseOVR)
            {

                LeftController = new OVRControllerState(XR.XRNode.LeftHand);
                RightController = new OVRControllerState(XR.XRNode.RightHand);
            }
            else
            {
                LeftController = new XRControllerState(XR.XRNode.LeftHand);
                RightController = new XRControllerState(XR.XRNode.RightHand);
            }
            UpdateInput(ct).Forget();
        }

        private static async UniTaskVoid UpdateInput(CancellationToken ct)
        {
            while (await UniTask.Yield(PlayerLoopTiming.Update,ct).SuppressCancellationThrow() == false)
            {
                UpdatePose(Time.deltaTime);
                UpdateRoot();
                UpdateController();
            }
        }
    
    
        public static void RegisterRootTransform(Transform transform)
        {
            m_RootTransform = transform;
            IsRootTracked = m_RootTransform != null;
        }
    

        private static void UpdateRoot()
        {
            m_Root = IsRootTracked == false ? Pose.identity : m_RootTransform.GetWorldPose();
            var rootPos = m_Root.position;
            var rootRot = m_Root.rotation;
            WorldHead = CalcWorldPose(m_LocalHead);
            WorldLeft = CalcWorldPose(m_LocalLeft);
            WorldRight = CalcWorldPose(m_LocalRight);
            return;

            Pose CalcWorldPose(Pose child)
            {
                return new Pose()
                {
                    position = rootPos + rootRot * child.position,
                    rotation = rootRot * child.rotation
                };
            }
        }


        private static void UpdatePose(float deltaTime)
        {
            XR.InputTracking.GetNodeStates(s_NodeStates);
            foreach (var nodeState in s_NodeStates)
            {
                switch (nodeState.nodeType)
                {
                    case XR.XRNode.CenterEye:
                        IsHeadTracked = TryUpdateTrackingPose(nodeState, ref m_LocalHead, ref m_HeadVelocity, deltaTime);
                        break;
                    case XR.XRNode.LeftHand:
                        IsLeftTracked = TryUpdateTrackingPose(nodeState, ref m_LocalLeft, ref m_LeftVelocity, deltaTime);
                        break;
                    case XR.XRNode.RightHand:
                        IsRightTracked = TryUpdateTrackingPose(nodeState, ref m_LocalRight, ref m_RightVelocity, deltaTime);
                        break;
                    case XR.XRNode.RightEye:
                    case XR.XRNode.Head:
                    case XR.XRNode.GameController:
                    case XR.XRNode.TrackingReference:
                    case XR.XRNode.HardwareTracker:
                    case XR.XRNode.LeftEye:
                    default:
                        break;
                }
            }
            return;
            
            bool TryUpdateTrackingPose(XR.XRNodeState state,ref Pose targetPose, ref PoseVelocity poseVelocity,float delta)
            {
                var currentPose = new Pose();
                var isPositionTracked = state.TryGetPosition(out currentPose.position);
                var isRotationTracked = state.TryGetRotation(out currentPose.rotation);
                var trackingSuccess = isPositionTracked && isRotationTracked;
                if (trackingSuccess)
                {
                    poseVelocity.CalculateVelocity(in targetPose,in currentPose,delta);
                }
                else
                {
                    poseVelocity.Position = Vector3.zero;
                    poseVelocity.Angular = 0;
                }
                targetPose.CopyPose(currentPose);
                return trackingSuccess;
            }
            
        }
        
        private static void UpdateController()
        {
            LeftController.Update();
            RightController.Update();
        }
        
        public static void CopyPose(ref this Pose to, in Pose from)
        {
            to.position = from.position;
            to.rotation = from.rotation;
        }
        
        public static void CalculateVelocity(ref this PoseVelocity poseVelocity,in Pose prev, in Pose current,float delta)
        {
            var prevRot = prev.rotation;
            var currentRot = current.rotation;
            var angle = Quaternion.Angle(prevRot, currentRot) / delta;
            var sign = Mathf.Sign(Quaternion.Dot(prevRot, currentRot));
            poseVelocity.Angular = angle * sign / delta;
            poseVelocity.Position = (current.position - prev.position) / delta;
        }
    
  
    }
}