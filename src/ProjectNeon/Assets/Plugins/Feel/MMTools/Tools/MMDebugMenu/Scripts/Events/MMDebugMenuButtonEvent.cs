using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    /// An event used to broadcast button events from a MMDebugMenu
    /// </summary>
    public struct MMDebugMenuButtonEvent
    {
        public enum EventModes { FromButton, SetButton }

        public delegate void Delegate(string buttonEventName, bool active = true, EventModes eventMode = EventModes.FromButton);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(string buttonEventName, bool active = true, EventModes eventMode = EventModes.FromButton)
        {
            OnEvent?.Invoke(buttonEventName, active, eventMode);
        }
    }
}