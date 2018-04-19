using System;
using Effects;
namespace FadeEffect
{
    /// <summary>
    /// Class for Fade parameters.
    /// </summary>
    public class FadeParameters : EffectParameters
    {
        // public float mSample;
        public float mFadeIn;
        public float mFadeOut;
        public float mSampleCnt;

        /// <summary>
        /// Default constructor. Sets mSample to 1.0.
        /// </summary>
        public FadeParameters()
        {
            //   mSample = 1.0f;
            mFadeIn = 1.0f;
            mFadeOut = 1.0f;
            mSampleCnt = 10.0f;
        }

        /// <summary>
        /// Constructor. Sets parameter to mSample, mFadeIn, mSamplecnt.
        /// </summary>

        public FadeParameters(float fadeIn, float fadeOut, float sampleCnt)
        {
            //    mSample = sample;
            mFadeIn = fadeIn;
            mFadeOut = fadeOut;
            mSampleCnt = sampleCnt;
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
        public Fade(ref FadeParameters ap)
        {
            mFadeIn = ap.mFadeIn;
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

        /// <summary>
        /// Sets the parameters for Fade effect
        /// </summary>

        public void SetParameters(ref EffectParameters param)
        {
            FadeParameters fd = (FadeParameters)param;
            mFadeIn = fd.mFadeIn;
            mFadeOut = fd.mFadeOut;
            mSampleCnt = fd.mSampleCnt;
        }


        /// <summary>
        /// Returns the parameters for Fade effect
        /// </summary>
        public EffectParameters GetParameters()
        {
            return (EffectParameters)new FadeParameters(mFadeIn, mFadeOut, mSampleCnt);
        }
    }
}