using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Buffer = SlimDX.Direct3D11.Buffer;
using Device = SlimDX.Direct3D11.Device;

namespace DirectX_01
{
    public partial class Form1 : Form
    {
        Device device;
        SwapChain swapChain;
        RenderTargetView renderTarget;

        float a = 0;
        private Buffer vertexBuffer;
        private Effect effect;
        private InputLayout inputLayout;
        private float time;
        int n = 100;

        public void Render()
        {
            effect.GetVariableBySemantic("TIME").AsScalar().Set(time);
            a += 0.0003f;
            //device.ImmediateContext.ClearRenderTargetView(renderTarget, new Color4(1, (float)Math.Abs(Math.Sin(Math.PI / 3 + a / 1.5)), (float)Math.Abs(Math.Sin(a)), (float)Math.Abs(Math.Sin(Math.PI / 3 - a / 7.5))));//(A,R,G,B)
            //swapChain.Present(0, PresentFlags.None);
            device.ImmediateContext.OutputMerger.SetTargets(renderTarget);
            device.ImmediateContext.ClearRenderTargetView(renderTarget, new Color4( (float)Math.Abs(Math.Sin(Math.PI / 3 + a / 1.5)), (float)Math.Abs(Math.Sin(a)), (float)Math.Abs(Math.Sin(Math.PI / 3 - a / 7.5))));
            device.ImmediateContext.InputAssembler.InputLayout = inputLayout;
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0,
                new VertexBufferBinding[] { new VertexBufferBinding(vertexBuffer,16 , 0) });
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            effect.GetTechniqueByIndex(0).GetPassByIndex(0).Apply(device.ImmediateContext);
            device.ImmediateContext.Draw(3*n, 0);
            swapChain.Present(0, PresentFlags.None);
            time += 0.0001f;
            
        }

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SwapChainDescription desc = new SwapChainDescription() //descの設定
            {
                BufferCount = 2, // desc.BufferCount=2;と同じ//frontBufferとBackBuffer
                Flags = SwapChainFlags.AllowModeSwitch,
                IsWindowed = true,//windowsフォームか
                ModeDescription = new ModeDescription()
                {
                    Format = Format.R8G8B8A8_UNorm,//それぞれ8bit
                    Height = Height,
                    Width = Width,
                    RefreshRate = new Rational(1, 60),
                    Scaling = DisplayModeScaling.Centered,
                    ScanlineOrdering = DisplayModeScanlineOrdering.Progressive
                },
                OutputHandle = Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,//frontBufferのデータは捨てる
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None,
                new FeatureLevel[1] { FeatureLevel.Level_10_1 }, desc, out device, out swapChain);
            using (Texture2D tex = Texture2D.FromSwapChain<Texture2D>(swapChain, 0))
            {
                renderTarget = new RenderTargetView(device, tex);

            }
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport[] { new Viewport(0, 0, Right, Bottom, 0, 1) });
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
          
            Vector4[] verticie = new Vector4[3*n];
             //verticie[0]=   new Vector4(-1, -1, 0, 1);
             //verticie[1]=   new Vector4(-1, 1, 0, 1);
             //verticie[2] = new Vector4(1, -1, 0, 1);
             //verticie[3] = new Vector4(1, 1, 0, 1);
             //verticie[4] = new Vector4(1, -1, 0, 1);
             //verticie[5] = new Vector4(-1, 1, 0, 1);
        for (int i=0;i<3*n;i=i+3)
         {
            verticie[i] = new Vector4((float)Math.Sin((i/(n*3f))*Math.PI*2),(float)Math.Cos((i/(n*3f))*Math.PI*2), 0, 1);
            verticie[i+1] = new Vector4(0, 0, 0, 1);
            verticie[i+2] = new Vector4((float)Math.Sin(((i -3)/ (n * 3f)) * Math.PI * 2), (float)Math.Cos(((i-3) / (n * 3f)) * Math.PI * 2), 0, 1);
        }

            using (DataStream ds = new DataStream(verticie, true, true))
            {
                vertexBuffer = new Buffer(device, ds, new BufferDescription()
                {
                    BindFlags = BindFlags.VertexBuffer,
                    SizeInBytes = (int)ds.Length,
                });
               
        
            }
            using (ShaderBytecode compiledCode = ShaderBytecode.CompileFromFile("shader.fx", "fx_5_0", ShaderFlags.Debug, EffectFlags.None))
            {
                effect = new Effect(device, compiledCode);
            }
            inputLayout = new InputLayout(device, effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature, new InputElement[]
            {
                new InputElement()
                {
                    SemanticName = "POSITION",
                    Format = Format.R32G32B32A32_Float
                }, 
            });
          //  device.ImmediateContext.Rasterizer.State=RasterizerState.FromDescription(device,new RasterizerStateDescription(){FillMode=FillMode.Wireframe,CullMode=CullMode.None}); //逆回り
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport[] { new Viewport(0, 0, Width, Height, 0, 1), });
        }
    }
}
