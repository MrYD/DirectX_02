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
        private float area;
        int n = 10;

        public void Render()
        {
            effect.GetVariableBySemantic("TIME").AsScalar().Set(time);
         //   effect.GetVariableBySemantic("AREA").AsScalar().Set(area);
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
            device.ImmediateContext.Draw(24*n, 0);
            swapChain.Present(0, PresentFlags.None);
            time += 0.001f;
            
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
          
            Vector4[] verticie = new Vector4[24*n];
          
        for (int i=0;i<24*n;i=i+12)
         {
            float j = i /(6f* n)-1;
            float k = (i + 12) / (6f * n) - 1;
            verticie[i] = new Vector4(j,0, 0, 1);
            verticie[i + 1] = new Vector4(j, 1, 0, 1);
            verticie[i + 2] = new Vector4(k, 1, 0, 1);
            verticie[i+3] = new Vector4(j,0, 0, 1);
            verticie[i+4] = new Vector4(k, 1, 0, 1);
            verticie[i + 5] = new Vector4(k, 0, 0, 1);
            verticie[i+6] = new Vector4(j, 0, 0, 1);
            verticie[i + 7] = new Vector4(k, -1, 0, 1);
            verticie[i + 8] = new Vector4(j, -1, 0, 1);
            verticie[i + 9] = new Vector4(j, 0, 0, 1);
            verticie[i + 10] = new Vector4(k, 0, 0, 1);
            verticie[i + 11] = new Vector4(k, -1, 0, 1);
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
