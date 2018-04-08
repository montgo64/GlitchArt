using Effects;

namespace AmplifyEffect
{
    /// <summary>
    /// Class for Amplify parameters.
    /// </summary>
    public class AmplifyParameters : EffectParameters
    {
        public double mRatio;

        /// <summary>
        /// Default constructor. Sets delay to 1.0, decay
        /// to 0.5f, and history length to 10.
        /// </summary>
        public AmplifyParameters()
        {
            mRatio = 3.0;
        }

        /// <summary>
        /// Constructor. Sets parameters to delay, decay,
        /// and history length.
        /// </summary>
        public AmplifyParameters(double rat, int dec, int hl)
        {
            mRatio = rat;
        }
    }

    /// <summary>
    /// Class for Amplify effect.
    /// </summary>
    public class Amplify : Effect
    {
        private double mRatio;

        /// <summary>
        /// Default constructor. Sets delay to 1.0, decay 
        /// to 0.5f, history length to 10000.
        /// </summary>
        public Amplify()
        {
            mRatio = 3.0;
        }

        /// <summary>
        /// Constructor. Stores parameter into variables.
        /// </summary>
        public Amplify(ref AmplifyParameters ap)
        {
            mRatio = ap.mRatio;
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
                output[0][i].FloatVal = input[0][i].FloatVal * mRatio;
            }
        }

        /// <summary>
        /// Sets the parameters for Amplify effect
        /// </summary>
        public void SetParameters(ref EffectParameters param)
        {
            AmplifyParameters ap = (AmplifyParameters)param;

            mRatio = ap.mRatio;
        }

        /// <summary>
        /// Returns the parameters for Amplify effect
        /// </summary>
        public EffectParameters GetParameters()
        {
            return (EffectParameters)new AmplifyParameters(mRatio, 0, 0);

        }
    }
}