using UnityEngine;
using UnityEngine.XR;
using Button = OVRInput.Button;
using Touch = OVRInput.Touch;

namespace XRInputUtility
{
    public class OVRControllerState : ControllerState
    {
        private readonly OVRInput.Controller controller;
        
        public override void Update()
        {
            m_PrimaryTouch = OVRInput.Get(Touch.One, controller);
            m_SecondaryTouch = OVRInput.Get(Touch.Two, controller);
            m_ThumbstickTouch = OVRInput.Get(Touch.PrimaryThumbstick, controller);
            m_TriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);
            m_GripValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);
            m_Axis2D = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller);
            var primaryPress = OVRInput.Get(Button.One, controller);
            var secondaryPress = OVRInput.Get(Button.Two, controller);
            var menuPress = OVRInput.Get(Button.Three, controller);
            var thumbstickPress = OVRInput.Get(Button.PrimaryThumbstick, controller);
            var triggerPress = OVRInput.Get(Button.PrimaryIndexTrigger, controller);
            var gripPress = OVRInput.Get(Button.PrimaryHandTrigger, controller);
            m_Primary.Update(primaryPress);
            m_Secondary.Update(secondaryPress);
            m_Menu.Update(menuPress);
            m_Thumbstick.Update(thumbstickPress);
            m_Trigger.Update(triggerPress);
            m_Grip.Update(gripPress);
        }

        public OVRControllerState(XRNode node) : base(node)
        {
            if(node is not (XRNode.LeftHand or XRNode.RightHand))return;
            controller = node is XRNode.LeftHand ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
        }


    }
}
