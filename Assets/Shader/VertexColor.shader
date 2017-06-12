Shader "Custom/VertexColor" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass{
			Lighting On
			ColorMaterial AmbientAndDiffuse
			SetTexture[_MainTex]{
				combine texture*primary DOUBLE
			}
		}
	}
	FallBack "Diffuse"
}
