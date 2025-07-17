using Sonigon;
using UnityEngine;

namespace NoireWallCards
{
    // based on a snippet shared by otDan, slightly expanded for my needs
    internal class AudioController
    {
        private static readonly SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);

        public static void Play(AudioClip audioClip, Transform transform, bool randomPitch = false, float randomPitchRange = 2.0f)
        {
            soundParameterIntensity.intensity = 1f;

            var soundContainer = ScriptableObject.CreateInstance<SoundContainer>();

            soundContainer.setting.volumeIntensityEnable = true;
            soundContainer.setting.pitchRandomEnable = randomPitch;
            soundContainer.setting.pitchRandomRangeSemitone = randomPitchRange;
            soundContainer.audioClip[0] = audioClip;

            var soundEvent = ScriptableObject.CreateInstance<SoundEvent>();

            soundEvent.soundContainerArray[0] = soundContainer;
            soundParameterIntensity.intensity = Optionshandler.vol_Sfx / 1f * Optionshandler.vol_Master;

            SoundManager.Instance.Play(soundEvent, transform, soundParameterIntensity);
        }
    }
}