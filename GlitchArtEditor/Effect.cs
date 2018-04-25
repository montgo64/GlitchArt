using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Effects
{
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    public struct FloatToInt
    {
        [FieldOffset(0)] public float FloatVal;
        [FieldOffset(0)] public int   IntVal;
    }

    public struct Parameter
    {
        public String name;
        public double value;
        public int minValue;
        public int maxValue;
        public double frequency;
    }

    public class EffectParameters
    {
        private Dictionary<String, Parameter> parameters;

        public EffectParameters() { }

        public Dictionary<String, Parameter> GetParams()
        {
            return parameters;
        }

        public void SetParams(Dictionary<String, Parameter> parameters)
        {
            this.parameters = parameters;
        }
    }

    interface Effect
    {
        void ProcessBlock(ref FloatToInt[] input, ref FloatToInt[] output, int length);
    }

}