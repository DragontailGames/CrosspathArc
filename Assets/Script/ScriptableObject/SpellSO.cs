﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Arc/Spell")]
public class SpellSO : ScriptableObject
{
    public string spellName;

    public string description;

    public string spellLogName;

    public Sprite icon;

    public EnumCustom.CastTarget castTarget;

    public int availableAt = 1;

    public int minValue;

    public int maxValue;

    public int fixedValue = 0;

    public int manaCost;

    public EnumCustom.SpellType spellType;

    public int area;

    public List<BuffDebuff> buffDebuff;

    public List<AttributeInfluence> attributeInfluence;

    public GameObject spellCastObject;

    public EnumCustom.SpecialEffect specialEffect;

    public int duration = 0;

    public AttributeInfluence attributeInfluenceDuration;

    public int invokeLimit = 0;

    public List<SubSpell> subSpell = new List<SubSpell>();

    public bool onlyMinions = false;

    public EnumCustom.CostType costType = EnumCustom.CostType.Mana;

    public int cooldownTurns = 0;

    public List<AttributeInfluence> attributeInfluenceCooldownTurns;

    public int probabilityToCast = 100;

    public bool melee = false;

    public int multipleHit = 0;

    public EnumCustom.Status mainStatusToTryHit = EnumCustom.Status.SpellHit;

    public EnumCustom.CastEffect castEffect = EnumCustom.CastEffect.Player;

    public int castAfterTurns = 0;

    public string unlockWhenKillThis = "";

    public int GetValue(CreatureController creatureController)
    {
        int value = 0;

        if(fixedValue == 0)
            value = UnityEngine.Random.Range(minValue, maxValue + 1);
        else
            value = fixedValue;

        foreach (var aux in attributeInfluence)
        {
            var auxAttribute = aux.GetValue(creatureController);
            value += auxAttribute;
        }

        return value;
    }

    public void Cast(UnityAction action, CreatureController caster, CreatureController target, Vector3Int tile, List<CharacterMinions> minionCounts, bool onlyCast = false)
    {
        int hitChance = caster.attributeStatus.GetValue(mainStatusToTryHit);
        int intAttribute = caster.attributeStatus.GetValue(EnumCustom.Attribute.Int);

        int spellDamage = GetValue(caster);

        string extraDamage = "";
        int damage = spellDamage;

        if (attributeInfluence.Count > 0)
        {
            extraDamage = " + ";
            int extraDmg = 0;

            foreach (var aux in attributeInfluence)
            {
                var auxAttribute = aux.GetValue(caster);
                extraDmg += auxAttribute;
                //damage += auxAttribute;
                spellDamage -= auxAttribute;
            }

            extraDamage += extraDmg;
        }

        if (castTarget == EnumCustom.CastTarget.Area_Hazard)
        {
            onlyCast = true;
        }

        string textDamage = "(" + (spellDamage + extraDamage) + ")";

        if (onlyCast == false)
        {
            if (castAfterTurns > 0)
            {
                UnityAction tempAction = action;
                new SpellAfterDelay(castAfterTurns, () => this.Cast(tempAction, caster, target, tile, minionCounts, true), caster.GetComponent<CharacterCombat>());

                Manager.Instance.canvasManager.UpdateStatus();

                action += () => { CastSubspells(caster, target); };
                action?.Invoke();

                return;
            }
            if (multipleHit > 0)
            {
                for (int i = 0; i < multipleHit - 1; i++)
                {
                    if(target.Hp>0)
                    {
                        this.Cast(null, caster, target, tile, minionCounts, true);
                    }
                }
            }
            action += () => { CastSubspells(caster, target); };
        }


        if (castTarget == EnumCustom.CastTarget.Area || castTarget == EnumCustom.CastTarget.Area_Hazard)
        {
            if (tile == new Vector3Int() && target != null)
            {
                tile = target.currentTileIndex;
            }
            CastAreaSpell(hitChance, intAttribute, tile, damage, textDamage, caster, action);
        }
        else if (castTarget == EnumCustom.CastTarget.Tile)
        {
            CastSpellInTile(minionCounts, tile, caster, action);
        }
        else if (castTarget == EnumCustom.CastTarget.Enemy)
        {
            if (target != null && CheckIsEnemy(caster, target))
            {
                CastProjectileSpell(hitChance, intAttribute, caster, target, damage, textDamage, action);
            }
            else
            {
                action?.Invoke();
            }
        }
        else if (castTarget == EnumCustom.CastTarget.Target)
        {
            CastProjectileSpell(100, intAttribute, caster, target, damage, textDamage, action);
        }
        else if (castTarget == EnumCustom.CastTarget.None)
        {
            if (this.spellType == EnumCustom.SpellType.Buff)
            {
                this.CastBuffDebuff(caster);
                foreach (var aux in caster.specialSpell)
                {
                    aux.HandleAttack(caster);
                }
                CastSubspells(caster, target);
                Manager.Instance.canvasManager.UpdateStatus();
            }
            else if (this.spellType == EnumCustom.SpellType.Debuff)
            {
                this.CastBuffDebuff(caster);
                foreach (var aux in caster.specialSpell)
                {
                    aux.HandleAttack(caster);
                }
                CastSubspells(caster, target);
                Manager.Instance.canvasManager.UpdateStatus();
            }
            else if (this.spellType == EnumCustom.SpellType.Special)
            {
                if (this.onlyMinions)
                {
                    foreach (var minion in minionCounts)
                    {
                        foreach (var minionCreature in minion.creatures)
                        {
                            this.CastSpecial(minionCreature, caster, action, tile);
                        }
                    }
                }
                else
                {
                    if (area > 0)
                    {
                        tile = caster.currentTileIndex;
                        CastAreaSpell(hitChance, intAttribute, tile, damage, textDamage, caster, action);
                    }
                    else
                    {
                        this.CastSpecial(caster, caster, action, tile);
                    }
                }

                Manager.Instance.canvasManager.UpdateStatus();
            }
            else if(this.spellType == EnumCustom.SpellType.Hit)
            {
                if(area>0)
                {
                    tile = caster.currentTileIndex;
                    CastAreaSpell(hitChance, intAttribute, tile, damage, textDamage, caster, action);
                }
            }
            else {
                Manager.Instance.canvasManager.UpdateStatus();
                action?.Invoke();
            }
            
        }
    }

    public IEnumerator CastAfterDelay(UnityAction action, int count)
    {
        yield return new WaitForSeconds(0.1f * count);
        action?.Invoke();
    }

    public bool CheckIsEnemy(CreatureController caster, CreatureController target)
    {
        bool isEnemy = false;

        if(caster.GetType() == typeof(EnemyController) && target.GetType() != typeof(EnemyController) ||
           caster.GetType() == typeof(CharacterController) || caster.GetType() == typeof(MinionController) && target.GetType() == typeof(EnemyController))
        {
            isEnemy = true;
        }

        return isEnemy;
    }

    public void CastBuffDebuff(CreatureController controller)
    {
        if (spellType == EnumCustom.SpellType.Buff)
        {
            foreach (var aux in this.buffDebuff)
            {
                if (aux.minionModifier)
                {
                    var minions = (controller as CharacterController).CharacterCombat.minionCounts;
                    foreach (var minion in minions)
                    {
                        foreach(var creatures in minion.creatures)
                        {
                            GameObject minionSpell = Instantiate(spellCastObject, creatures.transform);
                            Destroy(minionSpell, 1.0f);
                            ApplyBuff(creatures, aux, controller);
                        }
                    }

                    GameObject objectSpell = Instantiate(spellCastObject, controller.transform);
                    Destroy(objectSpell, 1.0f);
                }
                else
                {
                    ApplyBuff(controller, aux);
                    GameObject objectSpell = Instantiate(spellCastObject, controller.transform);
                    Destroy(objectSpell, 1.0f);
                }
            }
        }
        if (spellType == EnumCustom.SpellType.Debuff)
        {
            foreach (var aux in this.buffDebuff)
            {
                if (aux.minionModifier)
                {
                    var minions = (controller as CharacterController).CharacterCombat.minionCounts;
                    foreach (var minion in minions)
                    {
                        foreach (var creatures in minion.creatures)
                        {
                            GameObject minionSpell = Instantiate(spellCastObject, creatures.transform);
                            Destroy(minionSpell, 1.0f);
                            ApplyDebuff(creatures, aux, controller);
                        }
                    }

                    GameObject objectSpell = Instantiate(spellCastObject, controller.transform);
                    Destroy(objectSpell, 1.0f);
                }
                else
                {
                    ApplyDebuff(controller, aux);
                    GameObject objectSpell = Instantiate(spellCastObject, controller.transform);
                    Destroy(objectSpell, 1.0f);
                }
            }
        }
    }

    public void ApplyBuff(CreatureController controller, BuffDebuff buffDebuff, CreatureController caster = null)
    {
        if (buffDebuff.buffDebuffType == EnumCustom.BuffDebuffType.Attribute)
        {
            controller.attributeStatus.AddModifier(new AttributeModifier()
            {
                spellName = spellLogName == "" ? spellName : spellLogName,
                attribute = buffDebuff.attribute,
                count = buffDebuff.turnDuration,
                value = buffDebuff.value + buffDebuff.attributeInfluence.GetValue(caster != null ? caster : controller)
            }, null);
        }
        if (buffDebuff.buffDebuffType == EnumCustom.BuffDebuffType.Status)
        {
            controller.attributeStatus.AddModifier(null, new StatusModifier()
            {
                spellName = spellLogName == "" ? spellName : spellLogName,
                status = buffDebuff.status,
                count = buffDebuff.turnDuration,
                value = buffDebuff.value + buffDebuff.attributeInfluence.GetValue(caster != null ? caster : controller)
            });
        }
    }

    public void ApplyDebuff(CreatureController controller, BuffDebuff buffDebuff, CreatureController caster = null)
    {
        if (buffDebuff.buffDebuffType == EnumCustom.BuffDebuffType.Attribute)
        {
            controller.attributeStatus.AddModifier(new AttributeModifier()
            {
                spellName = spellLogName == "" ? spellName : spellLogName,
                attribute = buffDebuff.attribute,
                count = buffDebuff.turnDuration,
                value = -(buffDebuff.value + buffDebuff.attributeInfluence.GetValue(caster != null ? caster : controller))
            }, null);
        }
        if (buffDebuff.buffDebuffType == EnumCustom.BuffDebuffType.Status)
        {
            controller.attributeStatus.AddModifier(null, new StatusModifier()
            {
                spellName = spellLogName == "" ? spellName : spellLogName,
                status = buffDebuff.status,
                count = buffDebuff.turnDuration,
                value = -(buffDebuff.value + buffDebuff.attributeInfluence.GetValue(caster != null ? caster : controller))
            });
        }
    }

    public void CastSpecial(CreatureController target, CreatureController caster, UnityAction action, Vector3Int tile)
    {
        ParserCustom.SpellSpecialParser(new SpecialSpell(duration + attributeInfluenceDuration.GetValue(caster), GetValue(caster), caster, target, tile, specialEffect, spellLogName));
        GameObject objectSpell = Instantiate(spellCastObject, target.transform);
        action?.Invoke();
        Destroy(objectSpell, 1.0f);
    }

    public void CastProjectileSpell(int hitChance, int intAttribute, CreatureController caster, CreatureController target, int damage, string textDamage, UnityAction action)
    {
        bool hit = Combat.TryHit(hitChance, intAttribute, target.attributeStatus.GetValue(EnumCustom.Status.SpellDodge), target.nickname);
        if (!hit)
        {
            action?.Invoke();
            return;
        }

        UnityAction extraEffect = null;

        //Cria a spell e configura para a animação
        AnimateCastProjectileSpell(target.transform.position, caster.transform, () => {
            target.ReceiveSpell(caster, damage, textDamage, this);
            extraEffect?.Invoke();
            action?.Invoke();
        }, caster);
    }

    public void CastSpellInTile(List<CharacterMinions> minionCounts, Vector3Int index, CreatureController caster, UnityAction action)
    {
        if (spellType == EnumCustom.SpellType.Invoke)
        {
            GameObject creature = InvokeCreature(Manager.Instance.gameManager.tilemap.CellToLocal(index));
            if (minionCounts.Find(n => n.spell == this) != null)
            {
                if (minionCounts.Find(n => n.spell == this).creatures.Count >= invokeLimit)
                {
                    List<MinionController> orderedCreature = minionCounts.Find(n => n.spell == this).creatures.OrderBy(n => n.duration).ToList();
                    orderedCreature[0].GetComponent<MinionController>().Defeat();
                }
                minionCounts.Find(n => n.spell == this).creatures.Add(creature.GetComponent<MinionController>());
            }
            else
            {
                minionCounts.Add(new CharacterMinions()
                {
                    spell = this,
                    creatures = new List<MinionController>()
                    {
                        creature.GetComponent<MinionController>()
                    }
                });
            }
        }
        else if (spellType == EnumCustom.SpellType.Blink)
        {
            var points = Manager.Instance.gameManager.GetPath(caster.currentTileIndex, index);
            PathFind.Point point = null;
            for (int i = 0; i < (points.Count > 5 ? 5 : points.Count - 1); i++)
            {
                
                if (Manager.Instance.gameManager.HasAvailableTile(new Vector3Int(points[i].x, points[i].y, 0)))
                {
                    point = points[i];
                }
                else
                {
                    break;
                }
            }
            if (point != null)
            {
                Vector3Int pos = new Vector3Int(point.x, point.y, 0);
                caster.GetComponent<CharacterMoveTileIsometric>().Blink(pos);
            }
        }
        else if(spellType == EnumCustom.SpellType.Special)
        {
            GameObject spellObject = Instantiate(spellCastObject);
            ParserCustom.SpellSpecialParser(new SpecialSpell(duration + attributeInfluenceDuration.GetValue(caster), GetValue(caster), caster, caster, index, specialEffect, spellLogName, spellObject));
        }

        action?.Invoke();
    }

    private void CastAreaSpell(int hitChance, int intAttribute, Vector3Int startIndex, int damage, string textDamage, CreatureController caster,UnityAction action)
    {
        List<Vector3Int> tiles = new List<Vector3Int>();

        int endX = Mathf.FloorToInt(area / 2) - ((area % 2 == 0) ? 1 : 0);
        int endY = Mathf.FloorToInt(area / 2) - ((area % 2 == 0) ? 1 : 0);

        for (int x = startIndex.x - Mathf.FloorToInt(area / 2); x <= startIndex.x + endX; x++)
        {
            for (int y = startIndex.y - Mathf.FloorToInt(area / 2); y <= startIndex.y + endY; y++)
            {
                Vector3Int t = new Vector3Int(x, y, 0);
                if (x >= 0 && y >= 0 && Manager.Instance.gameManager.HasAvailableTile(t))
                {
                    tiles.Add(t);
                }
            }
        }

        foreach (var aux in tiles)
        {
            AnimateCastAreaSpell(Manager.Instance.gameManager.tilemap.CellToLocal(aux), aux, caster);
            if (castTarget != EnumCustom.CastTarget.Area_Hazard)
            {
                CreatureController creature = Manager.Instance.gameManager.creatures.Find(n => n.currentTileIndex == aux);

                if (creature != null && CheckEnemyType(caster, creature))
                {
                    if (spellType == EnumCustom.SpellType.Special)
                    {
                        ParserCustom.SpellSpecialParser(new SpecialSpell(duration + attributeInfluenceDuration.GetValue(caster), GetValue(caster), caster, creature, aux, specialEffect, spellLogName));
                    }
                    else
                    {
                        bool hit = Combat.TryHit(hitChance, intAttribute, creature.attributeStatus.GetValue(EnumCustom.Status.SpellDodge), creature.nickname);
                        if (hit)
                        {
                            creature.ReceiveSpell(caster, damage, textDamage, this);
                        }
                    }
                }
            }
        }
        action?.Invoke();
    }

    public bool CheckEnemyType(CreatureController caster, CreatureController target)
    {
        if (caster.GetType() == typeof(CharacterController) && target.GetType() == typeof(MinionController) || caster.GetType() == target.GetType())
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Animação da spell
    /// </summary>
    /// <param name="targetPos">posição do inimigo</param>
    /// <param name="character"></param>
    /// <param name="hitAction">Ação apos o hit</param>
    /// <returns></returns>
    public void AnimateCastProjectileSpell(Vector3 targetPos, Transform character, UnityAction hitAction, CreatureController caster)
    {
        //Corrige a rotação da spell
        Vector3 diff = targetPos - character.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        Vector3 startPos = character.position + Vector3.up * 0.5f;
        startPos += CalculateOffset(caster);

        GameObject spellCreated = Instantiate(spellCastObject, startPos, Quaternion.Euler(0f, 0f, rot_z - 180));
        spellCreated.gameObject.AddComponent(typeof(SpellProjectileController));
        spellCreated.GetComponent<SpellProjectileController>().StartHit(targetPos, hitAction);
    }

    public Vector3 CalculateOffset(CreatureController caster)
    {
        Vector3 returnValue = new Vector3();
        if(caster.direction.Contains("N"))
        {
            returnValue.x -= caster.offsetSpell.x;
            returnValue.y += caster.offsetSpell.y;
        }
        if (caster.direction.Contains("S"))
        {
            returnValue.x += caster.offsetSpell.x;
            returnValue.y -= caster.offsetSpell.y;
        }
        if (caster.direction.Contains("E"))
        {
            returnValue.y += caster.offsetSpell.x;
            returnValue.x += caster.offsetSpell.y;
        }
        if (caster.direction.Contains("W"))
        {
            returnValue.y += caster.offsetSpell.x;
            returnValue.x -= caster.offsetSpell.y;
        }

        return returnValue;
    }

    public void AnimateCastAreaSpell(Vector3 position, Vector3Int index, CreatureController caster)
    {
        GameObject spellCreated = Instantiate(spellCastObject, position + Vector3.up * 0.25f, Quaternion.Euler(Vector3.zero));
        //spellCreated.transform.rotation = Quaternion.Euler(new Vector3(spellCreated.transform.rotation.x, spellCreated.transform.rotation.y, UnityEngine.Random.Range(0, 360)));
        if(castTarget == EnumCustom.CastTarget.Area_Hazard)
        {
            SpellAreaHazard spellAreaHazard = spellCreated.GetComponent<SpellAreaHazard>();
            if(spellAreaHazard == null)
            {
                spellCreated.AddComponent<SpellAreaHazard>();
                spellAreaHazard = spellCreated.GetComponent<SpellAreaHazard>();
            }
            spellAreaHazard.duration = duration + attributeInfluenceDuration.GetValue(caster);
            spellAreaHazard.spellName = spellLogName == "" ? spellName : spellLogName;
            spellAreaHazard.castEffect = castEffect;
            spellAreaHazard.spellType = spellType;
            spellAreaHazard.buffDebuff = buffDebuff;
            spellAreaHazard.caster = caster;
            spellAreaHazard.spell = this;
            spellAreaHazard.specialEffect = specialEffect;
            spellAreaHazard.value = fixedValue != 0 ? fixedValue : UnityEngine.Random.Range(minValue, maxValue) + GetValue(caster);
            spellAreaHazard.position = index;
        }
        else
        {
            Destroy(spellCreated, 1.2f);
        }
    }

    public GameObject InvokeCreature(Vector3 position)
    {
        GameObject spellCreated = Instantiate(spellCastObject, position + Vector3.up * 0.25f, Quaternion.identity);
        spellCreated.GetComponent<MinionController>().duration = duration;
        return spellCreated;
    }
    
    public void CureDesease(CreatureController creatureController)
    {
        if(specialEffect != EnumCustom.SpecialEffect.None)
        {
            if (specialEffect != EnumCustom.SpecialEffect.All)
            {
                creatureController.specialSpell.Find(n => n.effect == specialEffect).EndOfDuration(creatureController);
            }
            else
            {
                var auxSpecialSpell = creatureController.specialSpell.FindAll(n => n.effect == EnumCustom.SpecialEffect.Cannot_Walk || 
                n.effect == EnumCustom.SpecialEffect.Paralyze || 
                n.effect == EnumCustom.SpecialEffect.Poison || 
                n.effect == EnumCustom.SpecialEffect.Sleep);
                auxSpecialSpell[UnityEngine.Random.Range(0, auxSpecialSpell.Count)].EndOfDuration(creatureController);
            }
        }
        foreach(var bfAux in buffDebuff)
        {
            if (bfAux.value < 0)
            {
                if (bfAux.buffDebuffType == EnumCustom.BuffDebuffType.Status)
                {
                    var statusToRemove = creatureController.attributeStatus.statusModifiers.Find(n => n.status == bfAux.status);    
                    Manager.Instance.canvasManager.RemoveLogText(statusToRemove.spellName);
                    creatureController.attributeStatus.statusModifiers.Remove(statusToRemove);
                }
                if(bfAux.buffDebuffType == EnumCustom.BuffDebuffType.Attribute)
                {
                    var attribute = creatureController.attributeStatus.attributeModifiers.Find(n => n.attribute == bfAux.attribute);
                    Manager.Instance.canvasManager.RemoveLogText(attribute.spellName);
                    creatureController.attributeStatus.attributeModifiers.Remove(attribute);
                }
            }
        }
    }

    public void CastSubspells(CreatureController caster, CreatureController target)
    {
        foreach (var aux in subSpell)
        {
            aux.Cast(caster, target, this);
            Manager.Instance.canvasManager.UpdateStatus();
        }
    }
}
