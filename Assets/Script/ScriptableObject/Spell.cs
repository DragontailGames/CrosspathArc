using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[CreateAssetMenu(menuName = "Arc/Spell")]
public class Spell : ScriptableObject
{
    public string spellName;

    public string description;

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

    public int invokeLimit = 0;

    public List<SubSpell> subSpell = new List<SubSpell>();

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

    public void Cast(UnityAction action, CreatureController caster, CreatureController target, Vector3Int tile, List<MinionCount> minionCounts)
    {
        int hitChance = caster.attributeStatus.GetValue(EnumCustom.Status.SpellHit);
        int intAttribute = caster.attributeStatus.GetValue(EnumCustom.Attribute.Int);

        int spellDamage = GetValue(caster);

        string extraDamage = "";
        int damage = spellDamage;

        if (attributeInfluence.Count > 0)
        {
            extraDamage = " + ";
        }

        foreach (var aux in attributeInfluence)
        {
            var auxAttribute = aux.GetValue(caster);
            extraDamage += auxAttribute;
            damage += auxAttribute;
        }
        string textDamage = "(" + spellDamage + extraDamage + ")";

        if (castTarget == EnumCustom.CastTarget.Area)
        {
            if (tile == new Vector3Int() && target != null)
            {
                tile = target.currentTileIndex;
            }
            CastAreaSpell(hitChance, intAttribute, tile, damage, textDamage,caster, action);
        }
        else if (castTarget == EnumCustom.CastTarget.Tile)
        {
            CastSpellInTile(minionCounts, tile, caster, action);
        }
        else if (castTarget == EnumCustom.CastTarget.Enemy)
        {
            if (target != null && target.GetType() == typeof(EnemyController))
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
            CastProjectileSpell(100, intAttribute, caster ,target, damage, textDamage,action);
        }
    }

    public void CastBuff(CharacterController controller)
    {
        if (spellType == EnumCustom.SpellType.Buff)
        {
            foreach (var aux in this.buffDebuff)
            {
                if (aux.buffDebuffType == EnumCustom.BuffDebuffType.Attribute)
                {
                    controller.attributeStatus.AddModifier(new AttributeModifier()
                    {
                        spellName = spellName,
                        attribute = aux.attribute,
                        count = aux.turnDuration,
                        value = aux.value
                    }, null);
                }
                if (aux.buffDebuffType == EnumCustom.BuffDebuffType.Status)
                {
                    controller.attributeStatus.AddModifier(null, new StatusModifier()
                    {
                        spellName = spellName,
                        status = aux.status,
                        count = aux.turnDuration,
                        value = aux.value
                    });
                }
                GameObject objectSpell = Instantiate(spellCastObject, controller.transform);
                Destroy(objectSpell, 1.0f);
            }
        }

        controller.Mp -= manaCost;
    }

    public void CastSpecial(CreatureController controller, CreatureController attacker)
    {
        ParserCustom.SpellSpecialParser(new SpecialSpell(duration, GetValue(attacker), controller, specialEffect));
    }

    private void CastProjectileSpell(int hitChance, int intAttribute, CreatureController creature, CreatureController target, int damage, string textDamage, UnityAction action)
    {
        bool hit = Combat.TryHit(hitChance, intAttribute, target.attributeStatus.GetValue(EnumCustom.Status.SpellDodge), target.nickname);
        if (!hit)
        {
            action?.Invoke();
            return;
        }

        UnityAction extraEffect = null;

        //Cria a spell e configura para a animação
        AnimateCastProjectileSpell(target.transform.position, creature.transform, () => {
            target.ReceiveSpell(creature, damage, textDamage, this);
            foreach (var aux in subSpell)
            {
                aux.Cast(creature, target);
            }
            extraEffect?.Invoke();
            action?.Invoke();
        });
    }

    public void CastSpellInTile(List<MinionCount> minionCounts, Vector3Int index, CreatureController caster, UnityAction action)
    {
        if (spellType == EnumCustom.SpellType.Invoke)
        {
            GameObject creature = InvokeCreature(Manager.Instance.gameManager.tilemap.CellToLocal(index));
            if (minionCounts.Find(n => n.spell == this) != null)
            {
                if (minionCounts.Find(n => n.spell == this).creatures.Count >= invokeLimit)
                {
                    List<GameObject> orderedCreature = minionCounts.Find(n => n.spell == this).creatures.OrderBy(n => n.GetComponent<MinionController>().duration).ToList();
                    orderedCreature[0].GetComponent<MinionController>().Defeat();
                }
                minionCounts.Find(n => n.spell == this).creatures.Add(creature);
            }
            else
            {
                minionCounts.Add(new MinionCount()
                {
                    spell = this,
                    creatures = new List<GameObject>()
                {
                    creature
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
                if (!Manager.Instance.gameManager.elevationTM.HasTile(new Vector3Int(points[i].x + 1, points[i].y + 1, 0)))
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

        action?.Invoke();
    }

    private void CastAreaSpell(int hitChance, int intAttribute, Vector3Int startIndex, int damage, string textDamage, CreatureController controller,UnityAction action)
    {
        List<Vector3Int> tiles = new List<Vector3Int>();

        int endX = Mathf.FloorToInt(area / 2) - ((area % 2 == 0) ? 1 : 0);
        int endY = Mathf.FloorToInt(area / 2) - ((area % 2 == 0) ? 1 : 0);

        for (int x = startIndex.x - Mathf.FloorToInt(area / 2); x <= startIndex.x + endX; x++)
        {
            for (int y = startIndex.y - Mathf.FloorToInt(area / 2); y <= startIndex.y + endY; y++)
            {
                Vector3Int t = new Vector3Int(x, y, 0);
                if (x >= 0 && y >= 0 && Manager.Instance.gameManager.tilemap.HasTile(t))
                {
                    tiles.Add(t);
                }
            }
        }

        foreach (var aux in tiles)
        {
            AnimateCastAreaSpell(Manager.Instance.gameManager.tilemap.CellToLocal(aux), aux);
            if (spellType != EnumCustom.SpellType.Area_Hazard)
            {
                EnemyController enemy = Manager.Instance.enemyManager.CheckEnemyInTile(aux);

                if (enemy != null)
                {
                    bool hit = Combat.TryHit(hitChance, intAttribute, enemy.attributeStatus.GetValue(EnumCustom.Status.SpellDodge), enemy.nickname);
                    if (hit)
                    {
                        enemy.ReceiveSpell(controller, damage, textDamage, this);
                    }
                }
            }

        }
        action?.Invoke();
    }

    /// <summary>
    /// Animação da spell
    /// </summary>
    /// <param name="targetPos">posição do inimigo</param>
    /// <param name="character"></param>
    /// <param name="hitAction">Ação apos o hit</param>
    /// <returns></returns>
    public void AnimateCastProjectileSpell(Vector3 targetPos, Transform character, UnityAction hitAction)
    {
        //Corrige a rotação da spell
        Vector3 diff = targetPos - character.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        GameObject spellCreated = Instantiate(spellCastObject, character.position + Vector3.up * 0.5f, Quaternion.Euler(0f, 0f, rot_z - 180));
        spellCreated.gameObject.AddComponent(typeof(SpellProjectileController));
        spellCreated.GetComponent<SpellProjectileController>().StartHit(targetPos, hitAction);
    }

    public void AnimateCastAreaSpell(Vector3 position, Vector3Int index)
    {
        GameObject spellCreated = Instantiate(spellCastObject, position + Vector3.up * 0.25f, Quaternion.identity);
        spellCreated.transform.rotation = Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0, 360)));
        if(spellType == EnumCustom.SpellType.Area_Hazard)
        {
            SpellAreaHazard spellAreaHazard = spellCreated.GetComponent<SpellAreaHazard>();
            spellAreaHazard.duration = duration;
            spellAreaHazard.damage = fixedValue != 0 ? fixedValue : UnityEngine.Random.Range(minValue, maxValue);
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
}
