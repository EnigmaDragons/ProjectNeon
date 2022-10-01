using System;
using AeLa.EasyFeedback.UI.Interfaces;
using UnityEngine;

namespace AeLa.EasyFeedback.UI
{
    internal abstract class UIInteropWrapper<T> : IUIInteropWrapper
        where T : Component
    {
        public static Type TargetType => typeof(T);

        public static T GetTarget(GameObject go)
        {
            return go.GetComponent<T>();
        }

        protected readonly T InternalTarget;

        public Component Target => InternalTarget;

        internal UIInteropWrapper(T internalTarget)
        {
            InternalTarget = internalTarget;
        }
    }
}