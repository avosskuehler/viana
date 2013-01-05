// Cropeffect shader 

sampler2D input : register(s0);
float4 blankColor : register(c0);
float minX : register(c1);
float maxX : register(c2);
float minY : register(c3);
float maxY : register(c4);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv);  
	if (uv.x<minX||uv.x>maxX||uv.y<minY||uv.y>maxY)
	{
		return blankColor;
	}
	else {
    return color;
	}
}