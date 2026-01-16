Shader "Alduris/HivesAllergy"
{
    Properties 
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }
    
    Category 
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off  // we can turn backface culling off because we know nothing will be facing backwards

        SubShader
        {
            Pass 
            {
                CGPROGRAM
                #pragma target 3.0
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                #include "_ShaderFix.cginc"
                #include "_RippleClip.cginc"
                #include "_Functions.cginc"

                float4 _MainTex_ST;
                sampler2D _MainTex;
                sampler2D _UniNoise;

                sampler2D _GrabTexture;
                
                uniform float4 _spriteRect;
                uniform float2 _screenSize;

                half4 UnpackColor(float c)
                {
                    float3 color = float3(c, c * 255.0, c * 65025.0);
                    color = frac(color);
                    color.rg -= color.gb / 255.0;
                    return half4(color, 1);
                }

                float bell(float x)
                {
                    return exp(-3.14159265359 * x*x);
                }

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float2 scrPos : TEXCOORD1;
                    float2 mudUv : TEXCOORD2;
                    float4 clr : COLOR;
                };

                v2f vert (appdata_full v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.scrPos = ComputeScreenPos(o.pos);
                    o.clr = UnpackColor(v.color.b);
                    o.mudUv = v.color.rg * 0.18;
                    o.clr.a = v.color.a;
                    return o;
                }

                half4 frag (v2f i) : SV_Target
                {
                    rippleClip(i.scrPos);

                    float threshold = 1-i.clr.a;
                    float noise = tex2D(_UniNoise, i.mudUv).r;
                    if (threshold > noise) discard;

                    float3 hsv = rgb2hsv(i.clr);
                    float hue = bell(mirror(hsv.x, 0.5) * 1.5) * sign((hsv.x + 0.5) % 1.0 - 0.5) * 0.5;
                    return half4(hsv2rgb(hue, pow(hsv.z, 0.333), 1.0), tex2D(_MainTex, i.uv).a * noise);
                }
                ENDCG
            }
        } 
    }
}