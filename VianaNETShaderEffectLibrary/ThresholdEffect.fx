// Threshold shader 

sampler2D input : register(s0);
float threshold : register(c0);
float4 blankColor : register(c1);
float4 targetColor : register(c2);
float4 cropColor : register(c3);
float minX : register(c4);
float maxX : register(c5);
float minY : register(c6);
float maxY : register(c7);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 c = 0;
    float rMin = targetColor.r - threshold;
    float rMax = targetColor.r + threshold;
    float gMin = targetColor.g - threshold;
    float gMax = targetColor.g + threshold;
    float bMin = targetColor.b - threshold;
    float bMax = targetColor.b + threshold;
	  
	if (uv.x < minX || uv.x > maxX || uv.y < minY || uv.y > maxY)
	{
		return cropColor;
	}

	float4 Color= tex2D(input , uv);
		
	if ((Color.r < rMin || Color.r > rMax) || 
	    (Color.g < gMin || Color.g > gMax) || 
	    (Color.b < bMin || Color.b > bMax))
	{
		return blankColor;
	}
	
    return Color;
}