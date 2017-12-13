// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:33142,y:32550,varname:node_4013,prsc:2|diff-502-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:31832,y:33524,ptovrint:False,ptlb:BaseColor,ptin:_BaseColor,varname:_BaseColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.216648,c2:0.472,c3:0.2606742,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:5270,x:31345,y:32581,ptovrint:False,ptlb:WaterLevel,ptin:_WaterLevel,varname:_WaterLevel,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.2;n:type:ShaderForge.SFN_FragmentPosition,id:6817,x:31345,y:32712,varname:node_6817,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:4833,x:30895,y:33446,prsc:2,pt:False;n:type:ShaderForge.SFN_Color,id:3573,x:32094,y:32088,ptovrint:False,ptlb:SandColor,ptin:_SandColor,varname:_SandColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9053163,c2:0.9338235,c3:0.6385705,c4:1;n:type:ShaderForge.SFN_Color,id:8757,x:31831,y:33073,ptovrint:False,ptlb:ClifColor,ptin:_ClifColor,varname:_ClifColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5808823,c2:0.5808823,c3:0.5808823,c4:1;n:type:ShaderForge.SFN_Color,id:976,x:31933,y:32605,ptovrint:False,ptlb:SnowColor,ptin:_SnowColor,varname:_SnowColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9558824,c2:0.9558824,c3:0.9558824,c4:1;n:type:ShaderForge.SFN_Dot,id:2269,x:31084,y:33384,varname:flatParts,prsc:2,dt:0|A-5261-OUT,B-4833-OUT;n:type:ShaderForge.SFN_Vector3,id:5261,x:30895,y:33323,varname:node_5261,prsc:2,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_Multiply,id:4337,x:32024,y:33201,varname:node_4337,prsc:2|A-8757-RGB,B-7808-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3271,x:31345,y:32877,ptovrint:False,ptlb:SnowLevel,ptin:_SnowLevel,varname:_SnowLevel,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_OneMinus,id:4389,x:32133,y:32860,varname:node_4389,prsc:2|IN-1910-OUT;n:type:ShaderForge.SFN_Multiply,id:2776,x:32400,y:33076,varname:node_2776,prsc:2|A-4389-OUT,B-1848-OUT;n:type:ShaderForge.SFN_Multiply,id:7697,x:32133,y:32711,varname:node_7697,prsc:2|A-976-RGB,B-1910-OUT;n:type:ShaderForge.SFN_Add,id:225,x:32583,y:32875,varname:node_225,prsc:2|A-7697-OUT,B-2776-OUT;n:type:ShaderForge.SFN_Clamp01,id:7808,x:31637,y:33349,varname:node_7808,prsc:2|IN-7349-OUT;n:type:ShaderForge.SFN_Multiply,id:9464,x:32024,y:33433,varname:node_9464,prsc:2|A-5810-OUT,B-1304-RGB;n:type:ShaderForge.SFN_Clamp01,id:1910,x:31933,y:32816,varname:snow,prsc:2|IN-8122-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9569,x:31559,y:32983,ptovrint:False,ptlb:SnowFade,ptin:_SnowFade,varname:_SnowFade,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Subtract,id:9913,x:31559,y:32786,varname:node_9913,prsc:2|A-6817-Y,B-3271-OUT;n:type:ShaderForge.SFN_Divide,id:8122,x:31760,y:32816,varname:node_8122,prsc:2|A-9913-OUT,B-9569-OUT;n:type:ShaderForge.SFN_Subtract,id:8769,x:31559,y:32641,varname:node_8769,prsc:2|A-6817-Y,B-5270-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1203,x:31559,y:32507,ptovrint:False,ptlb:SandFade,ptin:_SandFade,varname:_SandFade,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.25;n:type:ShaderForge.SFN_Divide,id:7246,x:31750,y:32507,varname:node_7246,prsc:2|A-8769-OUT,B-1203-OUT;n:type:ShaderForge.SFN_Multiply,id:1144,x:32267,y:32262,varname:node_1144,prsc:2|A-3573-RGB,B-3043-OUT;n:type:ShaderForge.SFN_Clamp01,id:219,x:31933,y:32395,varname:node_219,prsc:2|IN-7246-OUT;n:type:ShaderForge.SFN_OneMinus,id:3043,x:32094,y:32291,varname:node_3043,prsc:2|IN-219-OUT;n:type:ShaderForge.SFN_Multiply,id:7674,x:32754,y:32706,varname:node_7674,prsc:2|A-219-OUT,B-225-OUT;n:type:ShaderForge.SFN_Add,id:502,x:32939,y:32550,varname:node_502,prsc:2|A-1144-OUT,B-7674-OUT;n:type:ShaderForge.SFN_Add,id:1848,x:32214,y:33294,varname:node_1848,prsc:2|A-4337-OUT,B-9464-OUT;n:type:ShaderForge.SFN_OneMinus,id:8437,x:31278,y:33384,varname:node_8437,prsc:2|IN-2269-OUT;n:type:ShaderForge.SFN_OneMinus,id:5810,x:31832,y:33359,varname:node_5810,prsc:2|IN-7808-OUT;n:type:ShaderForge.SFN_Multiply,id:7349,x:31465,y:33349,varname:node_7349,prsc:2|A-7943-OUT,B-8437-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7943,x:31278,y:33247,ptovrint:False,ptlb:CliffMult,ptin:_CliffMult,varname:_CliffMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.5;proporder:1304-8757-976-3271-9569-5270-3573-1203-7943;pass:END;sub:END;*/

Shader "Shader Forge/terrain" {
    Properties {
        _BaseColor ("BaseColor", Color) = (0.216648,0.472,0.2606742,1)
        _ClifColor ("ClifColor", Color) = (0.5808823,0.5808823,0.5808823,1)
        _SnowColor ("SnowColor", Color) = (0.9558824,0.9558824,0.9558824,1)
        _SnowLevel ("SnowLevel", Float ) = 10
        _SnowFade ("SnowFade", Float ) = 1
        _WaterLevel ("WaterLevel", Float ) = 0.2
        _SandColor ("SandColor", Color) = (0.9053163,0.9338235,0.6385705,1)
        _SandFade ("SandFade", Float ) = 0.25
        _CliffMult ("CliffMult", Float ) = 1.5
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
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
            uniform float4 _BaseColor;
            uniform float _WaterLevel;
            uniform float4 _SandColor;
            uniform float4 _ClifColor;
            uniform float4 _SnowColor;
            uniform float _SnowLevel;
            uniform float _SnowFade;
            uniform float _SandFade;
            uniform float _CliffMult;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
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
                float node_219 = saturate(((i.posWorld.g-_WaterLevel)/_SandFade));
                float node_9913 = (i.posWorld.g-_SnowLevel);
                float node_8122 = (node_9913/_SnowFade);
                float snow = saturate(node_8122);
                float flatParts = dot(float3(0,1,0),i.normalDir);
                float node_7808 = saturate((_CliffMult*(1.0 - flatParts)));
                float3 diffuseColor = ((_SandColor.rgb*(1.0 - node_219))+(node_219*((_SnowColor.rgb*snow)+((1.0 - snow)*((_ClifColor.rgb*node_7808)+((1.0 - node_7808)*_BaseColor.rgb))))));
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
            uniform float4 _BaseColor;
            uniform float _WaterLevel;
            uniform float4 _SandColor;
            uniform float4 _ClifColor;
            uniform float4 _SnowColor;
            uniform float _SnowLevel;
            uniform float _SnowFade;
            uniform float _SandFade;
            uniform float _CliffMult;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float node_219 = saturate(((i.posWorld.g-_WaterLevel)/_SandFade));
                float node_9913 = (i.posWorld.g-_SnowLevel);
                float node_8122 = (node_9913/_SnowFade);
                float snow = saturate(node_8122);
                float flatParts = dot(float3(0,1,0),i.normalDir);
                float node_7808 = saturate((_CliffMult*(1.0 - flatParts)));
                float3 diffuseColor = ((_SandColor.rgb*(1.0 - node_219))+(node_219*((_SnowColor.rgb*snow)+((1.0 - snow)*((_ClifColor.rgb*node_7808)+((1.0 - node_7808)*_BaseColor.rgb))))));
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
