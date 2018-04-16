Shader "Custom/FlowLight" {
	//属性
	Properties{
		//_Color("Tint", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_FlowTex("Light Texture(A)", 2D) = "black"{}//贴图
		_MaskTex("Mask Texture(A)", 2D) = "white"{}//遮罩贴图
		_uvaddspeed("", float) = 0.5 //流光uv改变量
	}
	SubShader{
			Tags{ "RenderType" = "Transparent" 
			"Queue" = "Transparent" }
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Lambert

			// Use shader model 3.0 target, to get nicer looking lighting
			sampler2D _MainTex;
			sampler2D _FlowTex;//属性
			float _uvaddspeed;//属性


			struct Input {
				float2 uv_MainTex;
			};



			void surf(Input IN, inout SurfaceOutput o) {
				// Albedo comes from a texture tinted by color
				half4 c = tex2D(_MainTex, IN.uv_MainTex);

				float2 uv = IN.uv_MainTex;//计算流光uv
				uv.x /= 2;//取一半
				uv.x += -_Time.y* _uvaddspeed;//很向加上

				float flow = tex2D(_FlowTex, uv).a;//取出流光亮度

				//o.Albedo = c.rgb + float3(flow, flow, flow);//加上流光亮度颜色

				o.Emission = c.rgb + float3(flow, flow, flow);//加上流光亮度颜色
				// Metallic and smoothness come from slider variables
				o.Alpha = c.a;
			}
			ENDCG
		}
		FallBack "Diffuse"
}
