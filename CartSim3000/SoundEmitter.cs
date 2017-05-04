using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartSim3000
{
    class SoundEmitter : GameObject
    {
        public SoundEffectInstance soundEffectInstance;
        AudioEmitter emitter;

        public SoundEmitter(SoundEffect effect, Vector3 position)
            : base(position, Quaternion.Identity, 1f, null)
        {
            soundEffectInstance = effect.CreateInstance();
            emitter = new AudioEmitter();
            soundEffectInstance.Volume = 0.2f;
        }

        public void Update(AudioListener listener)
        {
            emitter.Position = Pos;
            soundEffectInstance.Apply3D(listener, emitter);
        }
        public void Play()
        {
            soundEffectInstance.Play();
        }
        public void Stop()
        {
            soundEffectInstance.Stop();
        }
        public void Resume()
        {
            soundEffectInstance.Resume();
        }
        public void Pause()
        {
            soundEffectInstance.Pause();
        }
    }
}
