using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CharacterCreator2D
{
    /// <summary>
    /// Static class for managing asset loaded with <see cref="Resources.Load(string)"/>.
    /// </summary>
    public static class ResourcesManager
    {
#if !UNITY_EDITOR && CC2D_RES
        private static readonly Dictionary<Object, HashSet<Func<Object>>> m_resGetterDict = new Dictionary<Object, HashSet<Func<Object>>>();
#endif

        /// <summary>
        /// Register to be released Object. Calling <see cref="ReleaseZeroReference()"/> will release and unload the object unless it has reference chains.
        /// &#xD;&#xD;See also : <seealso cref="RegisterReference{T}(T, Func{T})"/>, <seealso cref="ReleaseZeroReference()"/>
        /// </summary>
        public static void RegisterObject(Object unityObject)
        {
#if !UNITY_EDITOR && CC2D_RES
            if (unityObject)
            {
                if (!m_resGetterDict.ContainsKey(unityObject))
                {
                    m_resGetterDict[unityObject] = new HashSet<Func<Object>>();
                }
            }
#endif
        }

        /// <summary>
        /// Register Object reference chain. Calling <see cref="ReleaseZeroReference()"/> will release and unload if all getters to certain object are unequal or null.
        /// &#xD;&#xD;<example>
        /// Example:
        /// <code>
        ///     Sprite sprite = Resources.Load&lt;Sprite>("SomeTexture/SomeSprite");
        ///     RegisterReference(sprite.texture, () => sprite.texture);
        /// </code>
        /// </example>
        /// &#xD;&#xD;See also : <seealso cref="RegisterObject(Object)"/>, <seealso cref="ReleaseZeroReference()"/>
        /// </summary>
        /// <typeparam name="T">Unity Object Type</typeparam>
        /// <param name="unityObject">Stored unity object</param>
        /// <param name="getter">Reference getter delegate</param>
        public static void RegisterReference<T>(T unityObject, Func<T> getter) where T : Object
        {
#if !UNITY_EDITOR && CC2D_RES
            if (unityObject)
            {
                if (getter != null)
                {
                    if (m_resGetterDict.TryGetValue(unityObject, out HashSet<Func<Object>> result))
                    {
                        if (result != null)
                        {
                            result.Add(getter);
                        }
                    }
                    m_resGetterDict[unityObject] = new HashSet<Func<Object>>() { getter };
                }
            }
#endif
        }

        /// <summary>
        /// Release assets that have zero references and unload them. Only unload unused assets and assets registered via
        /// <see cref="RegisterObject(Object)"/> or <see cref="RegisterReference{T}(T, Func{T})"/>.
        /// </summary>
        public static void ReleaseZeroReference()
        {
#if !UNITY_EDITOR && CC2D_RES
            bool isUnload = true;
            while (isUnload)
            {
                isUnload = false;
                Dictionary<Object, HashSet<Func<Object>>> resGetterDictCopy = new Dictionary<Object, HashSet<Func<Object>>>(m_resGetterDict);
                foreach (KeyValuePair<Object, HashSet<Func<Object>>> pair in resGetterDictCopy)
                {
                    var getterSetCopy = new List<Func<Object>>(pair.Value);
                    foreach (Func<Object> getter in getterSetCopy)
                    {
                        if (!pair.Key.Equals(getter()))
                        {
                            pair.Value.Remove(getter);
                            isUnload = true;
                        }
                    }
                    if (pair.Value.Count == 0)
                    {
                        m_resGetterDict.Remove(pair.Key);
                        Resources.UnloadAsset(pair.Key);
                    }
                }
            }
            Resources.UnloadUnusedAssets();
#endif
        }
    }
}
