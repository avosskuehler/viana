// Threshold shader 

sampler2D input : register(s0);
float threshold : register(c0);
float4 blankColor : register(c1);
float4 targetColor : register(c2);

float4 main(float2 uv : TEXCOORD, float2 center: VPOS) : COLOR
{
    float rMin=targetColor .r-threshold;
    float rMax=targetColor .r+threshold;
    float gMin=targetColor .g-threshold;
    float gMax=targetColor .g+threshold;
    float bMin=targetColor .b-threshold;
    float bMax=targetColor .b+threshold;
    
    center.x=0;
    center.y=1;
    
    float4 Color= tex2D(input , uv);
    if ((Color.r<rMin || Color.r>rMax) || (Color.g<gMin || Color.g>gMax) || (Color.b<bMin || Color.b>bMax))
	{
		Color = blankColor;
	}

    return Color;
}