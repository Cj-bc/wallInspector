// References:
// + Core concepts :: https://tomhulton.blogspot.com/2015/08/portal-rendering-with-offscreen-render.html
// + tex2Dproj :: https://light11.hatenadiary.com/entry/2018/06/13/235543

Shader "Unlit/Portal"
{
    Properties
    {
        _PortalInsideTex ("PortalInside", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Alpha" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            sampler2D _PortalInsideTex;

            v2f vert (appdata v)
            {
                v2f o;
		fixed4 clipSpacePosition = UnityObjectToClipPos(v.vertex);
                o.vertex = clipSpacePosition;
		o.uv = ComputeScreenPos(clipSpacePosition);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2Dproj(_PortalInsideTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
