using System;
using System.Collections.Generic;
using Effects;
namespace FadeEffect
{
    /// <summary>
    /// Class for Fade parameters.
    /// </summary>
    public class FadeParameters : EffectParameters
    {
        public float mFadeIn;
        public float mFadeOut;
        public float mSampleCnt;

        /// <summary>
        /// Default constructor. Sets mFadeIn to 1.0, mFadeOut
        /// to 1, and mSampleCnt to 10.
        /// </summary>
        public FadeParameters()
        {
            Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();
            parameters.Add("Fade In", new Parameter { name = "Fade In", value = 1, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("Fade out", new Parameter { name = "Fade out", value = 1, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("Sample Count", new Parameter { name = "Sample Count", value = 10, minValue = 0, maxValue = 100, frequency = 1 });

            SetParams(parameters);
        }

        /// <summary>
        /// Constructor. Sets parameters for mSample, mFadeIn, mSamplecnt.
        /// </summary>
        public FadeParameters(float fadeIn, float fadeOut, float sampleCnt)
        {
            Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();
            parameters.Add("Fade In", new Parameter { name = "Fade In", value = fadeIn, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("Fade out", new Parameter { name = "Fade out", value = fadeOut, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("Sample Count", new Parameter { name = "Sample Count", value = sampleCnt, minValue = 0, maxValue = 100, frequency = 1 });

            SetParams(parameters);
        }
    }

    /// <summary>
    /// Class for Fade effect.
    /// </summary>
    public class Fade : Effect
    {
        private float mFadeIn;
        private float mFadeOut;
        private float mSampleCnt;

        /// <summary>
        /// Default constructor. Sets mRatio to 3.0
        /// </summary>
        public Fade()
        {
            mFadeIn = 1.0f;
            mFadeOut = 1.0f;
            mSampleCnt = 10.0f;
        }

        /// <summary>
        /// Constructor. Stores parameter into variables.
        /// </summary>
        public Fade(FadeParameters fade)
        {
            foreach (Parameter parameter in fade.GetParams().Values)
            {
                if (parameter.name.Equals("Fade In"))
                {
                    mFadeIn = (float)parameter.value;
                }
                else if (parameter.name.Equals("Fade Out"))
                {
                    mFadeOut = (float)parameter.value;
                }
                else if (parameter.name.Equals("Sample Count"))
                {
                    mSampleCnt = (float)parameter.value;
                }
            }
        }

        /// <summary>
        /// Applies the Fade effect to the floattoint array 
        /// and stores the output to another floattoint array.
        /// This method is called when the user hits the apply
        /// button in the Fade filter's window.
        /// </summary>
        public void ProcessBlock(ref FloatToInt[] input, ref FloatToInt[] output, int length)
        {
            if (mFadeIn > 1)
            {
                for (int i = 0; i < length; i++)
                {
                    output[i].FloatVal = ((input[i].FloatVal * (mFadeIn++)) / mSampleCnt);
                }
            }

            if (mFadeOut > 1)
            {
                for (int i = 0; i < length; i++)
                {
                    output[i].FloatVal = (input[i].FloatVal * (mSampleCnt - 1 - mFadeOut++) / mSampleCnt);
                }
            }
        }
    }
}