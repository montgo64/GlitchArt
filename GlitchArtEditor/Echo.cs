using Effects;

namespace EchoEffect
{
    /// <summary>
    /// Class for echo parameters.
    /// </summary>
    public class EchoParameters : EffectParameters
    {
        public double delay;
        public float decay;
        public int histLen;

        /// <summary>
        /// Default constructor. Sets delay to 1.0, decay
        /// to 0.5f, and history length to 10.
        /// </summary>
        public EchoParameters()
        {
            delay = 1.0;
            decay = 0.5f;
            histLen = 10;
        }

        /// <summary>
        /// Constructor. Sets parameters to delay, decay,
        /// and history length.
        /// </summary>
        public EchoParameters(double del, float dec, int hl)
        {
            delay = del;
            decay = dec;
            histLen = hl;
        }
    }

    /// <summary>
    /// Class for echo effect.
    /// </summary>
    public class Echo : Effect
    {
        private double delay;
        private float decay;
        private float[] history;
        private int histPos;
        private int histLen;

        /// <summary>
        /// Default constructor. Sets delay to 1.0, decay 
        /// to 0.5f, history length to 10000.
        /// </summary>
        public Echo()
        {
            delay = 1.0;
            decay = 0.5f;
            histPos = 0;
            histLen = 10000;
            history = new float[histLen];
        }

        /// <summary>
        /// Constructor. Stores parameter into variables.
        /// </summary>
        public Echo(ref EchoParameters ep)
        {
            delay = ep.delay;
            decay = ep.decay;
            histPos = 0;
            histLen = ep.histLen;
            history = new float[ep.histLen];

        }

        /// <summary>
        /// Applies the echo effect to the floattoint array 
        /// and stores the output to another floattoint array.
        /// This method is called when the user hits the apply
        /// button in the echo filter's window.
        /// </summary>
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

        /// <summary>
        /// Sets the parameters for echo effect
        /// </summary>
        public void SetParameters(ref EffectParameters param)
        {
            EchoParameters ep = (EchoParameters)param;

            delay = ep.delay;
            decay = ep.decay;
            histPos = 0;
            histLen = ep.histLen;
            history = new float[ep.histLen];
        }

        /// <summary>
        /// Returns the parameters for echo effect
        /// </summary>
        public EffectParameters GetParameters()
        {
            return (EffectParameters)new EchoParameters(delay, decay, histLen);

        }
    }
}