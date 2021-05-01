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
        window.Show();
    }

    Spell spell;

    private void OnEnable()
    {
        spell = CreateInstance("Spell") as Spell;
    }

    void OnGUI()
    {
        EditorGUIUtility.labelWidth = 130;
        GUIContent content = new GUIContent();

        content.text = "Spell Name";
        spell.spellName = EditorGUILayout.TextField(content, spell.spellName);

        content.text = "Available At";
        spell.availableAt = EditorGUILayout.IntSlider(content, spell.availableAt, 1, 10);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Damage");

        EditorGUILayout.BeginHorizontal();

        content.text = "Min";
        spell.minDamage = EditorGUILayout.IntField(content, spell.minDamage);
        content.text = "Max";
        spell.maxDamage = EditorGUILayout.IntField(content, spell.maxDamage);

        EditorGUILayout.EndHorizontal();
    }
}
