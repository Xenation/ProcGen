// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:False,qofs:100,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:32983,y:32896,varname:node_4013,prsc:2|diff-83-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:31809,y:32194,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.2862745,c2:0.5450981,c3:0.7960785,c4:1;n:type:ShaderForge.SFN_DepthBlend,id:2575,x:31767,y:32536,varname:node_2575,prsc:2|DIST-2485-OUT;n:type:ShaderForge.SFN_Slider,id:2485,x:31434,y:32536,ptovrint:False,ptlb:ShoreFoamDistance,ptin:_ShoreFoamDistance,varname:_ShoreFoamDistance,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:10;n:type:ShaderForge.SFN_Multiply,id:5699,x:32135,y:32349,varname:node_5699,prsc:2|A-1304-RGB,B-2575-OUT;n:type:ShaderForge.SFN_Color,id:2414,x:31997,y:32711,ptovrint:False,ptlb:FoamColor,ptin:_FoamColor,varname:_FoamColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Add,id:6910,x:32337,y:32508,varname:node_6910,prsc:2|A-5699-OUT,B-6972-OUT;n:type:ShaderForge.SFN_Multiply,id:6972,x:32181,y:32645,varname:node_6972,prsc:2|A-3854-OUT,B-2414-RGB;n:type:ShaderForge.SFN_OneMinus,id:3854,x:31997,y:32553,varname:node_3854,prsc:2|IN-2575-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8281,x:30865,y:33061,ptovrint:False,ptlb:ShoreWaveDistance,ptin:_ShoreWaveDistance,varname:_ShoreWaveDistance,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_DepthBlend,id:579,x:31019,y:33061,varname:node_579,prsc:2|DIST-8281-OUT;n:type:ShaderForge.SFN_Frac,id:8585,x:31807,y:32875,varname:node_8585,prsc:2|IN-2029-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8890,x:31239,y:33138,ptovrint:False,ptlb:WavesFreq,ptin:_WavesFreq,varname:_WavesFreq,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Multiply,id:4712,x:31455,y:33037,varname:node_4712,prsc:2|A-3907-OUT,B-8890-OUT;n:type:ShaderForge.SFN_Add,id:83,x:32803,y:32896,varname:node_83,prsc:2|A-4435-OUT,B-915-OUT;n:type:ShaderForge.SFN_OneMinus,id:3907,x:31239,y:32939,varname:node_3907,prsc:2|IN-579-OUT;n:type:ShaderForge.SFN_Time,id:7840,x:31265,y:32733,varname:node_7840,prsc:2;n:type:ShaderForge.SFN_Add,id:2029,x:31639,y:32875,varname:node_2029,prsc:2|A-1227-OUT,B-4712-OUT;n:type:ShaderForge.SFN_Negate,id:1227,x:31455,y:32845,varname:node_1227,prsc:2|IN-7840-T;n:type:ShaderForge.SFN_OneMinus,id:304,x:32451,y:32851,varname:node_304,prsc:2|IN-915-OUT;n:type:ShaderForge.SFN_Multiply,id:4435,x:32629,y:32797,varname:node_4435,prsc:2|A-6910-OUT,B-304-OUT;n:type:ShaderForge.SFN_Step,id:4843,x:31239,y:33252,varname:node_4843,prsc:2|A-579-OUT,B-9003-OUT;n:type:ShaderForge.SFN_Vector1,id:9003,x:31019,y:33252,varname:node_9003,prsc:2,v1:0.9;n:type:ShaderForge.SFN_Multiply,id:2636,x:32085,y:33043,varname:node_2636,prsc:2|A-8585-OUT,B-3907-OUT,C-4843-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:915,x:32281,y:32987,ptovrint:False,ptlb:UseWaves,ptin:_UseWaves,varname:_UseWaves,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-6135-OUT,B-2636-OUT;n:type:ShaderForge.SFN_Vector1,id:6135,x:32085,y:32933,varname:node_6135,prsc:2,v1:0;proporder:1304-2485-2414-8281-8890-915;pass:END;sub:END;*/

Shader "Shader Forge/water" {
    Properties {
        _Color ("Color", Color) = (0.2862745,0.5450981,0.7960785,1)
        _ShoreFoamDistance ("ShoreFoamDistance", Range(0, 10)) = 0.5
        _FoamColor ("FoamColor", Color) = (1,1,1,1)
        _ShoreWaveDistance ("ShoreWaveDistance", Float ) = 2
        _WavesFreq ("WavesFreq", Float ) = 10
        [MaterialToggle] _UseWaves ("UseWaves", Float ) = 0
    }
    SubShader {
        Tags {
            "Queue"="Transparent+100"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform float _ShoreFoamDistance;
            uniform float4 _FoamColor;
            uniform float _ShoreWaveDistance;
            uniform float _WavesFreq;
            uniform fixed _UseWaves;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 projPos : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float node_2575 = saturate((sceneZ-partZ)/_ShoreFoamDistance);
                float4 node_7840 = _Time + _TimeEditor;
                float node_579 = saturate((sceneZ-partZ)/_ShoreWaveDistance);
                float node_3907 = (1.0 - node_579);
                float node_8585 = frac(((-1*node_7840.g)+(node_3907*_WavesFreq)));
                float _UseWaves_var = lerp( 0.0, (node_8585*node_3907*step(node_579,0.9)), _UseWaves );
                float3 diffuseColor = ((((_Color.rgb*node_2575)+((1.0 - node_2575)*_FoamColor.rgb))*(1.0 - _UseWaves_var))+_UseWaves_var);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform float _ShoreFoamDistance;
            uniform float4 _FoamColor;
            uniform float _ShoreWaveDistance;
            uniform float _WavesFreq;
            uniform fixed _UseWaves;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 projPos : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float node_2575 = saturate((sceneZ-partZ)/_ShoreFoamDistance);
                float4 node_7840 = _Time + _TimeEditor;
                float node_579 = saturate((sceneZ-partZ)/_ShoreWaveDistance);
                float node_3907 = (1.0 - node_579);
                float node_8585 = frac(((-1*node_7840.g)+(node_3907*_WavesFreq)));
                float _UseWaves_var = lerp( 0.0, (node_8585*node_3907*step(node_579,0.9)), _UseWaves );
                float3 diffuseColor = ((((_Color.rgb*node_2575)+((1.0 - node_2575)*_FoamColor.rgb))*(1.0 - _UseWaves_var))+_UseWaves_var);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
