using UnityEngine;

namespace CharacterCreator2D.Utilities.Humanoid
{
    [CreateAssetMenu(fileName = "Humanoid Manager", menuName = "CC2D/Manager/Humanoid")]
    public class HumanoidManager : CharacterManager
    {
        [SerializeField]
        private HumanoidSetter m_setter = new HumanoidSetter();

        [SerializeField]
        private HumanoidAssigner m_assigner = new HumanoidAssigner();

        [SerializeField]
        private HumanoidBaker m_baker = new HumanoidBaker();

        protected override CharacterSetter GetSetter()
        {
            return m_setter;
        }

        protected override CharacterAssigner GetAssigner()
        {
            return m_assigner;
        }

        protected override CharacterBaker GetBaker()
        {
            return m_baker;
        }
    }
}
