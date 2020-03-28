#region Using Statments

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Scythe
{
    public class Camera2d
    {
        #region Fields

        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation

        #endregion

        #region Initizlize Class

        public Camera2d()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
        }

        #endregion

        #region Zoom

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        #endregion

        #region Rotation

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        #endregion

        #region Position

        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        #endregion

        #region Move

        public void Move(Vector2 amount)
        {
            _pos += amount;
        }


        #endregion

        #region Matrix (Camera Draw)

        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            _transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return _transform;
        }

        #endregion
    }
}
