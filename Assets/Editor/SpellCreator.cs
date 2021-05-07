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
        spell.minValue = EditorGUILayout.IntField(content, spell.minValue);
        content.text = "Max";
        spell.maxValue = EditorGUILayout.IntField(content, spell.maxValue);

        EditorGUILayout.EndHorizontal();

        content.text = "Fixed";
        spell.fixedValue = EditorGUILayout.IntField(content, spell.fixedValue);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        content.text = "Icon";
        //spell.icon = EditorGUILayout.ObjectField()
    }
}
