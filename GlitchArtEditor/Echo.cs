using Effects;

namespace Echo
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
            histLen = 10;
            history = new float[10];
        }


        public Echo(ref EchoParameters ep)
        {
            delay = ep.delay;
            decay = ep.decay;
            histPos = 0;
            histLen = ep.histLen;
            history = new float[ep.histLen];

        }

        public void ProcessBlock(ref float[] input, ref float[] output, int length)
        {
            for (int i = 0; i < length; i++, histPos++)
            {
                if (histPos == histLen)
                {
                    histPos = 0;
                }
                history[histPos] = output[i] = input[i] + history[histPos] * decay;
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