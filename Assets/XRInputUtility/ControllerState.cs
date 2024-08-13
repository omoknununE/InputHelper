using System;
using UnityEngine;
using XR = UnityEngine.XR;


namespace XRInputUtility
{
    public abstract class ControllerState
    {
        public readonly XR.XRNode Side;

        protected readonly ButtonState m_Primary = new();
        protected readonly ButtonState m_Secondary = new();
        protected readonly ButtonState m_Thumbstick = new();
        protected readonly ButtonState m_Menu = new();
        protected readonly ButtonState m_Trigger = new();
        protected readonly ButtonState m_Grip = new();
        protected bool m_PrimaryTouch;
        protected bool m_SecondaryTouch;
        protected bool m_ThumbstickTouch;
        protected float m_TriggerValue;
        protected float m_GripValue;
        protected Vector2 m_Axis2D;

        public IReadOnlyButton Primary => m_Primary;
        public IReadOnlyButton Secondary => m_Secondary;
        public IReadOnlyButton Thumbstick => m_Thumbstick;
        public IReadOnlyButton Menu => m_Menu;
        public IReadOnlyButton Trigger => m_Trigger;
        public IReadOnlyButton Grip => m_Grip;
        public bool PrimaryTouch => m_PrimaryTouch;
        public bool SecondaryTouch => m_SecondaryTouch;
        public bool ThumbstickTouch => m_ThumbstickTouch;
        public float TriggerValue => m_TriggerValue;
        public float GripValue => m_GripValue;
        public Vector2 Axis2D => m_Axis2D;

        public abstract void Update();

        protected void ResetState()
        {
            m_Primary.Update(false);
            m_Secondary.Update(false);
            m_Thumbstick.Update(false);
            m_Menu.Update(false);
            m_Trigger.Update(false);
            m_Grip.Update(false);
            m_PrimaryTouch = false;
            m_SecondaryTouch = false;
            m_ThumbstickTouch = false;
            m_TriggerValue = 0f;
            m_GripValue = 0f;
            m_Axis2D = Vector2.zero;
        }

        protected ControllerState(XR.XRNode node)
        {
            if (node is not XR.XRNode.LeftHand and not XR.XRNode.RightHand)
                throw new ArgumentException("Parameter is not LeftHand or RightHand", nameof(node));
            Side = node;
        }
    }
}