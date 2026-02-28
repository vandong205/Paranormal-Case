Shader "Custom/SpriteLit_Final_NoWarning"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Translucency("Backlight Brightness", Range(0, 1)) = 0.5
        _ShadowThickness("Surface Offset", Range(0,0.1)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off 
        ZWrite Off

        // ------------------------------------------------------------------
        // FORWARD PASS
        // ------------------------------------------------------------------
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Cấu hình quan trọng cho GPU Skinning
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile_instancing
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _ShadowThickness;
                float _Translucency;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 normalOS : NORMAL;
                
                // --- QUAN TRỌNG: Khai báo này giúp mất Warning khi Skinning ---
                float4 boneWeight : BLENDWEIGHTS;
                uint4 boneIndices : BLENDINDICES;
                // ------------------------------------------------------------

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 positionWS : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                // Hàm này trong URP sẽ tự động gọi GPU Skinning nếu Attributes có Bone dữ liệu
                VertexPositionInputs pos = GetVertexPositionInputs(v.positionOS.xyz);
                o.positionWS = pos.positionWS;
                
                o.normalWS = normalize(mul((float3x3)UNITY_MATRIX_M, float3(0,0,-1)));
                o.positionWS += o.normalWS * _ShadowThickness;
                o.positionHCS = TransformWorldToHClip(o.positionWS);

                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                clip(tex.a - 0.05);

                float3 albedo = tex.rgb * i.color.rgb;
                float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS);
                Light mainLight = GetMainLight(shadowCoord);

                // Ngược sáng không tối đen (Wrap Lighting)
                float dotNL = dot(i.normalWS, mainLight.direction);
                float NdotL = saturate((dotNL + 0.5) / 1.5); 
                NdotL = max(NdotL, abs(dotNL) * _Translucency);

                float3 lighting = mainLight.color * NdotL * mainLight.shadowAttenuation;

                uint count = GetAdditionalLightsCount();
                for(uint l = 0; l < count; l++)
                {
                    Light light = GetAdditionalLight(l, i.positionWS);
                    float ndotl = saturate(dot(i.normalWS, light.direction) + _Translucency);
                    lighting += light.color * ndotl * light.distanceAttenuation;
                }

                float3 ambient = SampleSH(i.normalWS) * 0.15;
                return float4(albedo * (lighting + ambient), tex.a * i.color.a);
            }
            ENDHLSL
        }

        // ------------------------------------------------------------------
        // SHADOW CASTER PASS
        // ------------------------------------------------------------------
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            ZWrite On ZTest LEqual ColorMask 0 Cull Off

            HLSLPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS   : NORMAL;
                
                // --- PHẢI CÓ CẢ Ở ĐÂY ĐỂ TRÁNH LỖI KHI ĐANG TÍNH TOÁN BÓNG ---
                float4 boneWeight : BLENDWEIGHTS;
                uint4 boneIndices : BLENDINDICES;
                // ----------------------------------------------------------

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vertShadow(Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);

                float3 positionWS = TransformObjectToWorld(v.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(v.normalOS);

                o.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _MainLightPosition.xyz));

                #if UNITY_REVERSED_Z
                    o.positionCS.z = min(o.positionCS.z, o.positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #else
                    o.positionCS.z = max(o.positionCS.z, o.positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #endif

                o.uv = v.uv;
                return o;
            }

            float4 fragShadow(Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                clip(tex.a - 0.5); 
                return 0;
            }
            ENDHLSL
        }
    }
}