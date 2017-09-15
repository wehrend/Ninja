Shader "RockVR/CopyReverse"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "" {}
    }
        CGINCLUDE

#include "UnityCG.cginc"

    struct v2f 
    {
        float4 pos : POSITION;
        float2 uv : TEXCOORD0;
    };

    uniform sampler2D _MainTex;

    v2f vert (appdata_img v) 
    {
        v2f o;
        o.pos = UnityObjectToClipPos (v.vertex);
        float2 uv = v.texcoord.xy;
        o.uv = uv;
        return o;
    }

    half4 frag (v2f i): COLOR
    {
        float2 uv;
        uv.y = 1 - i.uv.y;
        uv.x = i.uv.x;
        i.uv = uv;
        return tex2D (_MainTex, i.uv);
    }
        ENDCG

    Subshader 
    {
        ZTest Always
        Cull Off
        ZWrite Off
        Fog{ Mode off }
        Pass
        {
            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
            ENDCG
        }
    }
    Fallback Off
}