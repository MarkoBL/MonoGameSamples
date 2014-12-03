using System;
using Microsoft.Xna.Framework;

namespace MBL
{
	internal class Camera 
	{
		public Matrix View
		{
			get;
			private set;
		}

		public Matrix Projection 
		{
			get;
			private set;
		}

		public Vector3 Position = new Vector3 (2, 1, 2.5f);
		public Vector3 LookAt = new Vector3 (0, 1, 0);

		private Game game;

		public Camera(Game game)
		{
			this.game = game;
		}

		public void Update(float dt)
		{
			// It's not required to calculate the View and Projection every frame, but I'm just lazy for this sample code
			float aspectRatio = game.GraphicsDevice.Viewport.AspectRatio;
			Projection = Matrix.CreatePerspectiveFieldOfView(1.0f, 1.0f * aspectRatio, 1.0f, 50.0f);
			View = Matrix.CreateLookAt(Position, LookAt, Vector3.Up);
		}
	}
}

