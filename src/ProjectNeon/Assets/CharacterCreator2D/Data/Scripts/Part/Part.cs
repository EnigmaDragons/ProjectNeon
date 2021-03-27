using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterCreator2D
{
    public class Part : ScriptableObject
    {
        /// <summary>
        /// Part's package name.
        /// </summary>
        [Tooltip("Part's package name")]
        public string packageName;

        /// <summary>
        /// Part's category.
        /// </summary>
        [Tooltip("Part's category")]
        public PartCategory category;

        /// <summary>
        /// A list of BodyTypes supported by this Part.
        /// </summary>
        [Tooltip("List of BodyTypes supported by this Part")]
        public List<BodyType> supportedBody;

#if !CC2D_RES

        /// <summary>
        /// Part's main texture.
        /// </summary>
        [Tooltip("Part's main texture")]
        public Texture2D texture;

        /// <summary>
        /// Part's color mask.
        /// </summary>
        [Tooltip("Part's color mask")]
        public Texture2D colorMask;

        /// <summary>
        /// List of sprite used by this Part.
        /// </summary>
        [Tooltip("List of sprite used by this Part")]
        public List<Sprite> sprites;

#else

#if UNITY_EDITOR

        [SerializeField]
        [FormerlySerializedAs("texture")]
        [Tooltip("Part's main texture")]
        private Texture2D m_texture;

        [SerializeField]
        [FormerlySerializedAs("colorMask")]
        [Tooltip("Part's color mask")]
        private Texture2D m_colorMask;

        [SerializeField]
        [FormerlySerializedAs("sprites")]
        [Tooltip("List of sprite used by this Part")]
        private List<Sprite> m_sprites;

#endif

        [SerializeField]
        [ReadOnly]
        private string m_dataPath = "";

        private PartReferer m_partReferer;

        /// <summary>
        /// Part's main texture.
        /// </summary>
        public Texture2D texture
        {
            get
            {
#if UNITY_EDITOR
                return m_texture;
#else
                return PartReferer.texture;
#endif
            }
            set
            {
#if UNITY_EDITOR
                m_texture = value;
#endif
                PartReferer.texture = value;
            }
        }

        /// <summary>
        /// Part's color mask.
        /// </summary>
        public Texture2D colorMask
        {
            get
            {
#if UNITY_EDITOR
                return m_colorMask;
#else
                return PartReferer.colorMask;
#endif
            }
            set
            {
#if UNITY_EDITOR
                m_colorMask = value;
#endif
                PartReferer.colorMask = value;
            }
        }

        /// <summary>
        /// List of sprite used by this Part.
        /// </summary>
        public List<Sprite> sprites
        {
            get
            {
#if UNITY_EDITOR
                return m_sprites;
#else
                return PartReferer.sprites;
#endif
            }
            set
            {
#if UNITY_EDITOR
                m_sprites = value;
#endif
                PartReferer.sprites = value;
            }
        }

        /// <summary>
        /// Path to PartReferer in Resources that contains Texture and Sprites data in resources.
        /// </summary>
        public string datapath
        {
            get => m_dataPath;
            set
            {
#if UNITY_EDITOR
                m_dataPath = value;
#endif
            }
        }

        private PartReferer PartReferer
        {
            get
            {
                if (!m_partReferer)
                {
                    m_partReferer = Resources.Load<PartReferer>(m_dataPath);
                    if (m_partReferer)
                    {
                        ResourcesManager.RegisterObject(m_partReferer);
                    }
                    else
                    {
                        m_partReferer = CreateInstance<PartReferer>();
                        m_partReferer.sprites = new List<Sprite>();
#if UNITY_EDITOR
                        m_partReferer.hideFlags = HideFlags.DontSaveInEditor;
#endif
                    }
                }
                return m_partReferer;
            }
        }

#endif
    }
}
