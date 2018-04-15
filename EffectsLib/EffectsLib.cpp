// EffectsLib.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <iostream>

extern "C"
{
#include "reverb.h"
	__declspec(dllexport) void __stdcall reverb_helper_def(void* inbuf, void* outbuf, int length)
	{
		std::cout << "in helper func" << std::endl;
		sf_reverb_state_st rv;
		sf_presetreverb(&rv, 44100, SF_REVERB_PRESET_DEFAULT);

		int len = length / 2; //samples are doubles
		while (len > 0)
		{
			std::cout << "len = " << len << std::endl;
			sf_reverb_process(&rv, 128, (sf_sample_st*)inbuf, (sf_sample_st*)outbuf);
			len -= 128;
		}
	}

	__declspec(dllexport) void __stdcall reverb_helper_param(void* inbuf, void* outbuf, int length, int rate,
		int oversamplefactor, float ertolate, float erefwet, float dry, float ereffactor,
		float erefwidth, float width, float wet, float wander, float bassb, float spin, float inputlpf,
		float basslpf, float damplpf, float outputlpf, float rt60, float delay)
	{
		sf_reverb_state_st rv;
		sf_advancereverb(&rv, rate, oversamplefactor, ertolate, erefwet, dry, ereffactor, erefwidth, width,
			wet, wander, bassb, spin, inputlpf, basslpf, damplpf, outputlpf, rt60, delay);

		int len = length / 2; //samples are doubles
		while (len > 0)
		{
			sf_reverb_process(&rv, 128, (sf_sample_st*)inbuf, (sf_sample_st*)outbuf);
			len -= 128;
		}
	}
}

