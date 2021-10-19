using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    public partial class CharacterViewer
    {
        public class DataManager
        {
            public CharacterData CreateCharacterData(CharacterViewer character)
            {
                if (!character)
                {
                    return default;
                }
                CharacterData val = new CharacterData
                {
                    dataVersion = character.setupData.dataVersion,
                    bodyType = character.bodyType,
                    skinColor = character._skincolor,
                    tintColor = character._tintcolor,
                };
                val.slotData = new List<PartSlotData>();
                GetPartSlotDataList(character, val.slotData);
                val.emoteData = new List<EmoteIndexData>();
                GetEmotionIndexDataList(character, val.emoteData);
                val.bodySegmentData = new List<SegmentScaleData>();
                GetSegmentScaleDataList(character, val.bodySegmentData);
                return val;
            }

            public bool GetPartSlotDataList(CharacterViewer character, List<PartSlotData> result)
            {
                if (!character)
                {
                    return false;
                }
                if (result == null)
                {
                    return false;
                }
                result.Clear();
                foreach (SlotCategory cat in Enum.GetValues(typeof(SlotCategory)))
                {
                    PartSlot slot = character.slots.GetSlot(cat);
                    result.Add(CreatePartSlotData(cat, slot));
                }
                return true;
            }

            public PartSlotData CreatePartSlotData(SlotCategory cat, PartSlot slot)
            {
                return new PartSlotData()
                {
                    category = cat,
                    partName = slot.assignedPart == null ? "" : slot.assignedPart.name,
                    partPackage = slot.assignedPart == null ? "" : slot.assignedPart.packageName,
                    color1 = slot.color1,
                    color2 = slot.color2,
                    color3 = slot.color3
                };
            }

            public bool GetEmotionIndexDataList(CharacterViewer character, List<EmoteIndexData> result)
            {
                if (!character)
                {
                    return false;
                }
                if (result == null)
                {
                    return false;
                }
                result.Clear();
                foreach (EmotionType emotionType in Enum.GetValues(typeof(EmotionType)))
                {
                    EmoteIndex e = character.emotes.getIndex(emotionType);
                    if (emotionType >= EmotionType.Blink)
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(e.name))
                    {
                        continue;
                    }
                    result.Add(CreateEmoteIndexData(emotionType, e));
                }
                return true;
            }

            public EmoteIndexData CreateEmoteIndexData(EmotionType emotionType, EmoteIndex emoteIndex)
            {
                if (emoteIndex == null)
                {
                    return default;
                }
                EmoteIndexData result = new EmoteIndexData()
                {
                    emotionType = emotionType
                };
                if (emoteIndex != null)
                {
                    result.emotionName = string.IsNullOrEmpty(emoteIndex.name) ? "" : emoteIndex.name;
                    result.eyebrowPartName = GetPartName(emoteIndex.eyebrowPart);
                    result.eyebrowPackage = GetParPackage(emoteIndex.eyebrowPart);
                    result.eyesPartName = GetPartName(emoteIndex.eyesPart);
                    result.eyesPackage = GetParPackage(emoteIndex.eyesPart);
                    result.nosePartName = GetPartName(emoteIndex.nosePart);
                    result.nosePackage = GetParPackage(emoteIndex.nosePart);
                    result.mouthPartName = GetPartName(emoteIndex.mouthPart);
                    result.mouthPackage = GetParPackage(emoteIndex.mouthPart);
                    result.earPartName = GetPartName(emoteIndex.earPart);
                    result.earPackage = GetParPackage(emoteIndex.earPart);
                }
                return result;
            }

            public bool GetSegmentScaleDataList(CharacterViewer character, List<SegmentScaleData> result)
            {
                if (!character)
                {
                    return false;
                }
                if (result == null)
                {
                    return false;
                }
                result.Clear();
                foreach (SegmentType stype in Enum.GetValues(typeof(SegmentType)))
                {
                    Vector2 scale = character.GetBodySlider(stype);
                    result.Add(new SegmentScaleData()
                    {
                        segmentType = stype,
                        scale = new Vector2(scale.x, scale.y)
                    });
                }
                return true;
            }

            private static string GetPartName(Part part)
            {
                if (!part)
                {
                    return "";
                }
                return part.name;
            }

            private static string GetParPackage(Part part)
            {
                if (!part)
                {
                    return "";
                }
                return part.packageName;
            }

            public void AssignCharacterData(CharacterViewer character, CharacterData data)
            {
                //data version exception..
                if (data.dataVersion < 1)
                {
                    data.slotData.Add(new PartSlotData()
                    {
                        category = SlotCategory.BodySkin,
                        partName = data.bodyType == BodyType.Male ? "Base 00 Male" : "Base 00 Female",
                        partPackage = "Base"
                    });
                }
                //..data version exception

                character.SetBodyType(data.bodyType);
                character.SkinColor = data.skinColor;
                character.TintColor = data.tintColor;
                SetPartSlots(character, data.slotData);
                SetEmotions(character, data.emoteData);
                SetSegmentScales(character, data.bodySegmentData);
            }

            public bool SetPartSlots(CharacterViewer character, List<PartSlotData> slotDatas)
            {
                if (!character)
                {
                    return false;
                }
                if (slotDatas == null && slotDatas.Count <= 0)
                {
                    return false;
                }
                foreach (SlotCategory cat in Enum.GetValues(typeof(SlotCategory)))
                {
                    PartSlotData slotdata = slotDatas.Find(x => x.category == cat);
                    SetPartSlot(character, slotdata);
                }
                return true;
            }

            public bool SetPartSlot(CharacterViewer character, PartSlotData slotData)
            {
                SlotCategory cat = slotData.category;
                if (string.IsNullOrEmpty(slotData.partName))
                {
                    character.EquipPart(cat, (Part)null);
                    return false;
                }
                Part part = PartList.Static.FindPart(slotData.partName, slotData.partPackage, cat);
                if (part == null)
                {
                    return false;
                }
                character.EquipPart(cat, part);
                character.SetPartColor(cat, slotData.color1, slotData.color2, slotData.color3);
                return true;
            }

            public bool SetEmotions(CharacterViewer character, List<EmoteIndexData> emoteDatas)
            {
                if (!character)
                {
                    return false;
                }
                if (emoteDatas == null && emoteDatas.Count <= 0)
                {
                    return false;
                }
                foreach (EmotionType emotionType in Enum.GetValues(typeof(EmotionType)))
                {
                    EmoteIndexData edata = emoteDatas.Find(x => x.emotionType == emotionType);
                    if (emotionType >= EmotionType.Blink)
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(edata.emotionName))
                    {
                        continue;
                    }
                    SetEmotion(character, edata);
                }
                return true;
            }

            public bool SetEmotion(CharacterViewer character, EmoteIndexData edata)
            {
                if (!character)
                {
                    return false;
                }
                EmoteIndex targetEmoteIndex = character.emotes.getIndex(edata.emotionType);
                if (targetEmoteIndex == null)
                {
                    return false;
                }
                targetEmoteIndex.name = edata.emotionName;
                targetEmoteIndex.eyebrowPart = PartList.Static.FindPart(edata.eyebrowPartName, edata.eyebrowPackage, SlotCategory.Eyebrow);
                targetEmoteIndex.eyesPart = PartList.Static.FindPart(edata.eyesPartName, edata.eyesPackage, SlotCategory.Eyes);
                targetEmoteIndex.nosePart = PartList.Static.FindPart(edata.nosePartName, edata.nosePackage, SlotCategory.Nose);
                targetEmoteIndex.mouthPart = PartList.Static.FindPart(edata.mouthPartName, edata.mouthPackage, SlotCategory.Mouth);
                targetEmoteIndex.earPart = PartList.Static.FindPart(edata.earPartName, edata.earPackage, SlotCategory.Ear);
                return true;
            }

            public bool SetSegmentScales(CharacterViewer character, List<SegmentScaleData> bodySegmentDatas)
            {
                if (!character)
                {
                    return false;
                }
                //..update
                if (bodySegmentDatas == null || bodySegmentDatas.Count <= 0)
                {
                    foreach (SegmentType st in Enum.GetValues(typeof(SegmentType)))
                    {
                        character.SetBodySlider(st, new Vector2(0.5f, 0.5f));
                    }
                }
                else
                {
                    foreach (SegmentScaleData sd in bodySegmentDatas)
                    {
                        character.SetBodySlider(sd.segmentType, sd.scale);
                    }
                }
                //update..
                return true;
            }
        }
    }
}
