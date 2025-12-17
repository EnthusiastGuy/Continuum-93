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
                TextureEnabled = true,
                Alpha = 1.0f,  // Vertex alpha (we'll use texture alpha instead)
                VertexColorEnabled = false  // Don't use vertex colors, use texture colors
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
            // First, let base class handle window dragging/resizing
            bool baseHandled = base.HandleInput(mouse, prevMouse);

            var contentRect = ContentRect;
            Point mousePos = new Point(mouse.X, mouse.Y);
            
            // Check if mouse is in content area (not title bar) - matching ContinuumTools bounds check
            // Original checks: X < 8 || X > 8 + 650 || Y < 29 || Y > 29 + 365
            // We adapt this to our window's content area
            if (!contentRect.Contains(mousePos) || mousePos.Y <= Y + TitleBarHeight)
            {
                // Update last mouse position to prevent jump when mouse re-enters area
                // This matches the original behavior where lastMousePosition is always updated
                _lastMousePosition = new Vector3(mouse.X, mouse.Y, mouse.ScrollWheelValue);
                return baseHandled;
            }

            // Handle 3D view input (matching ContinuumTools UpdateInput exactly)
            // Use the passed mouse state instead of Mouse.GetState() for consistency
            var scrollWheelValue = mouse.ScrollWheelValue;

            // Zoom with scroll wheel (matching original)
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

            // Rotate with left mouse button (matching ContinuumTools exactly)
            bool leftJustPressed = mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released;
            
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                _autoRotate = false;
                
                // If button was just pressed, initialize lastMousePosition to current position
                // This prevents jumps when starting a drag
                if (leftJustPressed)
                {
                    _lastMousePosition = currentMousePosition;
                }
                
                // Calculate the change in mouse position
                mouseDelta = currentMousePosition - _lastMousePosition;

                // Update rotation angles based on mouse movement
                _rotationAngles.Y += mouseDelta.X * _rotationSpeed; // Y-axis rotation
                _rotationAngles.X += mouseDelta.Y * _rotationSpeed; // X-axis rotation
                _renderTargetDirty = true;
            }
            else if (mouse.MiddleButton == ButtonState.Pressed)
            {
                // Pan with middle mouse button (matching ContinuumTools exactly)
                bool middleJustPressed = mouse.MiddleButton == ButtonState.Pressed && prevMouse.MiddleButton == ButtonState.Released;
                
                // If button was just pressed, initialize lastMousePosition to current position
                if (middleJustPressed)
                {
                    _lastMousePosition = currentMousePosition;
                }
                
                mouseDelta = currentMousePosition - _lastMousePosition;
                _translation.X += mouseDelta.X * 0.01f;
                _translation.Y -= mouseDelta.Y * 0.01f;
                _renderTargetDirty = true;
            }

            // Always update last mouse position (matching ContinuumTools)
            // This ensures smooth delta calculation on next frame
            _lastMousePosition = currentMousePosition;

            // Return true if we handled input (mouse is in content area)
            // This allows the window to receive focus and continue receiving input
            return true;
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
            // Note: ContinuumTools VirtualScreenTarget doesn't specify depth format
            // but we need depth for proper 3D rendering, so we'll use Depth24
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
            // Clear color buffer (matching ContinuumTools - it clears to Transparent in Painter)
            // For depth, we'll clear to 1.0 (far) so closer objects (lower z) can be drawn
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

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

            // Enable alpha blending to support transparency in layers 1-7
            // Use NonPremultiplied blend state for proper alpha blending with transparent textures
            device.BlendState = BlendState.NonPremultiplied;

            // Draw layers in correct order for transparency:
            // 1. Draw layer 0 first (opaque, furthest back) with depth writes enabled
            // 2. Draw layers 7-1 back-to-front (transparent, closer) with depth writes disabled
            // INVERT z values so layer 0 is furthest: z = (7 - i) * 0.25f
            // This makes layer 0 at z=1.75 (furthest), layer 7 at z=0 (closest)
            
            // First, draw layer 0 (opaque, furthest)
            byte layer0 = 0;
            if (layer0 < Video.PaletteCount && layer0 < Video.Layers.Count && Video.Layers[layer0] != null)
            {
                float z = (7 - layer0) * 0.25f;  // z = 1.75
                Matrix depth = Matrix.CreateTranslation(0, 0, z);
                Matrix finalMatrix = depth * rotationMatrix;

                _basicEffect.World = finalMatrix;
                _basicEffect.Texture = Video.Layers[layer0];

                // Layer 0 is opaque, use normal depth state with writes enabled
                device.DepthStencilState = DepthStencilState.Default;

                _basicEffect.CurrentTechnique.Passes[0].Apply();

                device.RasterizerState = new RasterizerState()
                {
                    CullMode = CullMode.None,
                    MultiSampleAntiAlias = true,
                };

                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertices, 0, 2);
            }

            // Then draw layers 7-1 back-to-front (transparent, closer to camera)
            // For proper transparency with multiple overlapping layers:
            // - Disable depth testing entirely for transparent layers
            //   This ensures all transparent layers render regardless of depth
            // - Disable depth writes (so transparent pixels don't block later layers)
            // - Draw back-to-front so transparency blends correctly
            // Note: We rely on draw order (back-to-front) for correct layering
            device.DepthStencilState = new DepthStencilState
            {
                DepthBufferEnable = false,  // Disable depth testing for transparent layers
                DepthBufferWriteEnable = false  // Don't write depth
            };

            for (int layerIndex = 7; layerIndex >= 1; layerIndex--)
            {
                byte i = (byte)layerIndex;
                if (i < Video.PaletteCount && i < Video.Layers.Count && Video.Layers[i] != null)
                {
                    // Invert z: layer 7 at z=0 (closest), layer 1 at z=1.5
                    float z = (7 - i) * 0.25f;
                    Matrix depth = Matrix.CreateTranslation(0, 0, z);
                    Matrix finalMatrix = depth * rotationMatrix;

                    _basicEffect.World = finalMatrix;
                    _basicEffect.Texture = Video.Layers[i];

                    // Start rendering with the basic effect
                    _basicEffect.CurrentTechnique.Passes[0].Apply();

                    device.RasterizerState = new RasterizerState()
                    {
                        CullMode = CullMode.None,
                        MultiSampleAntiAlias = true,
                    };

                    // Draw the quad
                    device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertices, 0, 2);
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

