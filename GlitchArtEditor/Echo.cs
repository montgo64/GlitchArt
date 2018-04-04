using Effects;

namespace EchoEffect
{
    public class EchoParameters : EffectParameters
    {
        public double delay;
        public float decay;
        public int histLen;

        public EchoParameters()
        {
            delay = 1.0;
            decay = 0.5f;
            histLen = 10;
        }

        public EchoParameters(double del, float dec, int hl)
        {
            delay = del;
            decay = dec;
            histLen = hl;
        }
    }

    public class Echo : Effect
    {
        private double delay;
        private float decay;
        private float[] history;
        private int histPos;
        private int histLen;

        public Echo()
        {
            delay = 1.0;
            decay = 0.5f;
            histPos = 0;
            histLen = 10000;
            history = new float[histLen];
        }


        public Echo(ref EchoParameters ep)
        {
            delay = ep.delay;
            decay = ep.decay;
            histPos = 0;
            histLen = ep.histLen;
            history = new float[ep.histLen];

        }

        public void ProcessBlock(ref FloatToInt[] input, ref FloatToInt[] output, int length)
        {
            for (int i = 0; i < length; i++, histPos++)
            {
                if (histPos == histLen)
                {
                    histPos = 0;
                }
                history[histPos] = input[i].FloatVal + (history[histPos] * decay);
                output[i].FloatVal = history[histPos];
            }
        }


        public void SetParameters(ref EffectParameters param)
        {
            EchoParameters ep = (EchoParameters)param;

            delay = ep.delay;
            decay = ep.decay;
            histPos = 0;
            histLen = ep.histLen;
            history = new float[ep.histLen];
        }

        public EffectParameters GetParameters()
        {
            return (EffectParameters)new EchoParameters(delay, decay, histLen);

        }
    }
}