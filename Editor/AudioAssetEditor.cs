using UnityEditor;
using UnityEngine;

namespace FE.EasyAudio.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AudioAsset))]
    public class AudioAssetEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            AssignSerializedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawLine();

            #region Clip & Durations

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Clip");
            EditorGUI.BeginChangeCheck();
            if (m_clip != null)
            {
                AudioClip clip;
                clip = (AudioClip)EditorGUILayout.ObjectField(m_clip.objectReferenceValue, typeof(AudioClip), false);
                m_clip.objectReferenceValue = clip;
            }
            if (EditorGUI.EndChangeCheck()) CalculateUsedDuration();
            EditorGUILayout.EndHorizontal();

            DrawLine();

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Clip Duration", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(true);

            EditorGUIUtility.labelWidth = 125f;
            EditorGUILayout.PropertyField(m_originalDuration, GUILayout.MinWidth(195), GUILayout.MaxWidth(195));
            EditorGUILayout.Space(1f);
            EditorGUILayout.PropertyField(m_usedDuration, GUILayout.MinWidth(195), GUILayout.MaxWidth(195));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            Vector2 normalizedTime = m_normalizedTime.vector2Value;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Normalized Time", GUILayout.Width(150));
            EditorGUILayout.MinMaxSlider(ref normalizedTime.x, ref normalizedTime.y, 0f, 1f);
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                m_normalizedTime.vector2Value = normalizedTime;
                CalculateUsedDuration();
            }

            #endregion

            DrawLine();

            #region Volume Settings

            EditorGUILayout.LabelField("Volume Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_useCurvedVolume, new GUIContent("Use curved volume"), GUILayout.MinWidth(200));
            bool usingCurvedVolume = m_useCurvedVolume.boolValue;

            if (usingCurvedVolume)
            {
                AnimationCurve curve = EditorGUILayout.CurveField(m_volumeCurve.animationCurveValue, GUILayout.MinWidth(200));
                m_volumeCurve.animationCurveValue = curve;
            }
            else
            {
                float slider = EditorGUILayout.Slider(m_fixedVolume.floatValue, 0, 1, GUILayout.MinWidth(200));
                m_fixedVolume.floatValue = slider;
            }

            EditorGUILayout.EndHorizontal();

            #endregion

            DrawLine();

            #region Pitch Settings

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pitch Settings", EditorStyles.boldLabel, GUILayout.MinWidth(155));
            EditorGUILayout.PropertyField(m_pitchPreference, GUILayout.MinWidth(250));
            EditorGUILayout.EndHorizontal();

            GUIContent header = new GUIContent();
            GUILayoutOption[] pitchPrefOptions = { GUILayout.MinWidth(400), GUILayout.MaxWidth(400) };

            switch ((PitchPreference)m_pitchPreference.enumValueIndex)
            {
                case PitchPreference.Fixed:
                    header.text = "Fixed Pitch";
                    EditorGUILayout.Slider(m_fixedPitch, 0, 3, header, pitchPrefOptions);
                    break;
                case PitchPreference.Curve:
                    header.text = "Curved Delta";
                    EditorGUILayout.PropertyField(m_pitchCurve, header, pitchPrefOptions);
                    break;
                case PitchPreference.Random:
                    header.text = "Randomized Delta";
                    EditorGUILayout.Slider(m_pitchRandomDifference, 0, 3, header, pitchPrefOptions);
                    break;
            }

            #endregion

            EditorGUILayout.Space(20);
            DrawLine(Color.red);

            if (Application.isPlaying)
            {
                GUILayout.Label("Testing", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Play"))
                {
                    AudioAsset audioAsset = (AudioAsset)target;
                    audioAsset.Play();
                }

                EditorGUILayout.EndHorizontal();   
            }
            else
            {
                GUILayout.Label("Testing", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("Testing is only available in play mode.", MessageType.Info);
            }


            serializedObject.ApplyModifiedProperties();
        }

        private void AssignSerializedProperties()
        {
            m_clip = serializedObject.FindProperty("_clip");
            m_originalDuration = serializedObject.FindProperty("_originalDuration");
            m_usedDuration = serializedObject.FindProperty("_usedDuration");
            m_normalizedTime = serializedObject.FindProperty("_normalizedTime");
            m_useCurvedVolume = serializedObject.FindProperty("_useCurvedVolume");
            m_fixedVolume = serializedObject.FindProperty("_fixedVolume");
            m_volumeCurve = serializedObject.FindProperty("_volumeCurve");
            m_fixedPitch = serializedObject.FindProperty("_fixedPitch");
            m_pitchCurve = serializedObject.FindProperty("_pitchCurve");
            m_pitchRandomDifference = serializedObject.FindProperty("_pitchRandomDifference");
            m_pitchPreference = serializedObject.FindProperty("_pitchPreference");
        }

        private void CalculateUsedDuration()
        {
            AudioClip clip = (AudioClip)m_clip.objectReferenceValue;
            if (clip == null) return;
            float originalDuration = clip.length;
            m_originalDuration.floatValue = originalDuration;

            float usedDuration = originalDuration * (m_normalizedTime.vector2Value.y - m_normalizedTime.vector2Value.x);
            m_usedDuration.floatValue = usedDuration;
        }

        private void DrawLine()
        {
            EditorGUILayout.Space(5);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), Color.gray);
            EditorGUILayout.Space(5);
        }

        private void DrawLine(Color color)
        {
            EditorGUILayout.Space(5);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), color);
            EditorGUILayout.Space(5);
        }

        #region Serialized Properties

        private SerializedProperty m_clip;
        private SerializedProperty m_originalDuration;
        private SerializedProperty m_usedDuration;
        private SerializedProperty m_normalizedTime;
        private SerializedProperty m_useCurvedVolume;
        private SerializedProperty m_fixedVolume;
        private SerializedProperty m_volumeCurve;
        private SerializedProperty m_fixedPitch;
        private SerializedProperty m_pitchCurve;
        private SerializedProperty m_pitchPreference;
        private SerializedProperty m_pitchRandomDifference;

        #endregion
    }
}