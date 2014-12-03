//#define BIONICK

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModelViewerAux;

#if BIONICK
namespace Bionick.Game
#else
namespace MBL.AnimationBlending
#endif
{
	internal class AnimationClip
	{
		public AnimationClip(string name, ClipBone[] clipBones, ModelViewerAux.AnimationClip clip)
		{
			Name = name;
			ClipBones = clipBones;
			Clip = clip;
		}

		public string Name {
			get;
			private set;
		}

		public ModelViewerAux.AnimationClip Clip {
			get;
			private set;
		}

		public ClipBone[] ClipBones;
		public float SpeedModifier = 1f;
	}

	internal class BlendData
	{
		public BlendData(AnimationClip animation, float blendDuration = 0.1f)
		{
			Animation = animation;
			BlendDurtion = blendDuration;
		}

		public AnimationClip Animation;
		public float BlendDurtion = 0.1f;

		public float StopPreviousAnimationTime = 0f;
		//public float StartAnimationTime = 0f;
	}

	internal class BlendAnimator
    {
		class AnimationInfo
		{
			public AnimationClip Animation;
			public BlendData BlendInfo;

			public float Position = 0f;
			public bool Playing = true;

			public float BlendAmount = 0f;
		}

		class AnimatorInfo
		{
			public ModelExtra ModelExtra = null;
			public ClipBone[] ClipBones = null;
		}

		Dictionary<string, AnimatorInfo> animationClips = new Dictionary<string, AnimatorInfo>();
		List<AnimationInfo> animationInfoList = new List<AnimationInfo> ();

		ModelExtra modelExtra = null;
		List<MeshBone> meshBones = new List<MeshBone>();

		public Matrix[] MeshTransforms
		{
			get;
			private set;
		}

		public Matrix[] SkeletonTransforms
		{
			get;
			private set;
		}

		#if BIONICK
		public BlendAnimator(ModelInfo info)
		{
			Model model = info.Model;
			Initalize(model);

			foreach (MeshInfo mi in info.MeshInfoList)
			{
				MeshInfoSkinned skinned = mi as MeshInfoSkinned;
				if (skinned != null)
					skinned.Animator = this;
			}
		}
		#else
		private Game game;

		public BlendAnimator(Model model, Game game)
		{
			this.game = game;
			Initalize (model);
		}
		#endif

		private void Initalize(Model model)
		{
			modelExtra = model.Tag as ModelExtra;

			ObtainBones(model);

			MeshTransforms = new Matrix[meshBones.Count];
			SkeletonTransforms = new Matrix[modelExtra.Skeleton.Count];
		}

		private void ObtainBones(Model model)
		{
			foreach (ModelBone bone in model.Bones)
			{
				MeshBone newBone = new MeshBone(bone.Name, bone.Transform, bone.Parent != null ? meshBones[bone.Parent.Index] : null);
				meshBones.Add(newBone);
			}
		}

		public MeshBone FindBone(string name)
		{
			foreach(MeshBone bone in meshBones)
			{
				if (bone.Name == name)
					return bone;
			}

			return null;
		}

		public AnimationClip AddAnimationClip(string name)
		{
			AnimatorInfo clip = null;
			if (animationClips.TryGetValue (name, out clip)) {
				return new AnimationClip (name, clip.ClipBones, clip.ModelExtra.Clips[0]);
			}
			#if BIONICK
			Model model = Bionick.Game.Base.Game.Instance.Content.Load<Model>("Models/Animation/" + name);
			#else
			Model model = game.Content.Load<Model>("Animations/" + name);
			#endif
			if (model == null)
				return null;

			ModelExtra extra = model.Tag as ModelExtra;
			if (extra == null)
				return null;

			if (extra.Clips.Count != 1)
				return null;

			int boneCount = extra.Clips[0].Bones.Count;
			ClipBone[] clipBones = new ClipBone[boneCount];

			for(int i = 0; i < clipBones.Length; i++) {
				ModelViewerAux.AnimationClip.Bone bone = extra.Clips[0].Bones[i];
				clipBones[i] = new ClipBone(bone, FindBone(bone.Name));
			}

			clip = new AnimatorInfo ();
			clip.ModelExtra = extra;
			clip.ClipBones = clipBones;

			animationClips.Add (name, clip);

			AnimationClip info = new AnimationClip (name, clip.ClipBones, clip.ModelExtra.Clips[0]);
			return info;
		}

		private AnimationInfo GetLastAnimation()
		{
			if (animationInfoList.Count > 0) {
				return animationInfoList[animationInfoList.Count - 1];
			}

			return null;
		}

		public bool BlendAnimationClip(BlendData blendData)
		{
			AnimationInfo info = new AnimationInfo ();
			info.Animation = blendData.Animation;
			info.BlendInfo = blendData;

			AnimationInfo last = GetLastAnimation ();
			if (last == null) {
				info.BlendAmount = 1f;
			}
			if ((last == null) || (last.Animation != info.Animation)) {
				animationInfoList.Add (info);
				return true;
			}

			return false;
		}

		public void Update(float dt)
		{
			int count = animationInfoList.Count;
			if (count == 0)
				return;

			// update all animations and remove unused
			AnimationInfo previous = null;
			int removeIndex = -1;
			for (int i = (count - 1); i >= 0; i--) {

				if (i > 0)
					previous = animationInfoList[i - 1];
				else
					previous = null;

				AnimationInfo animationInfo = animationInfoList[i];
				AnimationClip animation = animationInfo.Animation;

				animationInfo.Position += animation.SpeedModifier * dt;

				if (animationInfo.Position >= animationInfo.BlendInfo.BlendDurtion || animationInfo.BlendInfo.BlendDurtion == 0f)
					animationInfo.BlendAmount = 1f;
				else {
					animationInfo.BlendAmount = Math.Min (1f, 1f / animationInfo.BlendInfo.BlendDurtion * animationInfo.Position);
				}

				// if an animation is fully blended in, remove the previous animations
				if (removeIndex < 0 && animationInfo.BlendAmount >= 1f) {
					removeIndex = i - 2;
				}

				if (previous != null && previous.Playing) {

					if (previous.BlendInfo.StopPreviousAnimationTime < animationInfo.Position) {
						previous.Playing = false;
					}
				}
			}

			// remove old animations
			if (removeIndex >= 0) {
				for(int i = 0; i <= removeIndex; i++)
					animationInfoList.RemoveAt(0);
			}


			AnimationInfo info1 = animationInfoList[0];
			count = meshBones.Count;
			for (int i = 0; i < info1.Animation.ClipBones.Length; i++) {

				// get the base rotation and translation
				AnimationClip animation1 = info1.Animation;

				ClipBone clipBone1 = animation1.ClipBones[i];
				MeshBone meshBone = clipBone1.AssignedBone;
				if (info1.Playing)
					clipBone1.SetPosition (info1.Position % animation1.Clip.Duration);

				Quaternion rotation = clipBone1.Rotation;
				Vector3 translation = clipBone1.Translation;

				// blend all following animations
				for(int j = 1; j < animationInfoList.Count; j++) {
					AnimationInfo info2 = animationInfoList[j];
					AnimationClip animation2 = info2.Animation;

					ClipBone clipBone2 = animation2.ClipBones[i];
					if (info2.Playing)
						clipBone2.SetPosition (info2.Position % animation2.Clip.Duration);

					rotation = Quaternion.Slerp(rotation, clipBone2.Rotation, info2.BlendAmount);
					translation = Vector3.Lerp(translation, clipBone2.Translation, info2.BlendAmount);
				}

				// create the transform matrix and apply it to the bone
				Matrix m = Matrix.CreateFromQuaternion(rotation);
				m.Translation = translation;
				meshBone.SetCompleteTransform(m);

				MeshTransforms [i] = meshBone.AbsoluteTransform;
			}

			for (int i = 0; i < meshBones.Count; i++)
			{
				MeshBone bone = meshBones[i];
				MeshTransforms[i] = bone.AbsoluteTransform;
			}

			for (int s = 0; s < modelExtra.Skeleton.Count; s++)
			{
				MeshBone bone = meshBones[modelExtra.Skeleton[s]];
				SkeletonTransforms[s] = bone.SkinTransform * bone.AbsoluteTransform;
			}
		}
    }
}
