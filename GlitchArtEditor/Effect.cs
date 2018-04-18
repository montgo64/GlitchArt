using System.Runtime.InteropServices;

namespace Effects
{
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    public struct FloatToInt
    {
        [FieldOffset(0)] public float FloatVal;
        [FieldOffset(0)] public int   IntVal;
    }
    public class EffectParameters
    {
        public EffectParameters() { }
    }

    interface Effect
    {
        void ProcessBlock(ref FloatToInt[] input, ref FloatToInt[] output, int length);

        void SetParameters(ref EffectParameters param);

        EffectParameters GetParameters();
    }
}