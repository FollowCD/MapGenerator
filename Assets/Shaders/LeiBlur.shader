Shader "LeiUI/LeiBlur"
{
	Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

		_Unexplored("Unexplored Color", Color) = (0.05, 0.05, 0.05, 0.05)
		_Explored("Explored Color", Color) = (0.35, 0.35, 0.35, 0.35)
		_BlendFactor("Blend Factor", range(0,1)) = 0

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

		CGINCLUDE

 			#include "UnityCG.cginc"
 			#include "UnityUI.cginc"

        	#pragma multi_compile __ UNITY_UI_CLIP_RECT
        	#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma target 2.0

			struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

			struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float4 worldPosition : TEXCOORD0;
				half2 uv:TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

			sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

			half4 _MainTex_TexelSize;

			uniform half4 _Unexplored;
			uniform half4 _Explored;
			uniform half _BlendFactor;

			v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                return OUT;
            }



            //高斯模糊
            fixed4 fragBlur(v2f IN) : SV_Target
            {
                half4 color  = (tex2D(_MainTex, IN.uv) + _TextureSampleAdd) * IN.color;

				half2 fog = lerp(color.rg,color.ba,_BlendFactor);
				half4 temp = lerp(_Unexplored,_Explored,fog.g);
				temp.a =(1-fog.r)*temp.a;
				color = temp;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

				return color;
			}


		ENDCG

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragBlur	
			#pragma fragmentoption ARB_precision_hint_fastest 
			ENDCG 
		}
        

    }
}
