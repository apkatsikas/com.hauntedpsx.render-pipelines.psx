using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using HauntedPSX.RenderPipelines.PSX.Runtime;
using static HauntedPSX.RenderPipelines.PSX.Editor.PSXMaterialUtils;

namespace HauntedPSX.RenderPipelines.PSX.Editor
{
    public abstract class BaseShaderGUI : ShaderGUI
    {
        protected MaterialEditor materialEditor { get; set; }

        protected MaterialProperty textureFilterModeProp { get; set; }

        protected MaterialProperty vertexColorModeProp { get; set; }

        protected MaterialProperty renderQueueCategoryProp { get; set; }

        protected MaterialProperty lightingModeProp { get; set; }

        protected MaterialProperty lightingBakedProp { get; set; }

        protected MaterialProperty lightingDynamicProp { get; set; }

        protected MaterialProperty shadingEvaluationModeProp { get; set; }

        protected MaterialProperty surfaceTypeProp { get; set; }

        protected MaterialProperty blendModeProp { get; set; }

        protected MaterialProperty cullingProp { get; set; }

        protected MaterialProperty alphaClipProp { get; set; }

        protected MaterialProperty alphaClippingDitherIsEnabledProp { get; set; }

        protected MaterialProperty alphaClippingScaleBiasMinMaxProp { get; set; }

        protected MaterialProperty affineTextureWarpingWeightProp { get; set; }

        protected MaterialProperty precisionGeometryWeightProp { get; set; }

        protected MaterialProperty fogWeightProp { get; set; }

        // Common Surface Input properties

        protected MaterialProperty mainTexProp { get; set; }

        protected MaterialProperty mainColorProp { get; set; }

        protected MaterialProperty emissionTextureProp { get; set; }

        protected MaterialProperty emissionColorProp { get; set; }

        protected MaterialProperty emissionBakedMultiplierProp { get; set; }

        protected MaterialProperty reflectionProp { get; set; }

        protected MaterialProperty reflectionCubemapProp { get; set; }

        protected MaterialProperty reflectionTextureProp { get; set; }

        protected MaterialProperty reflectionColorProp { get; set; }

        protected MaterialProperty reflectionBlendModeProp { get; set; }

        protected MaterialProperty doubleSidedNormalModeProp { get; set; }

        protected MaterialProperty doubleSidedConstantsProp { get; set; }

        public bool m_FirstTimeApply = true;

        // Header foldout states

        bool m_SurfaceOptionsFoldout;

        bool m_SurfaceInputsFoldout;

        bool m_AdvancedFoldout;


        public abstract void MaterialChanged(Material material);

        public virtual void FindProperties(MaterialProperty[] properties)
        {
            textureFilterModeProp = FindProperty(PropertyNames._TextureFilterMode, properties);
            vertexColorModeProp = FindProperty(PropertyNames._VertexColorMode, properties);
            renderQueueCategoryProp = FindProperty(PropertyNames._RenderQueueCategory, properties);
            lightingModeProp = FindProperty(PropertyNames._LightingMode, properties);
            lightingBakedProp = FindProperty(PropertyNames._LightingBaked, properties);
            lightingDynamicProp = FindProperty(PropertyNames._LightingDynamic, properties);
            shadingEvaluationModeProp = FindProperty(PropertyNames._ShadingEvaluationMode, properties);
            surfaceTypeProp = FindProperty(PropertyNames._Surface, properties);
            blendModeProp = FindProperty(PropertyNames._Blend, properties);
            cullingProp = FindProperty(PropertyNames._Cull, properties);
            alphaClipProp = FindProperty(PropertyNames._AlphaClip, properties);
            alphaClippingDitherIsEnabledProp = FindProperty(PropertyNames._AlphaClippingDitherIsEnabled, properties);
            alphaClippingScaleBiasMinMaxProp = FindProperty(PropertyNames._AlphaClippingScaleBiasMinMax, properties);
            affineTextureWarpingWeightProp = FindProperty(PropertyNames._AffineTextureWarpingWeight, properties);
            precisionGeometryWeightProp = FindProperty(PropertyNames._PrecisionGeometryWeight, properties);
            fogWeightProp = FindProperty(PropertyNames._FogWeight, properties);
            mainTexProp = FindProperty(PropertyNames._MainTex, properties, false);
            mainColorProp = FindProperty(PropertyNames._MainColor, properties, false);
            emissionTextureProp = FindProperty(PropertyNames._EmissionTexture, properties, false);
            emissionColorProp = FindProperty(PropertyNames._EmissionColor, properties, false);
            emissionBakedMultiplierProp = FindProperty(PropertyNames._EmissionBakedMultiplier, properties, false);
            reflectionProp = FindProperty(PropertyNames._Reflection, properties, false);
            reflectionCubemapProp = FindProperty(PropertyNames._ReflectionCubemap, properties, false);
            reflectionTextureProp = FindProperty(PropertyNames._ReflectionTexture, properties, false);
            reflectionColorProp = FindProperty(PropertyNames._ReflectionColor, properties, false);
            reflectionBlendModeProp = FindProperty(PropertyNames._ReflectionBlendMode, properties, false);
            doubleSidedNormalModeProp = FindProperty(PropertyNames._DoubleSidedNormalMode, properties);
            doubleSidedConstantsProp = FindProperty(PropertyNames._DoubleSidedConstants, properties);
        }

        public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties)
        {
            if (materialEditorIn == null)
                throw new ArgumentNullException("materialEditorIn");

            FindProperties(properties); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
            materialEditor = materialEditorIn;
            Material material = materialEditor.target as Material;

            // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
            // material to a hpsx shader.
            if (m_FirstTimeApply)
            {
                OnOpenGUI(material, materialEditorIn);
                m_FirstTimeApply = false;
            }

            ShaderPropertiesGUI(material);
        }

        public virtual void OnOpenGUI(Material material, MaterialEditor materialEditor)
        {
            // Foldout states
            m_SurfaceOptionsFoldout = true;
            m_SurfaceInputsFoldout = true;
            m_AdvancedFoldout = false;

            foreach (var obj in  materialEditor.targets)
                MaterialChanged((Material)obj);
        }

        public void ShaderPropertiesGUI(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            EditorGUI.BeginChangeCheck();

            m_SurfaceOptionsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_SurfaceOptionsFoldout, Styles.SurfaceOptions);
            if(m_SurfaceOptionsFoldout)
            {
                DrawSurfaceOptions(material);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            m_SurfaceInputsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_SurfaceInputsFoldout, Styles.SurfaceInputs);
            if (m_SurfaceInputsFoldout)
            {
                DrawSurfaceInputs(material);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            m_AdvancedFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_AdvancedFoldout, Styles.AdvancedLabel);
            if (m_AdvancedFoldout)
            {
                DrawAdvancedOptions(material);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            DrawAdditionalFoldouts(material);

            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in  materialEditor.targets)
                    MaterialChanged((Material)obj);
            }
        }

        public virtual void DrawSurfaceOptions(Material material)
        {
            PSXMaterialUtils.DrawRenderQueueCategory(materialEditor, renderQueueCategoryProp);
            PSXMaterialUtils.DrawTextureFilterMode(materialEditor, textureFilterModeProp);
            PSXMaterialUtils.DrawVertexColorMode(materialEditor, vertexColorModeProp);
            PSXMaterialUtils.DrawLightingMode(material, materialEditor, lightingModeProp, lightingBakedProp, lightingDynamicProp);
            PSXMaterialUtils.DrawShadingEvaluationMode(materialEditor, shadingEvaluationModeProp);
            PSXMaterialUtils.DrawSurfaceTypeAndBlendMode(material, materialEditor, surfaceTypeProp, blendModeProp);
            PSXMaterialUtils.DrawCullingSettings(material, materialEditor, cullingProp, doubleSidedNormalModeProp, doubleSidedConstantsProp);
            PSXMaterialUtils.DrawAlphaClippingSettings(material, alphaClipProp, alphaClippingDitherIsEnabledProp, alphaClippingScaleBiasMinMaxProp);

            PSXMaterialUtils.DrawAffineTextureWarpingWeight(affineTextureWarpingWeightProp);
            PSXMaterialUtils.DrawPrecisionGeometryWeight(precisionGeometryWeightProp);
            PSXMaterialUtils.DrawFogWeight(fogWeightProp);
        }

        public virtual void DrawSurfaceInputs(Material material)
        {
            PSXMaterialUtils.DrawMainProperties(material, materialEditor, mainTexProp, mainColorProp);
        }

        public virtual void DrawAdvancedOptions(Material material)
        {
            PSXMaterialUtils.DrawAdvancedOptions(material, materialEditor);
        }

        public virtual void DrawAdditionalFoldouts(Material material){}

        protected virtual void DrawEmissionProperties(Material material)
        {
            PSXMaterialUtils.DrawEmissionProperties(material, materialEditor, emissionTextureProp, emissionColorProp, emissionBakedMultiplierProp);
        }

        protected virtual void DrawReflectionProperties(Material material)
        {
            PSXMaterialUtils.DrawReflectionProperties(
                material,
                materialEditor,
                reflectionProp,
                reflectionBlendModeProp,
                reflectionCubemapProp,
                reflectionTextureProp,
                reflectionColorProp
            );
        }

        public void DoPopup(GUIContent label, MaterialProperty property, string[] options)
        {
            PSXMaterialUtils.DoPopup(label, property, options, materialEditor);
        }

        // Copied from shaderGUI as it is a protected function in an abstract class, unavailable to others

        public new static MaterialProperty FindProperty(string propertyName, MaterialProperty[] properties)
        {
            return PSXMaterialUtils.FindProperty(propertyName, properties);
        }

        // Copied from shaderGUI as it is a protected function in an abstract class, unavailable to others

        public new static MaterialProperty FindProperty(string propertyName, MaterialProperty[] properties, bool propertyIsMandatory)
        {
            return PSXMaterialUtils.FindProperty(propertyName, properties, propertyIsMandatory);
        }
    }
}
