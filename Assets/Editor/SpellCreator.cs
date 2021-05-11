using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpellCreator : EditorWindow
{
    [MenuItem("Arc/Spell Creator")]
    private static void ShowWindow()
    {
        SpellCreator window = (SpellCreator)GetWindow(typeof(SpellCreator));
        window.titleContent = new GUIContent("Spell Creator");
        window.minSize = new Vector2(400, 400);
        window.Show();
    }

    Spell spell;

    Spell[] spells;

    string folder;

    private void OnEnable()
    {
        CreateSpell();
    }

    private void CreateSpell()
    {
        spell = CreateInstance("Spell") as Spell;
        folder = "Wizard";
        spell.buffDebuff = new List<BuffDebuff>();
        spell.attributeInfluence = new List<EnumCustom.Attribute>();
        spells = Resources.LoadAll<Spell>("ScriptableObject/Spells/");
    }

    void OnGUI()
    {
        EditorGUIUtility.labelWidth = 130;
        GUIContent content = new GUIContent();

        content.text = "Spell Folder";
        folder = EditorGUILayout.TextField(content, folder);
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        spell.icon = EditorGUILayout.ObjectField(spell.icon, typeof(Sprite), true, GUILayout.Height(60), GUILayout.Width(60)) as Sprite;

        EditorGUILayout.BeginVertical();

        content.text = "Spell Name";
        spell.spellName = EditorGUILayout.TextField(content, spell.spellName);

        content.text = "Description";
        spell.description = EditorGUILayout.TextField(content, spell.description, GUILayout.Height(40));

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        content.text = "Create Object";
        spell.spellCastObject = EditorGUILayout.ObjectField(content, spell.spellCastObject, typeof(GameObject), true) as GameObject;

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        content.text = "Available At";
        spell.availableAt = EditorGUILayout.IntSlider(content, spell.availableAt, 1, 10);

        content.text = "Mana Cost";
        spell.manaCost = EditorGUILayout.IntField(content, spell.manaCost);

        content.text = "Cast Target";
        spell.castTarget = (EnumCustom.CastTarget)EditorGUILayout.EnumPopup(content, spell.castTarget);

        if (spell.castTarget == EnumCustom.CastTarget.Area)
        {
            content.text = "Area";
            spell.area = EditorGUILayout.IntField(content, spell.area);
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        RenderSpecialType();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if(GUILayout.Button("Create Spell"))
        {
            if (!string.IsNullOrEmpty(folder))
                folder = folder + "/";

            ProjectWindowUtil.CreateAsset(spell, $"Assets/Resources/ScriptableObject/Spells/{folder}{spell.spellName}.asset");
            CreateSpell();
        }
    }

    public void RenderSpecialType()
    {
        GUIContent content = new GUIContent();

        content.text = "Spell Type";
        spell.spellType = (EnumCustom.SpellType)EditorGUILayout.EnumPopup(content, spell.spellType);

        if(spell.spellType == EnumCustom.SpellType.Hit)
        {
            RenderDamage();
        }
        else if(spell.spellType == EnumCustom.SpellType.Buff || spell.spellType == EnumCustom.SpellType.Debuff)
        {
            RenderBuffDebuff();
        }
        else if(spell.spellType == EnumCustom.SpellType.Special)
        {
            RenderSpecial();
        }
    }

    public void RenderDamage()
    {
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Damage");
        GUIContent content = new GUIContent();

        EditorGUILayout.BeginHorizontal();

        content.text = "Min";
        spell.minValue = EditorGUILayout.IntField(content, spell.minValue);
        content.text = "Max";
        spell.maxValue = EditorGUILayout.IntField(content, spell.maxValue);

        EditorGUILayout.EndHorizontal();

        content.text = "Fixed";
        spell.fixedValue = EditorGUILayout.IntField(content, spell.fixedValue);
        
        if(spell.attributeInfluence.Count>0)
        {
            EditorGUILayout.LabelField("Attribute Influence");
        }

        for (int i = 0; i < spell.attributeInfluence.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            spell.attributeInfluence[i] = (EnumCustom.Attribute)EditorGUILayout.EnumPopup(spell.attributeInfluence[i]);

            if(GUILayout.Button("Remove"))
            {
                spell.attributeInfluence.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if(GUILayout.Button("Add Attribute Influece"))
        {
            spell.attributeInfluence.Add(new EnumCustom.Attribute());
        }
    }

    public void RenderBuffDebuff()
    {
        GUIContent content = new GUIContent();

        EditorGUILayout.Space(5);

        foreach (var spellBD in spell.buffDebuff.ToArray())
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space(2);

            content.text = "Buff_Debuff Type";
            spellBD.buffDebuffType = (EnumCustom.BuffDebuffType)EditorGUILayout.EnumPopup(content, spellBD.buffDebuffType);

            content.text = spellBD.buffDebuffType.ToString();

            if (spellBD.buffDebuffType == EnumCustom.BuffDebuffType.Attribute)
                spellBD.attribute = (EnumCustom.Attribute)EditorGUILayout.EnumPopup(content, spellBD.attribute);

            if (spellBD.buffDebuffType == EnumCustom.BuffDebuffType.Status)
                spellBD.status = (EnumCustom.Status)EditorGUILayout.EnumPopup(content, spellBD.status);

            if (spellBD.buffDebuffType == EnumCustom.BuffDebuffType.Special)
                spellBD.specialEffect = (EnumCustom.SpecialEffect)EditorGUILayout.EnumPopup(content, spellBD.specialEffect);

            content.text = "Value";
            spellBD.value = EditorGUILayout.IntField(content, spellBD.value);

            content.text = "Duration";
            spellBD.turnDuration = EditorGUILayout.IntField(content, spellBD.turnDuration);

            if (GUILayout.Button("Remove"))
            {
                spell.buffDebuff.Remove(spellBD);
            }

            EditorGUILayout.Space(2);
            GUILayout.EndVertical();

        }

        EditorGUILayout.Space(5);

        if (GUILayout.Button("Add Buff/Debuff"))
        {
            spell.buffDebuff.Add(new BuffDebuff());
        }
    }

    public void RenderSpecial()
    {
        GUIContent content = new GUIContent();

        EditorGUILayout.Space(5);

        content.text = "Special Effect";
        spell.specialEffect = (EnumCustom.SpecialEffect)EditorGUILayout.EnumPopup(content, spell.specialEffect);

        content.text = "Duration";
        spell.specialEffectDuration = EditorGUILayout.IntField(content, spell.specialEffectDuration);

        if(spell.specialEffect == EnumCustom.SpecialEffect.Invoke)
        {
            content.text = "Max Invokes";
            spell.invokeLimit = EditorGUILayout.IntField(content, spell.invokeLimit);
        }
    }
}
