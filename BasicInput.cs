using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DirectX_01
{
   public struct BasicInput
{
       public BasicInput(Vector4 pos, Vector3 nom)
    {
        this.position = pos;
        this.nomal = nom;
    }

    public Vector4 position;
    public Vector3 nomal;
    public static int SizeInBytes
    {
        get {
            return Marshal.SizeOf(typeof(BasicInput));
            }
    }

}

}
