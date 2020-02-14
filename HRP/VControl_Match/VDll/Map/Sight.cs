using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;

namespace VDll
{
    public class Sight {
		public float Pitch { get; set; }
		public float Facing { get; set; }
		public Vector3 Location { get { return location; } set {location = value; } }
		public Matrix4 Matrix { get { return matrix; } }

		private Matrix4 matrix;
		private Vector3 location;
		private Vector3 up = Vector3.UnitY;
		private MouseState previous;

		public static Sight Instance { 
			get { return instance == null ? (instance = new Sight()) : instance; } 
		}
		private static Sight instance = null;

		private Sight() {
            matrix = Matrix4.Identity;
			Pitch = -2f;
			Facing = 4f;//-MathHelper.PiOver6;
			location = new Vector3(1000f, 1000f, 1000f);
		}

		public void LoadMatrix() {
			Matrix4 cameraMatrix = Matrix;
			GL.LoadMatrix(ref cameraMatrix);
		}

        public Vector3 GetLookAtPoint()
        {
            Vector3 lookatPoint = new Vector3((float)Math.Cos(Facing), (float)Math.Sin(Pitch / 2), (float)Math.Sin(Facing));
            return lookatPoint;
        }

		public void Update(bool focused = false) {
			if (!focused) return;
			KeyboardState Keyboard = OpenTK.Input.Keyboard.GetState();
			float speed = 15f;
			if (Keyboard[Key.W] || Keyboard[Key.Up]) {
				location.X += (float)Math.Cos(Facing) * speed;
				location.Z += (float)Math.Sin(Facing) * speed;
			}

			if (Keyboard[Key.S] || Keyboard[Key.Down]) {
				location.X -= (float)Math.Cos(Facing) * speed;
				location.Z -= (float)Math.Sin(Facing) * speed;
			}

			if (Keyboard[Key.A] || Keyboard[Key.Left]) {
				location.X -= (float)Math.Cos(Facing + Math.PI / 2) * speed;
				location.Z -= (float)Math.Sin(Facing + Math.PI / 2) * speed;
			}

			if (Keyboard[Key.D] || Keyboard[Key.Right]) {
				location.X += (float)Math.Cos(Facing + Math.PI / 2) * speed;
				location.Z += (float)Math.Sin(Facing + Math.PI / 2) * speed;
			}
			if (Keyboard[Key.R]) {
                location.Y += speed;
			}
			if (Keyboard[Key.F]) {
				location.Y -= speed;
			}
			if (Keyboard[Key.Q]) {
				Facing += (float)Math.PI * 0.01f;
			}
			if (Keyboard[Key.E]) {
				Facing -= (float)Math.PI * 0.01f;
			}

			float sensitivity = 0.0075f;
			int xdelta = 0, ydelta = 0, zdelta = 0;
			var current = OpenTK.Input.Mouse.GetState();
            zdelta = current.Wheel - previous.Wheel;
			if (current[MouseButton.Left] || zdelta != 0) { // track pad right button
				if (!current.Equals(previous)) {
					xdelta = current.X - previous.X;
					ydelta = current.Y - previous.Y;
					//zdelta = current.Wheel - previous.Wheel;
					//Console.WriteLine("{0}, {1}, {2}", xdelta, ydelta, zdelta);
					Pitch -= ydelta * sensitivity;
					if (Pitch > Math.PI) Pitch = (float)Math.PI;
					if (Pitch < -Math.PI) Pitch = -(float)Math.PI;
					Facing += xdelta * sensitivity;
					if (Facing > Math.PI) Facing -= (float)(Math.PI * 2f);
					if (Facing < Math.PI) Facing += (float)(Math.PI * 2f);
					location.Y += zdelta * 20f;
					//Console.WriteLine("pitch: {0}, facing: {1}", Pitch, Facing);
				}
			}
			previous = current;

			Vector3 lookatPoint = new Vector3((float)Math.Cos(Facing), (float)Math.Sin(Pitch / 2), (float)Math.Sin(Facing));
            Vector3 target = Location + lookatPoint;
			matrix = Matrix4.LookAt(Location, target, up); // eye, target, up

			Facing = Facing % (2f * (float)Math.PI);
			if (Facing > Math.PI * 2f) Facing -= (float)(Math.PI * 2f);
			if (Facing < 0) Facing += (float)(Math.PI * 2f);

		}
	}


}