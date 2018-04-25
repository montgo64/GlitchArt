using System;
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
        }

        /// <summary>
        /// Constructor. Sets parameter to mSample, mFadeIn, mSamplecnt.
        /// </summary>

        public FadeOutParameters(float fadeOut)
        { 
            mFadeOut = fadeOut;
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
        public FadeOut(ref FadeOutParameters ap)
        {
            mFadeOut = ap.mFadeOut;
            mSampleCnt = ap.mSampleCnt;
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