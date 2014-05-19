
float time:TIME;

struct VS_INPUT
{
	float4 position:POSITION;
};

struct PS_INPUT
{
	float4 pos:POSITION;
	float4 position:SV_Position;//ShaderValue
};

PS_INPUT VS(VS_INPUT input)
{
	PS_INPUT ps;
	ps.position = input.position;
	//ps.position.x *= abs(sin(time));
	ps.pos = ps.position;
	return ps;
}

float4 PS(PS_INPUT input) :SV_Target
{
	return float4(input.pos.x,input.pos.y,0,1);
}

technique10 DefaultTechnique
{
	pass DefaultPass
	{
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}