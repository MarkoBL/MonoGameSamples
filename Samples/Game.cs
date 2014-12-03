using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MBL
{
	internal class Game : Microsoft.Xna.Framework.Game
	{
		KeyboardState keyState = new KeyboardState();
		KeyboardState lastKeyState = new KeyboardState();

		public GraphicsDeviceManager DeviceManager {
			get;
			private set;
		}

		public Camera Camera {
			get;
			private set;
		}

		AnimationBlending.AnimatedModel animatedModel;

		SpriteBatch spriteBatch;
		public SpriteFont UiFont {
			get;
			private set;
		}

		public Game () {
			DeviceManager = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void Initialize () {
			base.Initialize ();

			DeviceManager.PreferredBackBufferWidth = 1024;
			DeviceManager.PreferredBackBufferHeight = 768;

			IsMouseVisible = true;
			DeviceManager.ApplyChanges ();

			spriteBatch = new SpriteBatch (GraphicsDevice);
			UiFont = Content.Load<SpriteFont> ("Fonts/DejaVu");

			Camera = new Camera (this);
			animatedModel = new AnimationBlending.AnimatedModel (this);
		}

		protected override void Update (GameTime gameTime)
		{
			base.Update (gameTime);
			lastKeyState = keyState;
			keyState = Keyboard.GetState ();

			float dt = (float)(gameTime.ElapsedGameTime.TotalMilliseconds * 0.001);
			Camera.Update (dt);
			animatedModel.Update (dt);
		}

		protected override void Draw (GameTime gameTime)
		{
			base.Draw (gameTime);
			float dt = (float)(gameTime.ElapsedGameTime.TotalMilliseconds * 0.001);

			GraphicsDevice.Clear (Color.White);
			GraphicsDevice.BlendState = BlendState.Opaque;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;

			GraphicsDevice.SamplerStates [0] = SamplerState.AnisotropicWrap;
			GraphicsDevice.SamplerStates [0].MaxAnisotropy = 16;
			GraphicsDevice.SamplerStates [1] = SamplerState.AnisotropicWrap;
			GraphicsDevice.SamplerStates [1].MaxAnisotropy = 16;
			GraphicsDevice.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;

			animatedModel.Draw (dt);

			spriteBatch.Begin ();
			animatedModel.DrawOverlay (spriteBatch, dt);
			spriteBatch.End ();
		}

		public bool IsKeyPressed(Keys key)
		{
			return (lastKeyState.IsKeyUp (key) && keyState.IsKeyDown (key));
		}

		public bool IsKeyDown(Keys key)
		{
			return keyState.IsKeyDown (key);
		}

		public bool IsKeyUp(Keys key)
		{
			return keyState.IsKeyUp (key);
		}
	}
}

