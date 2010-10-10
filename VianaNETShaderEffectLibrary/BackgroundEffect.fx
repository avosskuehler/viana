// Threshold shader 

sampler2D input : register(s0);
float threshold : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv);  
    return (color -threshold) * (1.0-threshold)/1.0; 
}