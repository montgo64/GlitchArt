using Effects;

namespace Reverb
{
    class ReverbParameters : EffectParameters
    {
        double mRoomSize;
        double mPreDelay;
        double mReverberance;
        double mHfDamping;
        double mToneLow;
        double mToneHigh;
        double mWetGain;
        double mDryGain;
        double mStereoWidth;
        bool mWetOnly;

        public ReverbParameters()
        {

        }
    }


    class Reverb : Effect
    {
        private int numchans;
        public EffectParameters GetParameters()
        {
            throw new System.NotImplementedException();
        }

        public void ProcessBlock(ref float[] input, ref float[] output, int length)
        {
            float[] inchans = { 0, 0 };
            float[] outchans = { 0, 0 };

            for int c = 0; c < numchans; c++)
            {
                inchans[c] = input[c];
                outchans[c] = output[c];
            }

            int remaining = length;

            while(remaining != 0)
            {
                for
            }
        }

        public void SetParameters(ref EffectParameters param)
        {
            throw new System.NotImplementedException();
        }
    }
}
