using System;
using System.Collections.Generic;
using Effects;

namespace FadeOutEffect
{
    /// <summary>
    /// Class for Fade parameters.
    /// </summary>
    public class FadeOutParameters : EffectParameters
    {
     
        public float mFadeOut;
        public float mSampleCnt;

        /// <summary>
        /// Default constructor. Sets mSample to 1.0.
        /// </summary>
        public FadeOutParameters()
        {
            mFadeOut = 1.0f;
            mSampleCnt = 0.0f;
            Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();
            parameters.Add("Fade out", new Parameter { name = "Fade out", value = 1, minValue = 0, maxValue = 100, frequency = 1 });
            //   parameters.Add("Sample Count", new Parameter { name = "Sample Count", value = 10, minValue = 0, maxValue = 100, frequency = 1 });
            SetParams(new Dictionary<string, Parameter>() { { "Fade In",
                    new Parameter { name = "Fade Out", value =mFadeOut, minValue = 1, maxValue = 100, frequency = 1 } } });
        }

        /// <summary>
        /// Constructor. Sets parameter to mSample, mFadeIn, mSamplecnt.
        /// </summary>

        public FadeOutParameters(float fadeOut)
        { 
            mFadeOut = fadeOut;
            Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();
          
            parameters.Add("Fade out", new Parameter { name = "Fade out", value = fadeOut, minValue = 0, maxValue = 100, frequency = 1 });
            //     parameters.Add("Sample Count", new Parameter { name = "Sample Count", value = sampleCnt, minValue = 0, maxValue = 100, frequency = 1 });
            SetParams(new Dictionary<string, Parameter>() { { "Fade Out",
                    new Parameter { name = "Fade Out", value = fadeOut, minValue = 0, maxValue = 100, frequency = 1 } } });
        }
    }

    /// <summary>
    /// Class for FadeOut effect.
    /// </summary>
    public class FadeOut : Effect
    {
    
        private float mFadeOut;
        private float mSampleCnt;
        /// <summary>
        /// Default constructor. Sets mRatio to 3.0
        /// </summary>
        public FadeOut()
        {
            mFadeOut = 1.0f;
            mSampleCnt = 0.0f;
        }

        /// <summary>
        /// Constructor. Stores parameter into variables.
        /// </summary>
        public FadeOut( FadeOutParameters ap)
        {
            mFadeOut = ap.mFadeOut;
            mSampleCnt = ap.mSampleCnt;
            foreach (Parameter parameter in ap.GetParams().Values)
            { 
                if (parameter.name.Equals("Fade Out"))
                {
                    mFadeOut = (float)parameter.value;
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

              for (int i = 0; i < length; i++)
              {
                 output[i].FloatVal = (input[i].FloatVal * (++mSampleCnt - 1 - mFadeOut++) / ++mSampleCnt);
                 
            }
         
        }

        /// <summary>
        /// Sets the parameters for Fade Out effect
        /// </summary>

        public void SetParameters(ref EffectParameters param)
        {
            FadeOutParameters fd = (FadeOutParameters)param;
            mFadeOut = fd.mFadeOut;
        }


        /// <summary>
        /// Returns the parameters for Fade Out effect
        /// </summary>
        public EffectParameters GetParameters()
        {
            return (EffectParameters)new FadeOutParameters(mFadeOut);
        }
    }
}