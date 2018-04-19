using Effects;

namespace PhaserEffect
{
    public struct PhaserState
    {
        public float samplerate;
        public int skipcount;
        public double[] old; // must be as large as MAX_STAGES
        public double gain;
        public double fbout;
        public double outgain;
        public double lfoskip;
        public double phase;
        public int laststages;
    }

    class PhaserParams : EffectParameters
    {
        public int stages;
        public int dryWet;
        public double freq;
        public double phase;
        public int depth;
        public int feedback;
        public double outgain;
        public float samplerate;
    }

    class Phaser : Effect
    {
        const double phaser1foshape = 4.0;
        const int lfoskipsamples = 20;
        const int num_stages = 24;
        PhaserState state;

        int mStages;
        int mDryWet;
        double mFreq;
        double mPhase;
        int mDepth;
        int mFeedback;
        double mOutGain;

        public Phaser()
        {
            state.samplerate = 0;
            state.old = new double[num_stages];
            state.skipcount = 0;
            state.gain = 0;
            state.fbout = 0;
            state.laststages = 0;
            state.outgain = 0;

            mStages = 0;
            mDryWet = 0;
            mFreq = 0;
            mPhase = 10;
            mDepth = 10;
            mFeedback = 10;
            mOutGain = 10;
        }

        public Phaser(PhaserParams param)
        {
            state.samplerate = param.samplerate;
            state.old = new double[num_stages];
            state.skipcount = 0;
            state.gain = 0;
            state.fbout = 0;
            state.laststages = 0;
            state.outgain = 0;

            mStages = param.stages;
            mDryWet = param.dryWet;
            mFreq = param.freq;
            mPhase = param.phase;
            mDepth = param.depth;
            mFeedback = param.feedback;
            mOutGain = param.outgain;
        }


        public EffectParameters GetParameters()
        {
            PhaserParams param = new PhaserParams();

            param.samplerate = state.samplerate;
            param.stages = mStages;
            param.dryWet = mDryWet;
            param.freq = mFreq;
            param.depth = mDepth;
            param.phase = mPhase;
            param.feedback = mFeedback;
            param.outgain = mOutGain;

            return (EffectParameters)param;
        }

        public void ProcessBlock(ref FloatToInt[] input, ref FloatToInt[] output, int length)
        {
            for (int j = state.laststages; j < mStages; j++)
            {
                state.old[j] = 0;
            }
            state.laststages = mStages;
            state.lfoskip = mFreq * 2 * System.Math.PI / state.samplerate;
            state.phase = mPhase * System.Math.PI / 180;
            state.outgain = System.Math.Pow(10.0, mOutGain / 20.0);

            for(int i = 0; i < length; i++)
            {
                double inv = input[i].FloatVal;
                double m = inv + state.fbout * mFeedback / 101;

                if(((double)state.skipcount % lfoskipsamples) == 0)
                {
                    state.gain = (1.0 + System.Math.Cos((double)state.skipcount * state.lfoskip + state.phase)) / 2.0;
                    state.gain = expm1(state.gain * phaser1foshape) / expm1(phaser1foshape);
                    state.gain = 1.0 - state.gain / 255.0 * mDepth;
                }

                for(int j = 0; j < mStages; j++)
                {
                    double tmp = state.old[j];
                    state.old[j] = state.gain * tmp + m;
                    m = tmp - state.gain * state.old[j];
                }
                state.fbout = m;

                output[i].FloatVal = (float)(state.outgain * (m * mDryWet + inv * (255 - mDryWet)) / 255);

            }
        }

        public void SetParameters(ref EffectParameters p)
        {
            PhaserParams param = (PhaserParams)p;
            state.samplerate = param.samplerate;
            state.old = new double[num_stages];
            state.skipcount = 0;
            state.gain = 0;
            state.fbout = 0;
            state.laststages = 0;
            state.outgain = 0;

            mStages = param.stages;
            mDryWet = param.dryWet;
            mFreq = param.freq;
            mPhase = param.phase;
            mDepth = param.depth;
            mFeedback = param.feedback;
            mOutGain = param.outgain;
        }

        double expm1(double input)
        {
            if (System.Math.Abs(input) < 0.00001)
                return input + 0.5 * input * input;
            else
                return System.Math.Exp(input) - 1.0;
        }
    }
}
