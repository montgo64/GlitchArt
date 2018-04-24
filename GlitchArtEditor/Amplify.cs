using Effects;
using System;
using System.Collections.Generic;

namespace AmplifyEffect
{
    /// <summary>
    /// Class for Amplify parameters.
    /// </summary>
    public class AmplifyParameters : EffectParameters
    {
        /// <summary>
        /// Default constructor. Sets mRatio to 3.0.
        /// </summary>
        public AmplifyParameters()
        {
            SetParams(new Dictionary<string, Parameter>() { { "Amplification (dB)",
                    new Parameter { name = "Amplification (dB)", value = 0, minValue = -100, maxValue = 100, frequency = 2 } } });
        }

        /// <summary>
        /// Constructor. Sets parameter for mRatio.
        /// </summary>
        public AmplifyParameters(float mRatio)
        {
            SetParams(new Dictionary<string, Parameter>() { { "Amplification (dB)",
                    new Parameter { name = "Amplification (dB)", value = mRatio, minValue = -100, maxValue = 100, frequency = 2 } } });
        }
    }

    /// <summary>
    /// Class for Amplify effect.
    /// </summary>
    public class Amplify : Effect
    {
        private float mRatio;

        /// <summary>
        /// Default constructor. Sets mRatio to 3.0
        /// </summary>
        public Amplify()
        {
            mRatio = 3.0f;
        }

        /// <summary>
        /// Constructor. Stores parameter into variables.
        /// </summary>
        public Amplify(AmplifyParameters ap)
        {
            foreach (Parameter parameter in ap.GetParams().Values)
            {
                if (parameter.name.Equals("Amplification (dB)"))
                {
                    mRatio = (float)parameter.value;
                }
            }
        }

        /// <summary>
        /// Applies the Amplify effect to the floattoint array 
        /// and stores the output to another floattoint array.
        /// This method is called when the user hits the apply
        /// button in the Amplify filter's window.
        /// </summary>
        public void ProcessBlock(ref FloatToInt[] input, ref FloatToInt[] output, int length)
        {
            for (int i = 0; i < length; i++)
            {
                output[i].FloatVal = (input[i].FloatVal * mRatio);
            }
        }
    }
}