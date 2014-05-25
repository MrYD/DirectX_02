
float time : TIME;

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
	ps.pos = ps.position;
	float X = ps.pos.x;
	float Y = ps.pos.y;
	float roatX = X*cos(time / 2) - Y*sin(time / 2);
	float roatY = X*sin(time / 2) + Y*cos(time / 2);

	ps.position.y = sin((time + roatX) * 5);
	ps.position.x = roatX;
	

	return ps;
}

float4 PS(PS_INPUT input) :SV_Target
{
	float light = 0.1;
	float X = input.pos.x;
	float Y = input.pos.y;
	float roatX = X*cos(time / 2) - Y*sin(time / 2);
	float roatY = X*sin(time / 2) + Y*cos(time / 2);


	//左に移動するグラデーション
	//float r = sin((X + time    ) * 2) + X*Y + X * cos(time) + sin(time)*Y + light;     
	//float g = sin((X + time + 2) * 2) + X*Y + X * cos(time) + cos(time)*Y + light;
	//float b = sin((X + time - 2) * 2) + Y*Y + X * sin(time) + sin(time)*Y + light;

	//回転しながら弾ける色
	float r = abs(sin(log(2 + abs(time * roatX / ((roatY + 1) + roatY / (roatX + 1)))))) + sin((roatX + time) * 2) + roatX*roatY + roatX * cos(time) + sin(time)*roatY + light;
	float g = abs(sin(log(2 + abs(time * roatX / (roatY + 1))))) + sin((roatX + time + 2) * 2) + roatX*roatY + roatX * cos(time) + cos(time)*roatY + light;
	float b = abs(sin(log(2 + abs(time * roatY / (roatX + 1))))) + sin((roatX + time - 2) * 2) + roatY*roatY + roatX * sin(time) + sin(time)*roatY + light;


	return float4(r, g, b, 1);
}

technique10 DefaultTechnique
{
	pass DefaultPass
	{
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}