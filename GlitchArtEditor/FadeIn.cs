using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Effects;
using FadeInEffect;

namespace FadeInEffect
{
    /// <summary>
    /// Class for Fade parameters.
    /// </summary>
    public class FadeInParameters : EffectParameters
    {
        public float mFadeIn;
        public float mSampleCnt;

        /// <summary>
        /// Default constructor. Sets mSample to 1.0.
        /// </summary>
        public FadeInParameters()
        {
            mFadeIn = 1.0f;  
            SetParams(new Dictionary<string, Parameter>() { { "Fade In",
                    new Parameter { name = "Fade In", value =mFadeIn, minValue = 0, maxValue = 100, frequency = 1 } } });
        }

        /// <summary>
        /// Constructor. Sets parameter to mSample, mFadeIn, mSamplecnt.
        /// </summary>

        public FadeInParameters(float fadeIn )
        {
           
            SetParams(new Dictionary<string, Parameter>() { { "Fade In",
                    new Parameter { name = "Fade In", value = fadeIn, minValue = 0, maxValue = 100, frequency = 1 } } });
        }
    }
 //   }

    /// <summary>
    /// Class for FadeIn effect.
    /// </summary>
    public class FadeIn : Effect
    {
        private float mFadeIn;
        private float mSampleCnt;
        /// <summary>
        /// Default constructor. Sets mRatio to 3.0
        /// </summary>
        public FadeIn()
        {
          
            mFadeIn = 1.0f;
          //  mSampleCnt = 0.0f;
        }

      
        /// <summary>
        /// Constructor. Stores parameter into variables.
        /// </summary>
        public FadeIn( FadeInParameters ap)
        {
           // mFadeIn = ap.mFadeIn;
        //    mSampleCnt = ap.mSampleCnt;
            
             foreach (Parameter parameter in ap.GetParams().Values)
              {
                  mFadeIn = (float)parameter.value;
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
               output[i].FloatVal = ((input[i].FloatVal * (mFadeIn++)) / ++mSampleCnt);
           }
      
        }

        /// <summary>
        /// Sets the parameters for Fade In effect
        /// </summary>

       public void SetParameters(ref EffectParameters param)
        {
            FadeInParameters fd = (FadeInParameters)param;
            mFadeIn = fd.mFadeIn;
        
        }


        /// <summary>
        /// Returns the parameters for Fade In effect
        /// </summary>
        public EffectParameters GetParameters()
        {
            return (EffectParameters)new FadeInParameters(mFadeIn);
        }
   }
}