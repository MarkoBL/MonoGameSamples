using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ModelViewerAux
{
    /// <summary>
    /// An animation clip is a set of keyframes with associated bones.
    /// </summary>
    public class AnimationClip
    {
        #region Keyframe and Bone nested class

        /// <summary>
        /// An Keyframe is a rotation and translation for a moment in time.
        /// </summary>
        public class Keyframe
        {
            public float Time;             // The keyframe time
            public Quaternion Rotation;     // The rotation for the bone
            public Vector3 Translation;     // The translation for the bone

            public void SetTransform(Matrix transform)
            {
                Vector3 Scale = new Vector3(transform.Right.Length(), transform.Up.Length(), transform.Backward.Length());

                transform.Right = transform.Right / Scale.X;
                transform.Up = transform.Up / Scale.Y;
                transform.Backward = transform.Backward / Scale.Z;
                Rotation = Quaternion.CreateFromRotationMatrix(transform);

                Translation = transform.Translation;
            }
        }

        /// <summary>
        /// Keyframes are grouped per bone for an animation clip
        /// </summary>
        public class Bone
        {
            private string name = "";

            private List<Keyframe> keyframes = new List<Keyframe>();

            /// <summary>
            /// The bone name for these keyframes
            /// </summary>
            public string Name { get { return name; } set { name = value; } }

            /// <summary>
            /// The keyframes for this bone
            /// </summary>
            public List<Keyframe> Keyframes { get { return keyframes; } }
        }

        #endregion

        /// <summary>
        /// The bones for this animation
        /// </summary>
        private List<Bone> bones = new List<Bone>();

        /// <summary>
        /// Name of the animation clip
        /// </summary>
        public string Name;

        /// <summary>
        /// Duration of the animation clip
        /// </summary>
        public float Duration;

        /// <summary>
        /// The bones for this animation clip with their keyframes
        /// </summary>
        public List<Bone> Bones { get { return bones; } }
    }
}
