using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleDecalPool : MonoBehaviour
{
    public int maxDecals = 100;

    int particleDecalDataIndex;
    particleDecalData[] particleData;

	void Start ()
    {
        particleData = new particleDecalData[maxDecals];
        for (int i = 0; i < maxDecals; i++)
        {
            particleData[i] = new particleDecalData();
        }
	}
	
	void SepParticleData(ParticleCollisionEvent particleCollisionEvent)
    {
        if (particleDecalDataIndex >= maxDecals)
        {
            particleDecalDataIndex = 0;
        }

        particleDecalDataIndex++;
    }
}
