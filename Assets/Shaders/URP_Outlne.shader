Shader "Custom/URP_Outline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Float) = 0.02
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" }
        Pass
        {
            Name "Outline"
            Cull Front
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            Varyings vert(Attributes input)
            {
                Varyings output;

                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);

                positionWS += normalWS * _OutlineWidth;

                output.positionHCS = TransformWorldToHClip(positionWS);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return _OutlineColor;
            }

            ENDHLSL
        }
    }
}
