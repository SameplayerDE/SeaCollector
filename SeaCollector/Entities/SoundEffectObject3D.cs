using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SeaCollector.Framework;

namespace SeaCollector.Entities
{
    public class SoundEffectObject3D : GameObject3D
    {
        public AudioEmitter Emitter;

        public string SoundEffectFile;
        
        public SoundEffect SoundEffect;
        //public SoundEffectInstance SoundEffectInstance;

        public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            SoundEffect = contentManager.Load<SoundEffect>(SoundEffectFile);
            //SoundEffectInstance = SoundEffect.CreateInstance();
            
            Emitter = new AudioEmitter();
            
            
            base.LoadContent(graphicsDevice, contentManager);
        }

        public void Play()
        {
            SoundEffect.Play();
        }
        
        public void Stop()
        {
            SoundEffect.Play();
        }
        
    }
}