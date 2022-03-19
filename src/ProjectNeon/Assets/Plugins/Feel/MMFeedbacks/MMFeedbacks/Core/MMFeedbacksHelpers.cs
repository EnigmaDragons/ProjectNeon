using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    public class MMFeedbacksHelpers : MonoBehaviour
    {
        /// <summary>
		/// Remaps a value x in interval [A,B], to the proportional value in interval [C,D]
		/// </summary>
		/// <param name="x">The value to remap.</param>
		/// <param name="A">the minimum bound of interval [A,B] that contains the x value</param>
		/// <param name="B">the maximum bound of interval [A,B] that contains the x value</param>
		/// <param name="C">the minimum bound of target interval [C,D]</param>
		/// <param name="D">the maximum bound of target interval [C,D]</param>
		public static float Remap(float x, float A, float B, float C, float D)
        {
            float remappedValue = C + (x - A) / (B - A) * (D - C);
            return remappedValue;
        }
    }

    public class MMFReadOnlyAttribute : PropertyAttribute { }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class MMFInspectorButtonAttribute : PropertyAttribute
    {
        public readonly string MethodName;

        public MMFInspectorButtonAttribute(string MethodName)
        {
            this.MethodName = MethodName;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class MMFEnumConditionAttribute : PropertyAttribute
    {
        public string ConditionEnum = "";
        public bool Hidden = false;

        BitArray bitArray = new BitArray(32);
        public bool ContainsBitFlag(int enumValue)
        {
            return bitArray.Get(enumValue);
        }

        public MMFEnumConditionAttribute(string conditionBoolean, params int[] enumValues)
        {
            this.ConditionEnum = conditionBoolean;
            this.Hidden = true;

            for (int i = 0; i < enumValues.Length; i++)
            {
                bitArray.Set(enumValues[i], true);
            }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MMFInspectorButtonAttribute))]
    public class MMFInspectorButtonPropertyDrawer : PropertyDrawer
    {
        private MethodInfo _eventMethodInfo = null;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            MMFInspectorButtonAttribute inspectorButtonAttribute = (MMFInspectorButtonAttribute)attribute;

            float buttonLength = position.width;
            Rect buttonRect = new Rect(position.x + (position.width - buttonLength) * 0.5f, position.y, buttonLength, position.height);

            if (GUI.Button(buttonRect, inspectorButtonAttribute.MethodName))
            {
                System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
                string eventName = inspectorButtonAttribute.MethodName;

                if (_eventMethodInfo == null)
                {
                    _eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                }

                if (_eventMethodInfo != null)
                {
                    _eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
                }
                else
                {
                    Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
                }
            }
        }
    }
#endif

    public class MMFInformationAttribute : PropertyAttribute
    {
        public enum InformationType { Error, Info, None, Warning }

#if UNITY_EDITOR
        public string Message;
        public MessageType Type;
        public bool MessageAfterProperty;

        public MMFInformationAttribute(string message, InformationType type, bool messageAfterProperty)
        {
            this.Message = message;
            if (type == InformationType.Error) { this.Type = UnityEditor.MessageType.Error; }
            if (type == InformationType.Info) { this.Type = UnityEditor.MessageType.Info; }
            if (type == InformationType.Warning) { this.Type = UnityEditor.MessageType.Warning; }
            if (type == InformationType.None) { this.Type = UnityEditor.MessageType.None; }
            this.MessageAfterProperty = messageAfterProperty;
        }
#else
		public MMFInformationAttribute(string message, InformationType type, bool messageAfterProperty)
		{

		}
#endif
    }

    public class MMFHiddenAttribute : PropertyAttribute { }

    [AttributeUsage(System.AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class MMFConditionAttribute : PropertyAttribute
    {
        public string ConditionBoolean = "";
        public bool Hidden = false;

        public MMFConditionAttribute(string conditionBoolean)
        {
            this.ConditionBoolean = conditionBoolean;
            this.Hidden = false;
        }

        public MMFConditionAttribute(string conditionBoolean, bool hideInInspector)
        {
            this.ConditionBoolean = conditionBoolean;
            this.Hidden = hideInInspector;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class MMFHiddenPropertiesAttribute : Attribute
    {
        public string[] PropertiesNames;

        public MMFHiddenPropertiesAttribute(params string[] propertiesNames)
        {
            PropertiesNames = propertiesNames;
        }
    }
    
    /// <summary>
    /// An attribute used to group inspector fields under common dropdowns
    /// Implementation inspired by Rodrigo Prinheiro's work, available at https://github.com/RodrigoPrinheiro/unityFoldoutAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class MMFInspectorGroupAttribute : PropertyAttribute
    {
        public string GroupName;
        public bool GroupAllFieldsUntilNextGroupAttribute;
        public int GroupColorIndex;
        public bool RequiresSetup;
        public bool ClosedByDefault;

        public MMFInspectorGroupAttribute(string groupName, bool groupAllFieldsUntilNextGroupAttribute = false, int groupColorIndex = 24, bool requiresSetup = false, bool closedByDefault = false)
        {
            if (groupColorIndex > 139) { groupColorIndex = 139; }

            this.GroupName = groupName;
            this.GroupAllFieldsUntilNextGroupAttribute = groupAllFieldsUntilNextGroupAttribute;
            this.GroupColorIndex = groupColorIndex;
            this.RequiresSetup = requiresSetup;
            this.ClosedByDefault = closedByDefault;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class TmpAttribute : PropertyAttribute
    {
        /// <summary>
        ///   <para>The header text.</para>
        /// </summary>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=HeaderAttribute.header">`HeaderAttribute.header` on docs.unity3d.com</a></footer>
        public readonly string header;

        /// <summary>
        ///   <para>Add a header above some fields in the Inspector.</para>
        /// </summary>
        /// <param name="header">The header text.</param>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=HeaderAttribute">`HeaderAttribute` on docs.unity3d.com</a></footer>
        public TmpAttribute(string header) => this.header = header;
    }

    public static class MMFeedbackStaticMethods
    {
        static List<Component> m_ComponentCache = new List<Component>();

        /// <summary>
        /// Grabs a component without allocating memory uselessly
        /// </summary>
        /// <param name="this"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
		public static Component GetComponentNoAlloc(this GameObject @this, System.Type componentType)
        {
            @this.GetComponents(componentType, m_ComponentCache);
            var component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
            m_ComponentCache.Clear();
            return component;
        }
        
        public static Type MMFGetTypeByName(string name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name == name)
                    {
                        return type;
                    }
                }
            }
 
            return null;
        }

        /// <summary>
        /// Grabs a component without allocating memory uselessly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T MMFGetComponentNoAlloc<T>(this GameObject @this) where T : Component
        {
            @this.GetComponents(typeof(T), m_ComponentCache);
            Component component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
            m_ComponentCache.Clear();
            return component as T;
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Returns the object value of a target serialized property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static object MMFGetObjectValue(this SerializedProperty property)
        {
            if (property == null)
            {
                return null;
            }

            string propertyPath = property.propertyPath.Replace(".Array.data[", "[");
            object targetObject = property.serializedObject.targetObject;
            var elements = propertyPath.Split('.');
            foreach (var element in elements)
            {
                if (!element.Contains("["))
                {
                    targetObject = MMFGetPropertyValue(targetObject, element);
                }
                else
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int elementIndex = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    targetObject = MMFGetPropertyValue(targetObject, elementName, elementIndex);
                }
            }
            return targetObject;
        }
        
        private static object MMFGetPropertyValue(object source, string propertyName)
        {
            if (source == null)
            {
                return null;
            }
                 
            Type propertyType = source.GetType();

            while (propertyType != null)
            {
                FieldInfo fieldInfo = propertyType.GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    return fieldInfo.GetValue(source);
                }
                PropertyInfo propertyInfo = propertyType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    return propertyInfo.GetValue(source, null);
                }
                propertyType = propertyType.BaseType;
            }
            return null;
        }

        private static object MMFGetPropertyValue(object source, string propertyName, int index)
        {
            var enumerable = MMFGetPropertyValue(source, propertyName) as System.Collections.IEnumerable;
            if (enumerable == null)
            {
                return null;
            }
            var enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
            }
            return enumerator.Current;
        }
        #endif
    }

    /// <summary>
    /// Atttribute used to mark feedback class.
    /// The provided path is used to sort the feedback list displayed in the feedback manager dropdown
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FeedbackPathAttribute : System.Attribute
    {
        public string Path;
        public string Name;

        public FeedbackPathAttribute(string path)
        {
            Path = path;
            Name = path.Split('/').Last();
        }

        static public string GetFeedbackDefaultName(System.Type type)
        {
            FeedbackPathAttribute attribute = type.GetCustomAttributes(false).OfType<FeedbackPathAttribute>().FirstOrDefault();
            return attribute != null ? attribute.Name : type.Name;
        }

        static public string GetFeedbackDefaultPath(System.Type type)
        {
            FeedbackPathAttribute attribute = type.GetCustomAttributes(false).OfType<FeedbackPathAttribute>().FirstOrDefault();
            return attribute != null ? attribute.Path : type.Name;
        }
    }

    /// <summary>
    /// Atttribute used to mark feedback class.
    /// The contents allow you to specify a help text for each feedback
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FeedbackHelpAttribute : System.Attribute
    {
        public string HelpText;

        public FeedbackHelpAttribute(string helpText)
        {
            HelpText = helpText;
        }

        static public string GetFeedbackHelpText(System.Type type)
        {
            FeedbackHelpAttribute attribute = type.GetCustomAttributes(false).OfType<FeedbackHelpAttribute>().FirstOrDefault();
            return attribute != null ? attribute.HelpText : "";
        }
    }
    
    public static class MMF_FieldInfo
    {
        public static Dictionary<int, List<FieldInfo>> FieldInfoList = new Dictionary<int, List<FieldInfo>>();

        
        public static int GetFieldInfo(MMF_Feedback target, out List<FieldInfo> fieldInfoList)
        {
            Type targetType = target.GetType();
            int targetTypeHashCode = targetType.GetHashCode();

            if (!FieldInfoList.TryGetValue(targetTypeHashCode, out fieldInfoList))
            {
                IList<Type> typeTree = targetType.GetBaseTypes();
                fieldInfoList = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.NonPublic)
                    .OrderByDescending(x => typeTree.IndexOf(x.DeclaringType))
                    .ToList();
                FieldInfoList.Add(targetTypeHashCode, fieldInfoList);
            }

            return fieldInfoList.Count;
        }
        
        public static int GetFieldInfo(UnityEngine.Object target, out List<FieldInfo> fieldInfoList)
        {
            Type targetType = target.GetType();
            int targetTypeHashCode = targetType.GetHashCode();

            if (!FieldInfoList.TryGetValue(targetTypeHashCode, out fieldInfoList))
            {
                IList<Type> typeTree = targetType.GetBaseTypes();
                fieldInfoList = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.NonPublic)
                    .OrderByDescending(x => typeTree.IndexOf(x.DeclaringType))
                    .ToList();
                FieldInfoList.Add(targetTypeHashCode, fieldInfoList);
            }

            return fieldInfoList.Count;
        }

        public static IList<Type> GetBaseTypes(this Type t)
        {
            var types = new List<Type>();
            while (t.BaseType != null)
            {
                types.Add(t);
                t = t.BaseType;
            }

            return types;
        }
    }
}