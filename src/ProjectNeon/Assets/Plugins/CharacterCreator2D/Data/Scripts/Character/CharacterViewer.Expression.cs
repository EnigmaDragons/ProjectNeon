using System;
using UnityEngine;

namespace CharacterCreator2D
{
    public partial class CharacterViewer
    {
        /// <summary>
        /// Change character facial expression/emote.
        /// </summary>
        /// <param name="emotionType">target emotion.</param>
        public void Emote(EmotionType emotionType)
        {
            Emote(emotionType, 0f);
        }

        /// <summary>
        /// Change character facial expression/emote then reset to default face after a period of time.
        /// </summary>
        /// <param name="emotionType">target emotion.</param>
        /// <param name="duration">the duration of the emotion before resetting back to default.</param>
        public void Emote(EmotionType emotionType, float duration)
        {
            if (_isBaked)
            {
                return;
            }
            EmoteIndex i = emotes.getIndex(emotionType);
            if (isEmotingAnimationEvent)
            {
                updateAnimEmote(-1);
            }

            emote(i, duration, false);
        }

        /// <summary>
        /// Change character facial expression/emote.
        /// </summary>
        /// <param name="emotionName">target emotion name.</param>
        public void Emote(string emotionName)
        {
            Emote(emotionName, 0f);
        }

        /// <summary>
        /// Change character facial expression/emote then reset to default face after a period of time.
        /// </summary>
        /// <param name="emotionName">target emotion name.</param>
        /// <param name="duration">the duration of the emotion before resetting back to default.</param>
        public void Emote(string emotionName, float duration)
        {
            if (_isBaked)
            {
                return;
            }
            EmoteIndex i = null;
            foreach (EmotionType emotionType in Enum.GetValues(typeof(EmotionType)))
            {
                EmoteIndex e = emotes.getIndex(emotionType);
                if (e.name == emotionName)
                {
                    i = e;
                    break;
                }
            }
            if (i == null)
            {
                Debug.LogError("Emotion not found: " + emotionName);
                return;
            }
            if (isEmotingAnimationEvent)
            {
                updateAnimEmote(-1);
            }

            emote(i, duration, false);
        }

        private void emote(EmoteIndex i, float duration, bool isAnimation)
        {
            CancelInvoke("ResetEmote");
            isEmoting = !isAnimation;
            if (i.eyebrowPart != null)
            {
                EquipPart(SlotCategory.Eyebrow, i.eyebrowPart);
            }
            else
            {
                EquipPart(SlotCategory.Eyebrow, defaultEmote.eyebrowPart);
            }

            if (i.eyesPart != null)
            {
                EquipPart(SlotCategory.Eyes, i.eyesPart);
            }
            else
            {
                EquipPart(SlotCategory.Eyes, defaultEmote.eyesPart);
            }

            if (i.nosePart != null)
            {
                EquipPart(SlotCategory.Nose, i.nosePart);
            }
            else
            {
                EquipPart(SlotCategory.Nose, defaultEmote.nosePart);
            }

            if (i.mouthPart != null)
            {
                EquipPart(SlotCategory.Mouth, i.mouthPart);
            }
            else
            {
                EquipPart(SlotCategory.Mouth, defaultEmote.mouthPart);
            }

            if (i.earPart != null)
            {
                EquipPart(SlotCategory.Ear, i.earPart);
            }
            else
            {
                EquipPart(SlotCategory.Ear, defaultEmote.earPart);
            }

            if (duration > 0)
            {
                Invoke("ResetEmote", duration);
            }
        }

        /// <summary>
        /// Reset character facial expression/emote into its default face.
        /// </summary>
        public void ResetEmote()
        {
            if (_isBaked)
            {
                return;
            }
            EmoteIndex i = defaultEmote;
            EquipPart(SlotCategory.Eyebrow, i.eyebrowPart);
            EquipPart(SlotCategory.Eyes, i.eyesPart);
            EquipPart(SlotCategory.Nose, i.nosePart);
            EquipPart(SlotCategory.Mouth, i.mouthPart);
            EquipPart(SlotCategory.Ear, i.earPart);
            isEmoting = false;
        }

        private void getDefaultEmote()
        {
            Part p;
            p = GetAssignedPart(SlotCategory.Eyebrow);
            defaultEmote.eyebrowPart = p == null ? null : p;
            p = GetAssignedPart(SlotCategory.Eyes);
            defaultEmote.eyesPart = p == null ? null : p;
            p = GetAssignedPart(SlotCategory.Nose);
            defaultEmote.nosePart = p == null ? null : p;
            p = GetAssignedPart(SlotCategory.Mouth);
            defaultEmote.mouthPart = p == null ? null : p;
            p = GetAssignedPart(SlotCategory.Ear);
            defaultEmote.earPart = p == null ? null : p;
        }

        private void updateAnimEmote(float i)
        {
            int index = (int)i;
            if (index >= 0)
            {
                ResetEmote();
                isEmotingAnimationEvent = true;
                EmoteIndex e = emotes.getIndex((EmotionType)index);
                emote(e, 0f, true);
            }
            else
            {
                ResetEmote();
                isEmotingAnimationEvent = false;
            }
            _currentEmoteAnimIndex = emoteAnimIndex;
        }
    }
}
