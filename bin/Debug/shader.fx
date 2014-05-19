﻿
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
	//ps.position.y *= abs(sin(time));
	ps.pos = ps.position;
	return ps;
}

float4 PS(PS_INPUT input) :SV_Target
{
	return float4(cos(time) / (input.pos.x+1) + sin(time)/(input.pos.y+1)  , input.pos.x*input.pos.y + input.pos.x * cos(time) + cos(time)*input.pos.y + 1, input.pos.x*input.pos.y + input.pos.x * sin(time) + sin(time)*input.pos.y + 1, 1);
}

technique10 DefaultTechnique
{
	pass DefaultPass
	{
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}