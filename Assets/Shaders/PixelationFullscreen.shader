
/*
 * AI-GENERATED IMPLEMENTATION
 *
 * This code was developed with AI assistance to support
 * supplementary features outside the core NEA objectives.
 *
 * It is primarily intended to improve the visual presentation
 * and overall user experience of the project, rather than
 * demonstrate the programming techniques being assessed.
 */

Shader "Hidden/DungeonNEA/Pixelation"
{
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }

        ZTest Always
        ZWrite Off
        Cull Off

        HLSLINCLUDE

        #pragma target 3.5

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        float _ColorSteps;
        float _QuantizeColors;

        half4 FragDownsample(Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            return SAMPLE_TEXTURE2D_X_LOD(
                _BlitTexture,
                sampler_PointClamp,
                input.texcoord,
                0
            );
        }

        half4 FragUpscale(Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            half4 color = SAMPLE_TEXTURE2D_X_LOD(
                _BlitTexture,
                sampler_PointClamp,
                input.texcoord,
                0
            );

            if (_QuantizeColors > 0.5)
            {
                float intervals = max(_ColorSteps - 1.0, 1.0);
                color.rgb = floor(saturate(color.rgb) * intervals + 0.5)
                    / intervals;
            }

            return color;
        }

        ENDHLSL

        Pass
        {
            Name "Downsample"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragDownsample
            ENDHLSL
        }

        Pass
        {
            Name "Upscale"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragUpscale
            ENDHLSL
        }
    }

    Fallback Off
}
