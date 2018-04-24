using Effects;
using System;
using System.Collections.Generic;

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
            Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();
            parameters.Add("Delay", new Parameter { name = "Delay", value = 1, minValue = 1, maxValue = 100, frequency = 1 });
            parameters.Add("Decay", new Parameter { name = "Decay", value = 0.5, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("History Length", new Parameter { name = "History Length", value = 10000, minValue = 1000, maxValue = 100000, frequency = 1000 });

            SetParams(parameters);
        }

        /// <summary>
        /// Constructor. Sets parameters for delay, decay,
        /// and history length.
        /// </summary>
        public EchoParameters(double delay, float decay, int histLen)
        {
            Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();
            parameters.Add("Delay", new Parameter { name = "Delay", value = delay, minValue = 1, maxValue = 100, frequency = 1 });
            parameters.Add("Decay", new Parameter { name = "Decay", value = decay, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("History Length", new Parameter { name = "History Length", value = histLen, minValue = 1000, maxValue = 100000, frequency = 1000 });

            SetParams(parameters);
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
        public Echo(EchoParameters ep)
        {
            foreach (Parameter parameter in ep.GetParams().Values)
            {
                if (parameter.name.Equals("Delay"))
                {
                    delay = parameter.value;
                }
                else if (parameter.name.Equals("Decay"))
                {
                    decay = (float)parameter.value;
                }
                else if (parameter.name.Equals("History Length"))
                {
                    histPos = 0;
                    histLen = (int)parameter.value;
                    history = new float[histLen];
                }
            }
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
    }
}