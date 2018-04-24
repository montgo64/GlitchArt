using Effects;
using System;
using System.Collections.Generic;

namespace BassBoostEffect
{
    /// <summary>
    /// Class for BassBoost parameters.
    /// </summary>
    public class BassBoostParameters : EffectParameters
    {
        /// <summary>
        /// Default constructor. Sets bass to 5.0.
        /// </summary>
        public BassBoostParameters()
        {
            SetParams(new Dictionary<string, Parameter>() { { "Bass (dB)",
                    new Parameter { name = "Bass (dB)", value = 0, minValue = -100, maxValue = 100, frequency = 2 } } });
        }

        /// <summary>
        /// Constructor. Sets parameter for bass.
        /// </summary>
        public BassBoostParameters(double bass)
        {
            SetParams(new Dictionary<string, Parameter>() { { "Bass (dB)",
                    new Parameter { name = "Bass (dB)", value = bass, minValue = -100, maxValue = 100, frequency = 2 } } });
        }
    }

    /// <summary>
    /// Class for BassBoost effect.
    /// </summary>
    public class BassBoost : Effect
    {
        private double bass;
       

        /// <summary>
        /// Default constructor. Sets bass to 5.0.
        /// </summary>
        public BassBoost()
        {
            bass = 5.0;
        }

        /// <summary>
        /// Constructor. Stores parameter into bass.
        /// </summary>
        public BassBoost(BassBoostParameters btp)
        {
            foreach (Parameter parameter in btp.GetParams().Values)
            {
                if (parameter.name.Equals("Bass (dB)"))
                {
                    bass = parameter.value;
                }
            }
        }

        /// <summary>
        /// Applies the BassBoost effect to the doubletoint array 
        /// and stores the output to another doubletoint array.
        /// This method is called when the user hits the apply
        /// button in the BassBoost filter's window.
        /// </summary>
        public void ProcessBlock(ref FloatToInt[] input, ref FloatToInt[] output, int length)
        {
            for (int i = 0; i < length; i++)
            {
                double cap = 1;
                double gain1 = 1.0 / (bass + 1.0);
                cap = (input[i].FloatVal + cap*bass) *gain1;
                output[i].FloatVal = (float) cap;
            }
        }
    }
}
