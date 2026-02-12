Shader "Custom/URP_Sprite_3D_Lit_Final"
{
    Properties
    {
        // MainTex property name is required for automatic texture assignment by SpriteRenderer
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        [MainColor] _Color ("Tint", Color) = (1,1,1,1)
        _Ambient ("Ambient Strength", Range(0,1)) = 0.2
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Cull Off
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Required keywords for URP to provide lighting data to the shader
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // Color from SpriteRenderer
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 positionWS : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _Color;
            float _Ambient;
            float _Cutoff;

            Varyings vert (Attributes v)
            {
                Varyings o;
                VertexPositionInputs pos = GetVertexPositionInputs(v.positionOS.xyz);
                o.positionCS = pos.positionCS;
                o.positionWS = pos.positionWS;
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // 1. Sample base color from sprite texture
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;
                
                // 2. Discard transparent pixels
                clip(tex.a - _Cutoff);

                // 3. Surface normal facing toward camera (negative Z axis)
                float3 N = normalize(TransformObjectToWorldNormal(float3(0, 0, -1)));
                
                // 4. Main light (Directional Light)
                float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                float rawNdotL = saturate(dot(N, mainLight.direction));
                float NdotL = pow(rawNdotL, 0.6);
                float3 lighting = mainLight.color * (NdotL * mainLight.distanceAttenuation * mainLight.shadowAttenuation);

                // 5. Additional lights (Point lights)
                #if defined(_ADDITIONAL_LIGHTS)
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, i.positionWS);
                    // Lambert calculation accepts light from both front and back sides
                    float lambert = dot(N, light.direction);
                    lambert = lambert * 0.5 + 0.5; // Remap from [-1,1] to [0,1]
                    lambert = pow(lambert, 0.6);
                    lighting += light.color * lambert * light.distanceAttenuation;

                }
                #endif

                // 6. Apply intensity mapping to prevent dark areas from being too dark
                float intensityFactor = length(lighting);
                float mapped = pow(intensityFactor, 0.6);
                float3 finalRGB = tex.rgb * mapped;

                return half4(finalRGB, tex.a);
            }
            ENDHLSL
        }

        // Shadow caster pass for character shadow projection onto ground
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            Cull Off
            ZWrite On

            HLSLPROGRAM
            #pragma vertex ShadowVert
            #pragma fragment ShadowFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float _Cutoff;

            Varyings ShadowVert (Attributes v)
            {
                Varyings o;
                float3 positionWS = TransformObjectToWorld(v.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(float3(0, 0, -1));
                o.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _MainLightPosition.xyz));
                o.uv = v.uv;
                return o;
            }

            half4 ShadowFrag (Varyings i) : SV_Target
            {
                half alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).a;
                clip(alpha - _Cutoff);
                return 0;
            }
            ENDHLSL
        }
    }
}