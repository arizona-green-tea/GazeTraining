Shader "OneEyeShader"
{
    Properties
    {
        _LeftTex ("Left-Eye Texture", 2D) = "white" {}
        _RightTex ("Right-Eye Texture", 2D) = "white" {}
        _LeftOpacity ("Left-Eye Opacity", Range(0,1)) = 1.0
        _RightOpacity ("Right-Eye Opacity", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend srcAlpha oneMinusSrcAlpha
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _LeftTex;
            sampler2D _RightTex;
            float4 _LeftTex_ST;
            float4 _RightTex_ST;
            half _LeftOpacity;
            half _RightOpacity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                if (unity_StereoEyeIndex == 0) {
                	o.uv = TRANSFORM_TEX(v.uv, _LeftTex);
                }
                else {
                	o.uv = TRANSFORM_TEX(v.uv, _RightTex);
                }
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
            	if (unity_StereoEyeIndex == 0) {
	                fixed4 col = tex2D(_LeftTex, i.uv);             
	                col.a = _LeftOpacity;
	                return col;
            	}
            	else {
	                fixed4 col = tex2D(_RightTex, i.uv);             
	                col.a = _RightOpacity;
	                return col;
            	}
            }
            ENDCG
        }
    }
}