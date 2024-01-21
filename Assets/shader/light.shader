Shader "Unlit/light"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                float2 values = v.vertex.xy;
                v2f o;
                v.vertex.x += ((sin(values.y * 5.0 + _Time.y * 2.0) * 0.5) - 0.25) * 0.1;
                v.vertex.y += ((cos(values.x * 5.0 + _Time.y * 2.0) * 0.5) - 0.25) * 0.1;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = {1, 1, 0.5, 1}; //tex2D(_MainTex, i.uv);
                col.a = (1 - length(i.uv - float2(0.5, 0.5))) * 0.07;

                return col;
            }
            ENDCG
        }
    }
}