using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MBL.AnimationBlending
{
	internal class AnimatedModel
	{
		Game game;

		Model model;
		BlendAnimator animator;

		AnimationClip runAnimation;
		AnimationClip jumpAnimation;
		AnimationClip slideAnimation;
		AnimationClip dashAnimation;

		BlendData runBlend;
		BlendData jumpBlend;
		BlendData slideBlend;
		BlendData dashBlend;

		public AnimatedModel (Game game)
		{
			this.game = game;

			model = game.Content.Load<Model> ("Models/Sheep");

			foreach (ModelMesh mesh in model.Meshes) {
				SkinnedEffect skinnedEffect = mesh.Effects [0] as SkinnedEffect;
				if (mesh.Name == "HardSurface")
					skinnedEffect.Texture = game.Content.Load<Texture2D> ("Textures/SheepHS_dDo_d");
				else if (mesh.Name == "Organic")
					skinnedEffect.Texture = game.Content.Load<Texture2D> ("Textures/Sheep_Diffuse");
			}

			animator = new BlendAnimator (model, game);

			float blendDuration = 0.25f;

			runAnimation = animator.AddAnimationClip ("SheepRun");
			runBlend = new BlendData (runAnimation, blendDuration);

			jumpAnimation = animator.AddAnimationClip ("SheepJump");
			jumpBlend = new BlendData (jumpAnimation, blendDuration);

			slideAnimation = animator.AddAnimationClip ("SheepSlide");
			slideBlend = new BlendData (slideAnimation, blendDuration);

			dashAnimation = animator.AddAnimationClip ("SheepDash");
			dashBlend = new BlendData (dashAnimation, blendDuration);

			animator.BlendAnimationClip (runBlend);
		}

		public void Update(float dt)
		{
			if (game.IsKeyPressed (Keys.A))
				animator.BlendAnimationClip (runBlend);
			else if (game.IsKeyPressed (Keys.S))
				animator.BlendAnimationClip (jumpBlend);
			else if (game.IsKeyPressed (Keys.D))
				animator.BlendAnimationClip (slideBlend);
			else if (game.IsKeyPressed (Keys.F))
				animator.BlendAnimationClip (dashBlend);

			animator.Update (dt);
		}

		public void Draw(float dt)
		{
			Camera camera = game.Camera;

			foreach (ModelMesh mesh in model.Meshes) {
				SkinnedEffect skinnedEffect = mesh.Effects [0] as SkinnedEffect;
				skinnedEffect.View = camera.View;
				skinnedEffect.Projection = camera.Projection;
				skinnedEffect.World = animator.MeshTransforms[mesh.ParentBone.Index] * Matrix.Identity;
				skinnedEffect.SetBoneTransforms(animator.SkeletonTransforms);
				skinnedEffect.AmbientLightColor = new Vector3(1f, 1f, 1f);

				mesh.Draw();
			}
		}

		public void DrawOverlay(SpriteBatch spriteBatch, float dt)
		{
			spriteBatch.DrawString (game.UiFont, "A) Run", new Vector2 (10f, 10f), Color.Black);
			spriteBatch.DrawString (game.UiFont, "S) Jump", new Vector2 (10f, 40f), Color.Black);
			spriteBatch.DrawString (game.UiFont, "D) Slide", new Vector2 (10f, 70f), Color.Black);
			spriteBatch.DrawString (game.UiFont, "F) Dash", new Vector2 (10f, 100f), Color.Black);
		}
	}
}

