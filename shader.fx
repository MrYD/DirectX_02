
float time : TIME;
Texture2D tex:TEX;
float4x4 changer : CHANGER;
float4x4 view : VIEW;

float4x4 proj: PROJECTION;
SamplerState mySampler

{
	//AddressU = MIRROR;
	//AddressV = MIRROR;
		AddressU = WRAP;
		AddressV = WRAP;
	//BorderColor = float4(1,0,0,1);
	//AddressV = BORDER;
	//AddressV = CLAMP;
	//Filter = MIN_MAG_MIP_POINT;
	//MaxAnisotropy = 1.0;
	//Filter = ANISOTROPIC;
};

struct VS_INPUT
{
	float3 normal:NORMAL;
	float4 position:POSITION;
};

struct PS_INPUT
{
	float4 pos:POSITION;
	float4 position:SV_Position;//ShaderValue
	float3 normal:NORMAL;
};


PS_INPUT VS(VS_INPUT input)
{
	PS_INPUT ps;
	ps.position = input.position;
	ps.position = mul(ps.position, changer);
	ps.position = mul(ps.position, view);
	ps.position = mul(ps.position, proj);
	ps.pos = input.position;
	ps.normal = input.normal;
	//ps.position.y *= sin((time + ps.pos.x) * 5);
	return ps;

}


float4 PS(PS_INPUT input) :SV_Target
{
	float X = input.pos.x;
	float Y = input.pos.y;
	float roatX = X*cos(time / 2) - Y*sin(time / 2);
	float roatY = X*sin(time / 2) + Y*cos(time / 2);
	float3 P = input.pos.xyz;
	float3 L = float3(1, 2, 0);
	float light = max(dot(normalize(P - L), input.normal), 0);

	//左に移動するグラデーション
	//float r = sin((X + time    ) * 2) + X*Y + X * cos(time) + sin(time)*Y + light;     
	//float g = sin((X + time + 2) * 2) + X*Y + X * cos(time) + cos(time)*Y + light;
	//float b = sin((X + time - 2) * 2) + Y*Y + X * sin(time) + sin(time)*Y + light;

	//回転しながら弾ける色
	//float r = abs(sin(log(2 + abs(time * roatX / ((roatY + 1) + roatY / (roatX + 1)))))) + sin((roatX + time) * 2) + roatX*roatY + roatX * cos(time) + sin(time)*roatY + light;
	//float g = abs(sin(log(2 + abs(time * roatX / (roatY + 1))))) + sin((roatX + time + 2) * 2) + roatX*roatY + roatX * cos(time) + cos(time)*roatY + light;
	//float b = abs(sin(log(2 + abs(time * roatY / (roatX + 1))))) + sin((roatX + time - 2) * 2) + roatY*roatY + roatX * sin(time) + sin(time)*roatY + light;


	    //return float4(light,light,light,1);
	float4 col =tex.Sample(mySampler, float2(X, Y))+ float4(light/2, light/2, light/2, 1);
		
		//col.rgb = 1.0.rrr - col.rgb;
		//col.rgb = dot(col.rgb,float3(0.3,0.59,0.11)).xxx;
		//col.rgb = col.rgb*col.rgb*col.rgb;
		//col.xyz *= t;
		
		return col;
}

technique10 DefaultTechnique
{
	pass DefaultPass
	{
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
}