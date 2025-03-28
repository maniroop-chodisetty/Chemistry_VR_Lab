Shader "Bytesized/Fluid"
{
    Properties
    {
        /* Tint to multiply the main texture against */
		_Tint ("Tint", Color) = (1,1,1,1)
		/* Main texture */
		_MainTex ("Texture", 2D) = "white" {}
		/* The color of the foam */
		_FoamColor ("Foam Color", Color) = (1,1,1,1)
		/* The foam's width */
        _FoamWidth ("Foam Width", Range(0,0.1)) = 0.015
        /* Color for the rim lighting */
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		/* Power of the rim lighting effect */
	    _RimPower ("Rim Power", Range(0,10)) = 0.0
    }
 
    SubShader
    {
        Tags { "Queue"="Geometry" "DisableBatching" = "True" }
        Pass
        {
            /* Depth */
            Zwrite On
            /* We want the front and back faces */
            Cull Off
            /* Transparency */
            AlphaToMask On
            
        CGPROGRAM
            
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;	
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 viewDir : COLOR;
                float3 normal : COLOR2;
                float3 worldPos : TEXCOORD2;
            };
     
            /* Main texture and tint */
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Tint;
            /* Plane information for making the volume effect */
            float3 _PlanePoint;
            float3 _PlaneNormal;
            /* Foam color & width */
            float4 _FoamColor;
            float _FoamWidth;
            /* Rim lighting settings for the fluid */
            float _RimPower;
            float4 _RimColor;
    
            /* Vertex shader */
            v2f vert (appdata v)
            {
                v2f o;
                /* Boilerplate shader stuff */
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);	
                
                /* Calculate the vertex world position so we can compare it against the plane */
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;  
                o.worldPos = worldPos.xyz;
                o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
                o.normal = v.normal;
                return o;
            }
               
            /* Fragment shader */
            fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
            {
                /* Sample the texture & apply the fog */
                fixed4 col = tex2D(_MainTex, i.uv) * _Tint;
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                /* Calculate the fluid % */
                float4 result = step(dot(_PlaneNormal, i.worldPos - _PlanePoint + _PlaneNormal * _FoamWidth), 0.0);
                float4 resultColored = (result) * col;
                
                /* Calculate the foam */
                float4 foam = step(dot(_PlaneNormal, i.worldPos - _PlanePoint), 0.0);
                float4 foamColored = (foam - result) * (_FoamColor * 0.9);
                
                /* Calculate the rim lighting */
                float dotProduct = 1 - pow(dot(i.normal, i.viewDir), _RimPower);
                float4 RimResult = smoothstep(0.5, 1.0, dotProduct);
                RimResult *= _RimColor;
                
                /* Mix both results into a single color */
                float4 finalResult = resultColored + foamColored;				
                finalResult.rgb += RimResult;
                float4 topColor = (_FoamWidth > 0 ? _FoamColor * foam : col * result);
                return facing > 0 ? finalResult : topColor;   
            }
        ENDCG
        }
    }
}