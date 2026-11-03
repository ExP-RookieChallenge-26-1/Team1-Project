using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 손님 데이터 Inspector의 요구사항 칸.
/// 판정 종류를 목록에서 고르고, 그 판정이 쓰는 값(개수/비율)만 보여주며, 요구사항 대사를 적는다.
/// </summary>
[CustomPropertyDrawer(typeof(CustomerRequirement))]
public class CustomerRequirementDrawer : PropertyDrawer
{
    const float DialogueHeight = 54f;

    static readonly RequirementType[] Types = (RequirementType[])Enum.GetValues(typeof(RequirementType));
    static string[] typeLabels;

    static string[] TypeLabels
    {
        get
        {
            if (typeLabels == null)
            {
                typeLabels = new string[Types.Length];
                for (int i = 0; i < Types.Length; i++) typeLabels[i] = CustomerRequirementCalculation.TypeLabel(Types[i]);
            }
            return typeLabels;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float line = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        if (!property.isExpanded) return line;
        var type = CurrentType(property);
        int lines = 4;   // 제목, 판정 종류, 재료, 판정 미리보기
        if (CustomerRequirementCalculation.UsesCount(type) || CustomerRequirementCalculation.UsesRatio(type)) lines++;
        return line * (lines + 1) + DialogueHeight;   // +1: 대사 제목 줄
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        float line = EditorGUIUtility.singleLineHeight;
        float step = line + EditorGUIUtility.standardVerticalSpacing;
        var row = new Rect(position.x, position.y, position.width, line);

        property.isExpanded = EditorGUI.Foldout(row, property.isExpanded, label, true);
        if (!property.isExpanded) { EditorGUI.EndProperty(); return; }

        EditorGUI.indentLevel++;
        var typeProp = property.FindPropertyRelative("type");
        var ingredientProp = property.FindPropertyRelative("ingredient");
        var countProp = property.FindPropertyRelative("count");
        var ratioProp = property.FindPropertyRelative("ratio");
        var dialogueProp = property.FindPropertyRelative("dialogue");

        // 판정 종류 (= 사용할 판정 함수)
        row.y += step;
        int current = Array.IndexOf(Types, CurrentType(property));
        EditorGUI.BeginChangeCheck();
        int picked = EditorGUI.Popup(row, "판정 종류", Mathf.Max(0, current), TypeLabels);
        if (EditorGUI.EndChangeCheck()) typeProp.intValue = (int)Types[picked];
        var type = Types[picked];

        row.y += step;
        EditorGUI.PropertyField(row, ingredientProp, new GUIContent("재료"));

        if (CustomerRequirementCalculation.UsesCount(type))
        {
            row.y += step;
            countProp.intValue = Mathf.Max(0, EditorGUI.IntField(row, "개수", countProp.intValue));
        }
        else if (CustomerRequirementCalculation.UsesRatio(type))
        {
            row.y += step;
            ratioProp.floatValue = EditorGUI.Slider(row, "비율 (0~1)", ratioProp.floatValue, 0f, 1f);
        }

        // 지금 설정이 어떤 판정인지 미리보기
        row.y += step;
        var preview = new CustomerRequirement
        {
            type = type,
            ingredient = (Ingredient)ingredientProp.intValue,
            count = countProp.intValue,
            ratio = ratioProp.floatValue,
        };
        EditorGUI.LabelField(row, "판정", preview.Describe(), EditorStyles.boldLabel);

        row.y += step;
        EditorGUI.LabelField(row, "요구사항 대사 (줄바꿈마다 대사 한 번)");
        row.y += step;
        var area = EditorGUI.IndentedRect(new Rect(row.x, row.y, row.width, DialogueHeight));
        EditorGUI.BeginChangeCheck();
        string text = EditorGUI.TextArea(area, dialogueProp.stringValue, EditorStyles.textArea);
        if (EditorGUI.EndChangeCheck()) dialogueProp.stringValue = text;

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

    static RequirementType CurrentType(SerializedProperty property) =>
        (RequirementType)property.FindPropertyRelative("type").intValue;
}
