using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/** WHEN ADDING A BEHAVIOUR:
 * 
 * - Add prop_allowBehaviour under generic attributes
 * - Add the props for the behaviour as SerializedProperties
 * - Connect the properties in OnEnable
 * - Get the allowBehaviour value in OnInspectorGUI
 * - Show / hide the variables and create the Folder in OnInspectorGUI
 * 
 */ 

[CustomEditor(typeof(SteeringEnemy)), CanEditMultipleObjects]
public class SteeringEnemyEditor : Editor
{
    [Header("Generic attributes")]
    public SerializedProperty prop_allowStatic;
    public SerializedProperty prop_allowFollow;
    public SerializedProperty prop_allowEscape;

    // follow behaviour
    public SerializedProperty prop_followMaxSpeed;
    public SerializedProperty prop_followForceMagnitude;
    public SerializedProperty prop_followDistance;

    // escape behaviour
    public SerializedProperty prop_escapeMaxSpeed;
    public SerializedProperty prop_escapeForceMagnitude;
    public SerializedProperty prop_escapeMinDistance;

    // Foldout menus
    protected static bool showStaticOptions;
    protected static bool showFollowOptions;
    protected static bool showEscapeOptions;

    private void OnEnable()
    {
        prop_allowStatic = serializedObject.FindProperty("allowStatic");
        prop_allowFollow = serializedObject.FindProperty("allowFollow");
        prop_allowEscape = serializedObject.FindProperty("allowEscape");

        prop_escapeMaxSpeed = serializedObject.FindProperty("escapeMaxSpeed");
        prop_escapeForceMagnitude = serializedObject.FindProperty("escapeForceMagnitude");
        prop_escapeMinDistance = serializedObject.FindProperty("escapeMinDistance");

        prop_followMaxSpeed = serializedObject.FindProperty("followMaxSpeed");
        prop_followForceMagnitude = serializedObject.FindProperty("followForceMagnitude");
        prop_followDistance = serializedObject.FindProperty("followDistance");
    }

    public override void OnInspectorGUI()
    {
        // Unity editor stuff
        base.OnInspectorGUI();
        serializedObject.Update();

        // Getting the behaviour values
        bool allowStatic = prop_allowStatic.boolValue;
        bool allowFollow = prop_allowFollow.boolValue;
        bool allowEscape = prop_allowEscape.boolValue;
        
        if (allowStatic)
        {
            // Creating foldout menu
            showStaticOptions = EditorGUILayout.Foldout(showStaticOptions, "STATIC behaviour options");
            /* Put lines similar to this one for each property that should be shown when 
                EditorGUILayout.PropertyField(property, new GUIContent("controllable"));*/
        }

        if (allowFollow)
        {
            // Creating foldout menu
            showFollowOptions = EditorGUILayout.Foldout(showFollowOptions, "FOLLOW behaviour options");

            if (showFollowOptions)
            {
                EditorGUILayout.PropertyField(prop_followMaxSpeed, new GUIContent("Max speed"));
                EditorGUILayout.PropertyField(prop_followForceMagnitude, new GUIContent("Force magnitude"));
                EditorGUILayout.PropertyField(prop_followDistance, new GUIContent("Distance"));
            }
        }

        if (allowEscape)
        {
            // Creating foldout menu
            showEscapeOptions = EditorGUILayout.Foldout(showEscapeOptions, "ESCAPE behaviour options");

            if (showEscapeOptions)
            {
                EditorGUILayout.PropertyField(prop_escapeMaxSpeed, new GUIContent("Max speed"));
                EditorGUILayout.PropertyField(prop_escapeForceMagnitude, new GUIContent("Force magnitude"));
                EditorGUILayout.PropertyField(prop_escapeMinDistance, new GUIContent("Escape min distance"));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
