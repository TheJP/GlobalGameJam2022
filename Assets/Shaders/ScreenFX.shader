Shader "Image Effects/ScreenFX" {
	Properties {
		_MainTex ("Render Input", 2D) = "white" {}
		_Color1 ("Main Color", Color) = (1,1,1,1)
		_Color2 ("Main Color", Color) = (1,1,1,1)
	}
	SubShader {
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		Pass {

  

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
			
				sampler2D _MainTex;
                fixed4 _Color1;
                fixed4 _Color2;

	struct v2f_img_color
	{
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		fixed4 color : COLOR;
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};


    v2f_img_color vert( appdata_img v )
    {
        v2f_img_color o;
        //UNITY_INITIALIZE_OUTPUT(v2f_img, o);
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.pos = UnityObjectToClipPos (v.vertex);
		o.color = o.pos.y;
        o.uv = v.texcoord;
        return o;
    }


		float4 frag(v2f_img_color IN) : COLOR {

		half4 c = tex2D (_MainTex, IN.uv);	
		c = saturate(c.r * 10 - 5);

        c = lerp(_Color1, _Color2, c.r);

		
		c.b += 0.5f * IN.color.r;
                    
        float2 vd = float2(IN.uv.x - 0.5, (IN.uv.y - 0.5));
        c = lerp(c, 0 , saturate(2 * dot(vd,vd) - 0.5));
              

		return c;
				}
			ENDCG
		}
	}
}
