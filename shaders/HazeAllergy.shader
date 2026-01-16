Shader "Alduris/HazeAllergy"
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
            GrabPass { }
            Pass 
            {
                CGPROGRAM
                #pragma target 3.0
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                #include "_ShaderFix.cginc"

                #define ALLERGY_HAZE_RAD 4

                float4 _MainTex_ST;
                sampler2D _MainTex;
                sampler2D _LevelTex;

                sampler2D _GrabTexture;
                
                uniform float4 _spriteRect;
                uniform float2 _screenSize;

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float2 scrPos : TEXCOORD1;
                    float4 clr : COLOR;
                };

                v2f vert (appdata_full v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.scrPos = ComputeScreenPos(o.pos);
                    o.clr = v.color;
                    return o;
                }

                half4 frag (v2f i) : SV_Target
                {
                    float4 returnColor = 0;
                    float counter = 0;
                    
                    float2 unitMult = i.clr.a / _screenSize;
                    float2 chromAbbDir = float2(cos(i.clr.g*6.283185), sin(i.clr.g*6.283185)) * i.clr.r;

                    [unroll]
                    for (int j = -ALLERGY_HAZE_RAD; j < ALLERGY_HAZE_RAD; j++)
                    {
                        [unroll]
                        for (int k = -ALLERGY_HAZE_RAD; k < ALLERGY_HAZE_RAD; k++)
                        {
                            float distFac = 1 - saturate(distance(float2(j, k), 0) / ALLERGY_HAZE_RAD);
                            float2 offset = float2(j, k) * unitMult;
                        
                            float4 colorToAdd;
                            colorToAdd.g = tex2D(_GrabTexture, i.scrPos + offset).g;
                            colorToAdd.r = tex2D(_GrabTexture, i.scrPos + offset + chromAbbDir * offset).r;
                            colorToAdd.b = tex2D(_GrabTexture, i.scrPos + offset - chromAbbDir * offset).b;
                            colorToAdd.a = 1;

                            returnColor += colorToAdd * distFac;
                            counter += distFac;
                        }
                    }

                    return returnColor / counter;
                }
                ENDCG
            }
        } 
    }
}

