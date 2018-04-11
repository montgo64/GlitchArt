using Effects;
using System;

namespace BassBoostEffect
{
    /// <summary>
    /// Class for BassBoost parameters.
    /// </summary>
    public class BassBoostParameters : EffectParameters
    {
        public double bass;

        /// <summary>
        /// Default constructor. Sets bass to 5.0.
        /// </summary>
        public BassBoostParameters()
        {
            bass = 5.0;
        }

        /// <summary>
        /// Constructor. Sets parameter to bass.
        /// </summary>
        public BassBoostParameters(double bas)
        {
            bass = bas;
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
        public BassBoost(ref BassBoostParameters btp)
        {
            bass = btp.bass;
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

        /// <summary>
        /// Sets the parameters for BassBoost effect
        /// </summary>
        public void SetParameters(ref EffectParameters param)
        {
            BassBoostParameters btp = (BassBoostParameters)param;

            bass = btp.bass;
        }

        /// <summary>
        /// Returns the parameters for BassBoost effect
        /// </summary>
        public EffectParameters GetParameters()
        {
            return (EffectParameters)new BassBoostParameters(bass);

        }
    }
}
