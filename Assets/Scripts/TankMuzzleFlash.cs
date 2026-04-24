using UnityEngine;

public class TankMuzzleFlash : MonoBehaviour {
    [SerializeField] private ParticleSystem[] systems;

    public void PlayFlash() {
        if (systems == null || systems.Length == 0)
            return;

        for (int i = 0; i < systems.Length; i++) {
            if (systems[i] == null)
                continue;

            systems[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            systems[i].Play(true);
        }
    }
}