using Continuum93.Emulator;
using Continuum93.ServiceModule;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Continuum93.ServiceModule.UI
{
    public class Video3DWindow : Window
    {
        private BasicEffect _basicEffect;
        private VertexPositionTexture[] _vertices;
        private Vector3 _rotationAngles;
        private Vector3 _translation;
        private const float RotationSpeed = 0.01f;
        private const float ZoomSpeed = 0.1f;
        private Vector3 _cameraPosition = new(0, 0, 6);
        private Vector3 _lastMousePosition;
        private bool _autoRotate = true;
        private double _localTimeMS = 0;
        private RenderTarget2D _renderTarget;

        public Video3DWindow(
            string title,
            int x, int y,
            int width, int height,
            float spawnDelaySeconds = 0,
            bool canResize = true,
            bool canClose = false)
            : base(title, x, y, width, height, spawnDelaySeconds, canResize, canClose)
        {
            Initialize();
        }

        private void Initialize()
        {
            // Initialize video layers
            Video.InitializeLayers();

            // Create a basic effect
            _basicEffect = new BasicEffect(Renderer.GetGraphicsDevice())
            {
                TextureEnabled = true
            };

            // Define the vertices of the 3D quad and their UV coordinates
            _vertices = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(-2.40f, 1.35f, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(2.40f, 1.35f, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-2.40f, -1.35f, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(2.40f, -1.35f, 0), new Vector2(1, 1))
            };

            // Render target will be created when window is fully spawned
            _renderTarget = null;
        }

        protected override void UpdateContent(GameTime gameTime)
        {
            // Ensure render target is created once window has valid size
            if (_renderTarget == null && ContentRect.Height > 0 && ContentRect.Width > 0)
            {
                int width = Math.Max(1, ContentRect.Width - Padding * 2);
                int height = Math.Max(1, ContentRect.Height - Padding * 2);
                
                _renderTarget = new RenderTarget2D(
                    Renderer.GetGraphicsDevice(),
                    width,
                    height,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.Depth24);
            }

            // Update video data only if service view is active
            if (Service.STATE.UseServiceView)
            {
                Video.Update();
            }

            // Auto-rotate
            if (_autoRotate)
            {
                float amplitude = 0.7f;
                float frequency = 0.0001f;
                float time = (float)gameTime.TotalGameTime.TotalMilliseconds;

                float rockingAngle = amplitude * MathF.Sin(frequency * time);
                _rotationAngles.Y = rockingAngle;
            }

            // Handle input if window is focused
            if (IsFocused)
            {
                HandleInput(gameTime);
            }
        }

        private void HandleInput(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var scrollWheelValue = mouseState.ScrollWheelValue;

            // Zoom with scroll wheel
            if (ContentRect.Contains(new Point(mouseState.X, mouseState.Y)))
            {
                if (scrollWheelValue > _lastMousePosition.Z)
                {
                    _cameraPosition.Z -= ZoomSpeed;
                }
                else if (scrollWheelValue < _lastMousePosition.Z)
                {
                    _cameraPosition.Z += ZoomSpeed;
                }

                Vector3 currentMousePosition = new(mouseState.X, mouseState.Y, scrollWheelValue);
                Vector3 mouseDelta;

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    _autoRotate = false;
                    mouseDelta = currentMousePosition - _lastMousePosition;

                    // Update rotation angles based on mouse movement
                    _rotationAngles.Y += mouseDelta.X * RotationSpeed; // Y-axis rotation
                    _rotationAngles.X += mouseDelta.Y * RotationSpeed; // X-axis rotation
                }
                else if (mouseState.MiddleButton == ButtonState.Pressed)
                {
                    mouseDelta = currentMousePosition - _lastMousePosition;
                    _translation.X += mouseDelta.X * 0.01f;
                    _translation.Y -= mouseDelta.Y * 0.01f;
                }
                else
                {
                    // Re-enable auto-rotate when mouse is released
                    _autoRotate = true;
                }

                _lastMousePosition = currentMousePosition;
            }
        }

        protected override void OnResized()
        {
            base.OnResized();
            
            // Recreate render target with new size (only if dimensions are valid)
            if (ContentRect.Height > 0 && ContentRect.Width > 0)
            {
                if (_renderTarget != null)
                {
                    _renderTarget.Dispose();
                }
                
                int width = Math.Max(1, ContentRect.Width - Padding * 2);
                int height = Math.Max(1, ContentRect.Height - Padding * 2);
                
                _renderTarget = new RenderTarget2D(
                    Renderer.GetGraphicsDevice(),
                    width,
                    height,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.Depth24);
            }
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            // Don't draw if render target isn't ready or window isn't fully spawned
            if (_renderTarget == null || ContentRect.Height <= 0 || ContentRect.Width <= 0)
                return;

            if (Video.Layers == null || Video.PaletteCount == 0)
                return;

            // Note: spriteBatch is already active from DrawContentClipped
            // We need to end it temporarily to do 3D rendering
            spriteBatch.End();

            var device = Renderer.GetGraphicsDevice();
            var oldViewport = device.Viewport;
            var oldRenderTarget = device.GetRenderTargets();

            // Set render target
            device.SetRenderTarget(_renderTarget);
            device.Clear(Color.Black);

            // Set up viewport for 3D rendering
            device.Viewport = new Viewport(0, 0, _renderTarget.Width, _renderTarget.Height);

            // Set up the basic effect
            _basicEffect.View = Matrix.CreateLookAt(_cameraPosition, new Vector3(), Vector3.Up);
            _basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, 
                (float)_renderTarget.Width / _renderTarget.Height, 
                0.01f, 
                20);

            // Create rotation matrices
            Matrix rotationX = Matrix.CreateRotationX(_rotationAngles.X);
            Matrix rotationY = Matrix.CreateRotationY(_rotationAngles.Y);
            Matrix rotationZ = Matrix.CreateRotationZ(_rotationAngles.Z);
            Matrix pan = Matrix.CreateTranslation(_translation);

            // Apply the rotation to the world matrix
            Matrix rotationMatrix = pan * rotationX * rotationY * rotationZ;

            // Set rasterizer state
            device.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                MultiSampleAntiAlias = true,
            };

            device.DepthStencilState = DepthStencilState.Default;
            device.BlendState = BlendState.AlphaBlend;

            // Draw each layer
            for (byte i = 0; i < Video.PaletteCount && i < 8; i++)
            {
                if (Video.Layers[i] == null)
                    continue;

                float z = i * 0.25f;
                Matrix depth = Matrix.CreateTranslation(0, 0, z);
                Matrix finalMatrix = depth * rotationMatrix;

                _basicEffect.World = finalMatrix;
                _basicEffect.Texture = Video.Layers[i];

                // Start rendering with the basic effect
                foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Draw the quad
                    device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertices, 0, 2);
                }
            }

            // Restore render target and viewport
            device.SetRenderTargets(oldRenderTarget);
            device.Viewport = oldViewport;

            // Restart sprite batch for drawing the render target
            // Use the same rasterizer state that DrawContentClipped set up (with scissor test)
            var raster = new RasterizerState() { ScissorTestEnable = true };
            spriteBatch.Begin(
                rasterizerState: raster,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp
            );

            // Draw the render target to the window content
            spriteBatch.Draw(
                _renderTarget,
                new Rectangle(
                    contentRect.X + Padding,
                    contentRect.Y + Padding,
                    contentRect.Width - Padding * 2,
                    contentRect.Height - Padding * 2),
                Color.White);
        }

        // Cleanup resources
        public void Dispose()
        {
            _renderTarget?.Dispose();
            _basicEffect?.Dispose();
        }
    }
}

