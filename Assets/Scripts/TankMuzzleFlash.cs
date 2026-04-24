using UnityEngine;

public class TankMuzzleFlash : MonoBehaviour {
    [SerializeField] private ParticleSystem[] muzzleFlashSystems;

    public void PlayFlash() {
        if (muzzleFlashSystems == null || muzzleFlashSystems.Length == 0)
            return;

        for (int i = 0; i < muzzleFlashSystems.Length; i++) {
            if (muzzleFlashSystems[i] == null)
                continue;

            muzzleFlashSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlashSystems[i].Play();
        }
    }
}