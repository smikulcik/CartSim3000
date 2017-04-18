using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingSpree
{
    class LArmAnimations
    {
        private float state;
        private float duration = 1.5f;
        private bool isPlaying = false;

        GameObject larm;

    
        public LArmAnimations(GameObject larm)
        {
            state = 0;
            this.larm = larm;
        }

        /// <summary>
        /// Starts the animation for one time
        /// </summary>
        public void Play()
        {
            state = 0;
            isPlaying = true;
        }

        /// <summary>
        /// Updates the game object every update cycle
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (isPlaying)
            {
                state += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(state > duration)
                {
                    state = 0;
                    isPlaying = false;
                }
                larm.Rotation = slerp(state/duration);
            }
        }

        static Quaternion rest = (
            Quaternion.CreateFromAxisAngle(Vector3.Right, .8f) *
            Quaternion.CreateFromAxisAngle(Vector3.Up, -.5f) *
            Quaternion.CreateFromAxisAngle(Vector3.Forward, -MathHelper.PiOver2) *
            Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi)
        );

        static Quaternion[] frames = new Quaternion[] {
            rest,
            Quaternion.CreateFromAxisAngle(Vector3.Right, -MathHelper.PiOver2) *
            Quaternion.CreateFromAxisAngle(Vector3.Up, .5f) *
            Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi),

            Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2) *
            Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi),

            Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi),
            rest,
        };
        
        public static Quaternion slerp(float percent)
        {
            float amt;

            int i = (int)(percent * (frames.Length-1));
            amt = percent * (frames.Length-1) - i;  //amount to slerp between ith frame and ith+1 frame
            return Quaternion.Slerp(frames[i], frames[i+1], amt);
        }
    }
}
