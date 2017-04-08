    Shader "SciFiEffects/Antirim" {
        Properties {
            _Color("Main Color", Color)= (1,1,1)
            _MainTex ("Diffuse Color", 2D) = "white" {}
            _RimPower ("Alpha Amount", Range(0.0,1)) = 0.4
            _AlphaMultiplier( "Alpha Multiplier" , Range( 0,8 )) = 2.0
        }
        SubShader {
            Tags { "Queue"="Transparent" "RenderType"="Opaque" }
		
            CGPROGRAM
            #pragma surface surf Lambert alpha
           
            struct Input {
                float2 uv_MainTex;
                float3 viewDir;
            };

            float4 _Color;
            sampler2D _MainTex;
            float _RimPower;
            float _AlphaMultiplier;


           
            void surf (Input IN, inout SurfaceOutput o) {
           fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

                o.Albedo = c.rgb;
                half VN = saturate(dot (normalize(IN.viewDir), o.Normal));
                half rim = pow (1.0f - VN, (_RimPower*8));
                o.Alpha = c.a - (rim * _AlphaMultiplier);
                
            }
            ENDCG
        }
     
        FallBack "Transparent/VertexLit"
    }