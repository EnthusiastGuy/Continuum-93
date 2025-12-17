using Continuum93.Emulator;
using Continuum93.ServiceModule.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Continuum93.ServiceModule.UI
{
    public class VirtualScreen3DWindow : Window
    {
        private BasicEffect _basicEffect;
        private VertexPositionTexture[] _vertices;
        private Vector3 _rotationAngles;
        private Vector3 _translation;
        private readonly float _rotationSpeed = 0.01f;
        private readonly float _zoomSpeed = 0.1f;
        private Vector3 _cameraPosition = new(0, 0, 6);
        private Vector3 _lastMousePosition;
        private bool _autoRotate = true;
        private double _localTimeMS = 0;
        private bool _isInitialized = false;
        private RenderTarget2D _renderTarget;
        private bool _renderTargetDirty = true;

        public VirtualScreen3DWindow(
            string title,
            int x, int y,
            int width, int height,
            float spawnDelaySeconds = 0,
            bool canResize = true,
            bool canClose = false)
            : base(title, x, y, width, height, spawnDelaySeconds, canResize, canClose)
        {
        }

        protected override void UpdateContent(GameTime gameTime)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            Video.Update();

            // Auto-rotate logic
            if (_autoRotate)
            {
                float amplitude = 0.7f;     // The amplitude of rocking
                float frequency = 0.0001f;  // The speed of rocking
                float time = (float)GameTimePlus.GetTotalMs();

                float rockingAngle = amplitude * MathF.Sin(frequency * time);
                _rotationAngles.Y = rockingAngle;
                _renderTargetDirty = true;
            }

            // Render 3D content to render target when needed
            // We render every frame when visible since video layers update continuously
            if (Visible && _isInitialized && _basicEffect != null)
            {
                if (_renderTargetDirty || RefreshRequired)
                {
                    Render3DContent();
                    _renderTargetDirty = false;
                    RefreshRequired = false;
                }
                else
                {
                    // Also render if video layers are available (they update every frame)
                    // This ensures the 3D view stays in sync with video updates
                    Render3DContent();
                }
            }
        }

        private void Initialize()
        {
            var device = Renderer.GetGraphicsDevice();

            // Pre-populate video layers if not already done
            if (Video.Layers.Count == 0)
            {
                Video.InitializeLayers();
            }

            // Create a basic effect
            _basicEffect = new BasicEffect(device)
            {
                TextureEnabled = true
            };

            // Define the vertices of the 3D quad and their UV coordinates
            // Aspect ratio: 480x270 = 16:9, so width:height = 1.777...
            // Scale to match: width 2.40, height 1.35 (2.40/1.35 = 1.777...)
            _vertices = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(-2.40f, 1.35f, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(2.40f, 1.35f, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-2.40f, -1.35f, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(2.40f, -1.35f, 0), new Vector2(1, 1))
            };

            _isInitialized = true;
        }

        public override bool HandleInput(MouseState mouse, MouseState prevMouse)
        {
            // First, let the base class handle window dragging/resizing
            bool baseHandled = base.HandleInput(mouse, prevMouse);

            // Only process 3D view input if mouse is within content area and window is focused
            if (!IsFocused || !Visible)
            {
                // Still update last mouse position to prevent jump when refocusing
                _lastMousePosition = new Vector3(mouse.X, mouse.Y, mouse.ScrollWheelValue);
                return baseHandled;
            }

            var contentRect = ContentRect;
            Point mousePos = new Point(mouse.X, mouse.Y);

            // Only handle 3D input if mouse is in content area (not title bar)
            if (!contentRect.Contains(mousePos))
            {
                _lastMousePosition = new Vector3(mouse.X, mouse.Y, mouse.ScrollWheelValue);
                return baseHandled;
            }

            // Handle 3D view specific input
            var scrollWheelValue = mouse.ScrollWheelValue;

            // Zoom with scroll wheel
            if (scrollWheelValue > _lastMousePosition.Z)
            {
                _cameraPosition.Z -= _zoomSpeed;
                _renderTargetDirty = true;
            }
            else if (scrollWheelValue < _lastMousePosition.Z)
            {
                _cameraPosition.Z += _zoomSpeed;
                _renderTargetDirty = true;
            }

            Vector3 currentMousePosition = new(mouse.X, mouse.Y, scrollWheelValue);
            Vector3 mouseDelta;

            // Rotate with left mouse button (but only if not dragging the window)
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Pressed)
            {
                // Check if we're not in the title bar area
                if (mousePos.Y > Y + TitleBarHeight)
                {
                    _autoRotate = false;
                    mouseDelta = currentMousePosition - _lastMousePosition;

                    // Update rotation angles based on mouse movement
                    _rotationAngles.Y += mouseDelta.X * _rotationSpeed; // Y-axis rotation
                    _rotationAngles.X += mouseDelta.Y * _rotationSpeed; // X-axis rotation
                    _renderTargetDirty = true;
                }
            }
            else if (mouse.MiddleButton == ButtonState.Pressed)
            {
                // Pan with middle mouse button
                mouseDelta = currentMousePosition - _lastMousePosition;
                _translation.X += mouseDelta.X * 0.01f;
                _translation.Y -= mouseDelta.Y * 0.01f;
                _renderTargetDirty = true;
            }

            _lastMousePosition = currentMousePosition;

            // Return true if we handled input, otherwise return base result
            return baseHandled || (mouse.LeftButton == ButtonState.Pressed && mousePos.Y > Y + TitleBarHeight);
        }

        protected override void OnResized()
        {
            base.OnResized();
            // Recreate render target if needed
            if (_renderTarget != null)
            {
                _renderTarget.Dispose();
                _renderTarget = null;
            }
            _renderTargetDirty = true;
        }

        public void Dispose()
        {
            _renderTarget?.Dispose();
            _basicEffect?.Dispose();
        }

        private void Render3DContent()
        {
            if (!_isInitialized || _basicEffect == null)
                return;

            var device = Renderer.GetGraphicsDevice();
            var contentRect = ContentRect;

            // Don't render if content rect is invalid
            if (contentRect.Width <= 0 || contentRect.Height <= 0)
                return;

            // Create render target for 3D rendering
            if (_renderTarget == null || 
                _renderTarget.Width != contentRect.Width || 
                _renderTarget.Height != contentRect.Height)
            {
                _renderTarget?.Dispose();
                _renderTarget = new RenderTarget2D(
                    device,
                    contentRect.Width,
                    contentRect.Height,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.Depth24);
            }

            // Save current render target and viewport
            var oldTargetBindings = device.GetRenderTargets();
            RenderTarget2D oldTarget = oldTargetBindings.Length > 0 
                ? oldTargetBindings[0].RenderTarget as RenderTarget2D 
                : null;
            Viewport oldViewport = device.Viewport;

            // Render 3D scene to render target
            device.SetRenderTarget(_renderTarget);
            device.Clear(Color.Black);

            // Set up viewport for 3D rendering
            device.Viewport = new Viewport(0, 0, contentRect.Width, contentRect.Height);

            // Set up the basic effect
            _basicEffect.View = Matrix.CreateLookAt(_cameraPosition, new Vector3(), Vector3.Up);
            _basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, 
                (float)contentRect.Width / contentRect.Height, 
                0.01f, 
                20);

            // Create rotation matrices
            Matrix rotationX = Matrix.CreateRotationX(_rotationAngles.X);
            Matrix rotationY = Matrix.CreateRotationY(_rotationAngles.Y);
            Matrix rotationZ = Matrix.CreateRotationZ(_rotationAngles.Z);
            Matrix pan = Matrix.CreateTranslation(_translation);

            // Apply the rotation to the world matrix
            Matrix rotationMatrix = pan * rotationX * rotationY * rotationZ;

            // Enable depth testing and blending
            device.DepthStencilState = DepthStencilState.Default;
            device.BlendState = BlendState.AlphaBlend;

            // Draw each layer
            for (byte i = 0; i < 8; i++)
            {
                if (i < Video.PaletteCount && i < Video.Layers.Count && Video.Layers[i] != null)
                {
                    float z = i * 0.25f;
                    Matrix depth = Matrix.CreateTranslation(0, 0, z);
                    Matrix finalMatrix = depth * rotationMatrix;

                    _basicEffect.World = finalMatrix;
                    _basicEffect.Texture = Video.Layers[i];

                    // Start rendering with the basic effect
                    foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        device.RasterizerState = new RasterizerState()
                        {
                            CullMode = CullMode.None,
                            MultiSampleAntiAlias = true,
                        };

                        // Draw the quad
                        device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertices, 0, 2);
                    }
                }
            }

            // Restore viewport
            device.Viewport = oldViewport;

            // Restore render target
            device.SetRenderTarget(oldTarget);
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            if (!_isInitialized || _renderTarget == null)
                return;

            // Just draw the pre-rendered 3D content as a texture
            // The sprite batch is already active, so we can draw directly
            spriteBatch.Draw(_renderTarget, contentRect, Color.White);
        }
    }
}

