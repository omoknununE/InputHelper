using System.Collections.Generic;
using System.Linq;
using XR = UnityEngine.XR;

namespace XRInputUtility
{
    public class XRControllerState : ControllerState
    {
        private readonly List<XR.InputDevice> m_Devices = new();

        public override void Update()
        {
            XR.InputDevices.GetDevicesAtXRNode(Side, m_Devices);
            var device = m_Devices.FirstOrDefault();
            if (device == default)
            {
                ResetState();
                return;
            }

            device.TryGetFeatureValue(XR.CommonUsages.primaryTouch, out m_PrimaryTouch);
            device.TryGetFeatureValue(XR.CommonUsages.secondaryTouch, out m_SecondaryTouch);
            device.TryGetFeatureValue(XR.CommonUsages.primary2DAxisClick, out m_ThumbstickTouch);
            device.TryGetFeatureValue(XR.CommonUsages.trigger, out m_TriggerValue);
            device.TryGetFeatureValue(XR.CommonUsages.grip, out m_GripValue);
            device.TryGetFeatureValue(XR.CommonUsages.primary2DAxis, out m_Axis2D);
            device.TryGetFeatureValue(XR.CommonUsages.primaryButton, out var primaryPress);
            device.TryGetFeatureValue(XR.CommonUsages.secondaryButton, out var secondaryPress);
            device.TryGetFeatureValue(XR.CommonUsages.primary2DAxisClick, out var thumbstickPress);
            device.TryGetFeatureValue(XR.CommonUsages.menuButton, out var menuPress);
            device.TryGetFeatureValue(XR.CommonUsages.triggerButton, out var triggerPress);
            device.TryGetFeatureValue(XR.CommonUsages.gripButton, out var gripPress);
            m_Primary.Update(primaryPress);
            m_Secondary.Update(secondaryPress);
            m_Thumbstick.Update(thumbstickPress);
            m_Menu.Update(menuPress);
            m_Trigger.Update(triggerPress);
            m_Grip.Update(gripPress);
        }

        public XRControllerState(XR.XRNode node) : base(node)
        {
            
        }
    }
}

