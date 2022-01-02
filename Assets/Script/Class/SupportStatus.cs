using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SupportStatus
{
    public string name;

    public EnumCustom.Status status;

    public EnumCustom.SpecialEffect effect;

    public float valuePerLevel;

    public List<AttributeStatusPerLevel> attributeStatusPerLevels = new List<AttributeStatusPerLevel>();

    public void StartRound(CreatureController controller, Skill skill)
    {
        if (effect == EnumCustom.SpecialEffect.None)
        {
            if (attributeStatusPerLevels.Count <= 0)
            {
                float value = 0.1f + (valuePerLevel * skill.level);
                controller.attributeStatus.AddModifier(null, new StatusModifier() { status = status, count = 150, spellName = name, value = Mathf.FloorToInt(value) });
            }
            else
            {
                ItemInventory itemAux = Manager.Instance.characterController.CharacterInventory.equipements.Find(n => n.item.GetType() == typeof(ChestEquipmentSO));

                int hp = 0;
                int mp = 0;
                if (itemAux != null)
                {
                    if (itemAux.equiped == true && (itemAux.item as ChestEquipmentSO).skill.skill.support.Contains(this))
                    {
                        List<StatusModifier> statusModifiersList = new List<StatusModifier>();
                        List<AttributeModifier> attributeModifiersList = new List<AttributeModifier>();
                        for (int i = 0; i < skill.level; i++)
                        {

                            if (attributeStatusPerLevels[i].status.Count > 0)
                            {
                                foreach (var aux in attributeStatusPerLevels[i].status)
                                {
                                    statusModifiersList.Add(new StatusModifier() { status = aux.status, count = 150, spellName = name, value = Mathf.FloorToInt(aux.value), level = i });
                                }
                            }
                            else if (attributeStatusPerLevels[i].attributes.Count > 0)
                            {
                                foreach (var aux in attributeStatusPerLevels[i].attributes)
                                {

                                    attributeModifiersList.Add(new AttributeModifier() { attribute = aux.attribute, count = 150, spellName = name, value = Mathf.FloorToInt(aux.value), level = i });
                                }
                            }
                            hp += attributeStatusPerLevels[i].hp;
                            mp += attributeStatusPerLevels[i].mp;
                        }
                        controller.attributeStatus.AddUniqueModifier(attributeModifiersList, statusModifiersList, name);

                        controller.attributeStatus.hpExtraSuportSkillEquipment = hp;
                        controller.attributeStatus.mpExtraSuportSkillEquipment = mp;
                    }
                    else if (itemAux.equiped == false)
                    {
                        controller.attributeStatus.AddUniqueModifier(new List<AttributeModifier>(), new List<StatusModifier>(), name);
                        controller.attributeStatus.hpExtraSuportSkillEquipment = hp;
                        controller.attributeStatus.mpExtraSuportSkillEquipment = mp;
                    }
                }
                else
                {
                    controller.attributeStatus.hpExtraSuportSkillEquipment = hp;
                    controller.attributeStatus.mpExtraSuportSkillEquipment = mp;
                }
            }
        }
    }

    public void IncreaseLevel(CreatureController controller, Skill skill)
    {
        StartRound(controller, skill);
        if (effect == EnumCustom.SpecialEffect.Aggro)
        {
            float nAggro = valuePerLevel * skill.level;
            if (nAggro % 1 == 0)
            {
                controller.Aggro -= 1;
            }
        }
    }
}
