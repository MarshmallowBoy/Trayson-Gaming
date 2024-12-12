Shader "Hidden/RcasFinalBlit_URP"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
        LOD 100
        ZWrite Off ZTest Always Blend Off Cull Off

        Pass
        {
            HLSLPROGRAM 
             
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex Vert
            #pragma fragment RCAS_Frag

            TEXTURE2D_X(_BlitTexture);
            uniform float4 _BlitScaleBias;
            uniform float _BlitMipLevel;
            uniform float4 _BlitTexture_TexelSize;

            #ifndef UNITY_CORE_SAMPLERS_INCLUDED
                SAMPLER(sampler_LinearClamp);
            #endif

            #if SHADER_API_GLES
            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            #else
            struct Attributes
            {
                uint vertexID : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            #endif
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord   : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
            
            #if SHADER_API_GLES
                float4 pos = input.positionOS;
                float2 uv  = input.uv;
            #else
                float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
                float2 uv  = GetFullScreenTriangleTexCoord(input.vertexID);
            #endif
            
                output.positionCS = pos;
                output.texcoord   = uv * _BlitScaleBias.xy + _BlitScaleBias.zw;
                return output;
            }
            
            float Sharpness;

            // RCAS also supports a define to enable a more expensive path to avoid some sharpening of noise.
            // Would suggest it is better to apply film grain after RCAS sharpening (and after scaling) instead of using this define,
            //  #define FSR_RCAS_DENOISE 1
            
            static const float RCAS_LIMIT = (0.25-(1.0/16.0));

            half4 RCAS_Frag (Varyings input) : SV_Target
            {
                // Set up constants
                float con = exp2(-Sharpness);

                // Perform RCAS pass
                
                // Algorithm uses minimal 3x3 pixel neighborhood.
                //    b 
                //  d e f
                //    h
	            float2 off_mult = _BlitTexture_TexelSize.xy * 1.0;
                float2 uv = input.texcoord;
                float3 b = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uv + float2(0, -1) * off_mult, _BlitMipLevel).rgb;
                float3 d = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uv + float2(-1, 0) * off_mult, _BlitMipLevel).rgb;
                float3 e = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uv, _BlitMipLevel).rgb;
                float3 f = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uv + float2(1, 0) * off_mult, _BlitMipLevel).rgb;
                float3 h = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uv + float2(0, 1) * off_mult, _BlitMipLevel).rgb;
                // Luma times 2.
                float bL = b.g + 0.5 * (b.b + b.r);
                float dL = d.g + 0.5 * (d.b + d.r);
                float eL = e.g + 0.5 * (e.b + e.r);
                float fL = f.g + 0.5 * (f.b + f.r);
                float hL = h.g + 0.5 * (h.b + h.r);
                // Noise detection.
                float nz = 0.25 * (bL + dL + fL + hL) - eL;
                nz = clamp(
                    abs(nz)
                    /(
                        max(max(bL,dL),max(eL,max(fL,hL)))
                        -min(min(bL,dL),min(eL,min(fL,hL)))
                    ),
                    0.0, 1.0
                );
                nz= 1.0 - 0.5 * nz;
                // Min and max of ring.
                float3 mn4 = min(b, min(d, min(f, h)));
                float3 mx4 = max(b, max(d, max(f, h)));
                // Immediate constants for peak range.
                float2 peakC = float2(1.0, -4.0);
                // Limiters, these need to be high precision RCPs.
                float3 hitMin = min(mn4,e) / (4.0 * mx4);
                float3 hitMax = (peakC.x - max(mx4,e))  / (4.0 * mn4 + peakC.y);
                float3 lobeRGB = max(-hitMin, hitMax);
                float lobe = max(-RCAS_LIMIT,min(max(lobeRGB.r, max(lobeRGB.g, lobeRGB.b)), 0.0)) * con;
                // Apply noise removal.
                
                #ifdef FSR_RCAS_DENOISE
                    lobe *+ nz;
                #endif
                
                // Resolve, which needs the medium precision rcp approximation to avoid visible tonality changes.
                float3 col = (lobe * (b + d + h + f) + e) / (4.0 * lobe + 1.0);
	            
                return float4(col, 1.0);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
