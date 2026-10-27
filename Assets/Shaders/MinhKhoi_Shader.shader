Shader "Custom/NewUnlitUniversalRenderPipelineShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _Specular("Specular", Float) = 1
        _SpecularConcentration("Specular Concentration", Float) = 20
        _AmbientColor("Ambient Color", Color) = (1, 1, 1, 1)
        _BaseMap("Base Map", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex MyVertFunction
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct MeshData
            {
                float4 positionOS : POSITION; // float thứ 4: 0 -> vector, 1 -> point.
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0; // TEXCOORD0 -> channel 0.
            };

            struct VertexOutput
            {
                float4 positionHCS : SV_POSITION; // Tọa độ clip space.
                float2 uv_1 : TEXCOORD0;
                float2 uv_2 : TEXCOORD1;
                float NdotL : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
                float3 normalOS: NORMAL;
            };

            sampler2D _BaseMap;
            
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor; // Chung buffer sẽ chung batch
                float _Specular;
                float _SpecularConcentration;
                float4 _AmbientColor;
                float4 _BaseMap_ST; // Dùng bởi TRANSFORM_TEX
            CBUFFER_END

            VertexOutput MyVertFunction(MeshData input)
            {
                VertexOutput output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv_1 = TRANSFORM_TEX(input.uv,  _BaseMap);
                output.uv_2 = input.uv;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalOS = TransformObjectToWorldNormal(input.normalOS);

                return output;
            }

            float4 frag(VertexOutput input) : SV_Target
            {
                float4 color  = tex2D(_BaseMap, input.uv_1) * _BaseColor;
                float3 normal = normalize(input.normalOS);
                Light mainLight = GetMainLight();
                float NdotL = saturate(dot(mainLight.direction, normal));

                float3 diffuse = color.rgb * (mainLight.color.rgb * NdotL + SampleSH(normal));

                float3 viewVec = normalize(GetCameraPositionWS() - input.positionWS.xyz);
                float3 reflectVec = mainLight.direction - 2.0 * dot(mainLight.direction, normal) * normal;
                
                float3 specular = color.rgb * mainLight.color.rgb * pow(saturate(dot(reflectVec, -viewVec)), max(_SpecularConcentration, 0.01)) * max(_Specular, 0.0);
        
                return float4(diffuse + specular, 1.0);
            }
            ENDHLSL
        }
    }
}
