Shader "UI/Default3D" 
{
	// ------------------------------------【属性值】------------------------------------
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}
	
	SubShader
		{
			Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On

		// --------------------------------唯一的通道-------------------------------
		Pass
		{
			// ===========开启CG着色器语言编写模块===========
			CGPROGRAM

			// 编译指令:告知编译器顶点和片段着色函数的名称
			#pragma vertex vert
			#pragma fragment frag

			// 包含头文件
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

			// 顶点着色器输入结构
			struct appdata
			{
				float4 vertex : POSITION;		// 顶点位置
				float3 normal : NORMAL;			// 法线向量坐标
				float4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			// 顶点着色器输出结构
			struct v2f
			{
				float4 position : SV_POSITION;	//像素位置
				float3 normal : NORMAL;			//法线向量坐标
				float4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			// 变量声明
			float4 _Color;
			sampler2D _MainTex;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

			// --------------------------------【顶点着色函数】-----------------------------
			// 输入：顶点输入结构体
			// 输出：顶点输出结构体
			// ---------------------------------------------------------------------------------
			v2f vert(appdata input)
			{
				v2f output;
				UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				output.position = UnityObjectToClipPos(input.vertex);

				// 获取顶点在世界空间中的法线向量坐标
				output.normal = mul(float4(input.normal, 0.0), unity_WorldToObject).xyz;

				output.color = input.color;
				output.texcoord = TRANSFORM_TEX(input.texcoord, _MainTex);
				
				return output;
			}
			
			

			//--------------------------------【片段着色函数】-----------------------------
			// 输入：顶点输出结构体
			// 输出：float4型的像素颜色值
			//---------------------------------------------------------------------------------
			fixed4 frag(v2f input) : COLOR
			{
				// 先准备好需要的参数
				// 获取法线的方向
				float3 normalDirection = normalize(input.normal);

				// 获取入射光线的值与方向
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

				// 计算灯光衰减
                float attenuation = LIGHT_ATTENUATION(input);
                float3 attenColor = attenuation * _LightColor0.xyz;

				// 基于兰伯特模型计算灯光
				float ndotL = max(0, dot(normalDirection, lightDirection));

				// 方向光
                float3 directionDiffuse = pow(ndotL, 0.3) * attenColor;

				// 环境光  
				float3 inDirectionDiffuse = input.color.rgb + UNITY_LIGHTMODEL_AMBIENT.rgb;
				
				half4 color = input.color;
				
				if (input.texcoord.x > -1)
					color = (tex2D(_MainTex, input.texcoord) + _TextureSampleAdd) * input.color;

				float3 diffuseColor = color * (directionDiffuse + inDirectionDiffuse);
				
				float4 ret = float4(diffuseColor, color.a);

				#ifdef UNITY_UI_CLIP_RECT
                ret.a *= UnityGet2DClipping(input.position.xy, _ClipRect);
                #endif
				
                #ifdef UNITY_UI_ALPHACLIP
                clip (ret.a - 0.001);
                #endif
				
				return ret;
			}
			ENDCG
		}
	}
}