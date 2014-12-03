
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ModelViewerAux;

#if BIONICK
namespace Bionick.Game
#else
namespace MBL.AnimationBlending
#endif
{
	internal class ClipBone
	{
		public ClipBone(ModelViewerAux.AnimationClip.Bone bone, MeshBone assignedBone)
		{
			AssignedBone = assignedBone;
			Bone = bone;

			SetKeyframes();
			SetPosition(0);
		}

		private int currentKeyframe = 0;

		public Quaternion Rotation
		{
			get;
			private set;
		}
		public Vector3 Translation
		{
			get;
			private set;
		}

		public MeshBone AssignedBone {
			get;
			private set;
		}

		public ModelViewerAux.AnimationClip.Keyframe Keyframe1;
		public ModelViewerAux.AnimationClip.Keyframe Keyframe2;
		public ModelViewerAux.AnimationClip.Bone Bone { get; set; }

		public void SetPosition(float position)
		{
			List<ModelViewerAux.AnimationClip.Keyframe> keyframes = Bone.Keyframes;
			if (keyframes.Count == 0)
				return;

			while (position < Keyframe1.Time && currentKeyframe > 0)
			{
				currentKeyframe--;
				SetKeyframes();
			}

			while (position >= Keyframe2.Time && currentKeyframe < Bone.Keyframes.Count - 2)
			{
				currentKeyframe++;
				SetKeyframes();
			}

			if (Keyframe1 == Keyframe2)
			{
				Rotation = Keyframe1.Rotation;
				Translation = Keyframe1.Translation;
			}
			else
			{
				// Interpolate between keyframes
				float t = (float)((position - Keyframe1.Time) / (Keyframe2.Time - Keyframe1.Time));

				Rotation = Quaternion.Slerp(Keyframe1.Rotation, Keyframe2.Rotation, t);
				Translation = Vector3.Lerp(Keyframe1.Translation, Keyframe2.Translation, t);
			}
		}

		private void SetKeyframes()
		{
			if (Bone.Keyframes.Count > 0)
			{
				Keyframe1 = Bone.Keyframes[currentKeyframe];
				if (currentKeyframe == Bone.Keyframes.Count - 1)
					Keyframe2 = Keyframe1;
				else
					Keyframe2 = Bone.Keyframes[currentKeyframe + 1];
			}
			else
			{
				Keyframe1 = null;
				Keyframe2 = null;
			}
		}
	}

	internal class MeshBone
    {
        MeshBone parent = null;

        public string Name
        {
            get;
            private set;
        }

        public Matrix SkinTransform
		{ 
			get; 
			private set; 
		}

		public Matrix AbsoluteTransform 
		{ 
			get; 
			private set; 
		}

		public MeshBone(string name, Matrix bindTransform, MeshBone parent)
        {
            this.Name = name;
			this.parent = parent;

			ComputeAbsoluteTransform (bindTransform);
			SkinTransform = Matrix.Invert (AbsoluteTransform);
        }

		private void ComputeAbsoluteTransform(Matrix completeTransform)
        {
			if (parent != null)
 				AbsoluteTransform = completeTransform * parent.AbsoluteTransform;
            else
 				AbsoluteTransform = completeTransform;
        }

        public void SetCompleteTransform(Matrix m)
        {
			ComputeAbsoluteTransform (m);
        }
    }
}
