using UnityEngine;

namespace CharacterCreator2D.Utilities
{
    public class CharacterManager : ScriptableObject
    {
        private static readonly CharacterSetter m_defaultSetter = new CharacterSetter();
        private static readonly CharacterAssigner m_defaultAssigner = new CharacterAssigner();
        private static readonly CharacterBaker m_defaultBaker = new CharacterBaker();

        private static CharacterManager m_defaultManger;

        public CharacterSetter Setter => GetSetter() ?? m_defaultSetter;

        public CharacterAssigner Assigner => GetAssigner() ?? m_defaultAssigner;

        public CharacterBaker Baker => GetBaker() ?? m_defaultBaker;

        public static CharacterManager DefaultManager
        {
            get
            {
                if (!m_defaultManger)
                {
                    m_defaultManger = CreateInstance<CharacterManager>();
                    m_defaultManger.name = "DefaultCharacterManager";
                }
                return m_defaultManger;
            }
        }


        protected virtual CharacterSetter GetSetter()
        {
            return m_defaultSetter;
        }

        protected virtual CharacterAssigner GetAssigner()
        {
            return m_defaultAssigner;
        }

        protected virtual CharacterBaker GetBaker()
        {
            return m_defaultBaker;
        }
    }
}
