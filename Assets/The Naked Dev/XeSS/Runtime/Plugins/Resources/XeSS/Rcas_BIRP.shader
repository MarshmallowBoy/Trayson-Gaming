Shader "Hidden/Rcas_BIRP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // List of properties to control your post process effect
            float Sharpness;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            uniform float4 _MainTex_TexelSize;

            // RCAS also supports a define to enable a more expensive path to avoid some sharpening of noise.
            // Would suggest it is better to apply film grain after RCAS sharpening (and after scaling) instead of using this define,
            //  #define FSR_RCAS_DENOISE 1          
            static const float RCAS_LIMIT = (0.25-(1.0/16.0));


            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                return o;
            }

            fixed4 frag (v2f input) : SV_Target
            {
                // Set up constants
                float con = exp2(-Sharpness);
                
                // Perform RCAS pass
                
                // Algorithm uses minimal 3x3 pixel neighborhood.
                //    b 
                //  d e f
                //    h
                float2 off_mult = _MainTex_TexelSize.xy * 1.0;
                float2 uv = input.texcoord.xy;
                float3 b = tex2D(_MainTex, uv + float2(0, -1) * off_mult).rgb;
                float3 d = tex2D(_MainTex, uv + float2(-1, 0) * off_mult).rgb;
                float3 e = tex2D(_MainTex, uv).rgb;
                float3 f = tex2D(_MainTex, uv + float2(1, 0) * off_mult).rgb;
                float3 h = tex2D(_MainTex, uv + float2(0, 1) * off_mult).rgb;
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
            ENDCG
        }
    }
}
