// Threshold shader 

sampler2D input : register(s0);
float threshold : register(c0);

float4 main(float2 uv : TEXCOORD, float2 center: VPOS) : COLOR
{
    float4 color = tex2D(input, uv);  
    return (color -0.5) * (1.0-0.5)/1.0; 
}