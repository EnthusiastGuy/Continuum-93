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
        private readonly DepthStencilState _transparentDepthState = new DepthStencilState
        {
            DepthBufferEnable = true,
            DepthBufferWriteEnable = false
        };
        private Texture2D[] _layerLabelTextures;
        private VertexPositionTexture[] _labelVertices;
        private const float LabelBaseY = 1.1f;
        private const float LabelStepY = -0.32f;
        // Place label just outside the quad's left edge (quad spans -2.4..2.4 in X)
        private const float LabelX = -2.55f;
        private const float LabelZOffsetTowardCamera = -0.0005f;

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

            BuildLabelGeometry();
            GenerateLayerLabelTextures();
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
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

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
            device.BlendState = BlendState.NonPremultiplied;

            // Draw each layer
            int layerCount = Math.Min(Video.PaletteCount, 8);

            for (byte drawIndex = 0; drawIndex < layerCount; drawIndex++)
            {
                int sourceIndex = MapLayerIndex(drawIndex, layerCount);
                if (sourceIndex < 0 || sourceIndex >= Video.Layers.Count)
                    continue;

                if (Video.Layers[sourceIndex] == null)
                    continue;

                float z = drawIndex * 0.25f;
                Matrix depth = Matrix.CreateTranslation(0, 0, z);
                Matrix finalMatrix = depth * rotationMatrix;

                _basicEffect.World = finalMatrix;
                _basicEffect.Texture = Video.Layers[sourceIndex];

                // Opaque background writes depth; transparent overlays only read depth
                device.DepthStencilState = drawIndex == 0 ? DepthStencilState.Default : _transparentDepthState;

                foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    // Draw the quad
                    device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertices, 0, 2);
                }

                DrawLayerLabel(device, (byte)sourceIndex, z, rotationMatrix, drawIndex == 0 ? DepthStencilState.Default : _transparentDepthState);
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
            if (_layerLabelTextures != null)
            {
                foreach (var tex in _layerLabelTextures)
                {
                    tex?.Dispose();
                }
            }
        }

        private void BuildLabelGeometry()
        {
            _labelVertices = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(-0.35f, 0.15f, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3( 0.35f, 0.15f, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-0.35f,-0.15f, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3( 0.35f,-0.15f, 0), new Vector2(1, 1))
            };
        }

        private void GenerateLayerLabelTextures()
        {
            var device = Renderer.GetGraphicsDevice();
            var spriteBatch = Renderer.GetSpriteBatch();
            var font = Fonts.ModernDOS_12x18 ?? Fonts.ModernDOS_10x16 ?? Fonts.ModernDOS_10x15;

            if (device == null || spriteBatch == null || font == null)
                return;

            _layerLabelTextures = new Texture2D[8];

            var oldTargets = device.GetRenderTargets();
            var oldViewport = device.Viewport;

            for (int i = 0; i < 8; i++)
            {
                var rt = new RenderTarget2D(device, 64, 32, false, SurfaceFormat.Color, DepthFormat.None);
                device.SetRenderTarget(rt);
                device.Clear(Color.Transparent);

                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.NonPremultiplied,
                    SamplerState.PointClamp,
                    DepthStencilState.None,
                    RasterizerState.CullNone);

                string label = i.ToString();
                var (w, h, _) = font.MeasureText(label, 0, 0);
                float x = (rt.Width - w) * 0.5f;
                float y = (rt.Height - h) * 0.5f;

                font.DrawString(
                    label,
                    x,
                    y,
                    Color.Yellow,
                    rt.Width,
                    0,
                    Color.Black,
                    0);

                spriteBatch.End();

                device.SetRenderTargets(oldTargets);
                device.Viewport = oldViewport;

                _layerLabelTextures[i] = rt;
            }
        }

        private void DrawLayerLabel(GraphicsDevice device, byte layerIndex, float layerZ, Matrix rotationMatrix, DepthStencilState depthState)
        {
            if (_layerLabelTextures == null ||
                layerIndex >= _layerLabelTextures.Length ||
                _layerLabelTextures[layerIndex] == null ||
                _labelVertices == null)
                return;

            var texture = _layerLabelTextures[layerIndex];
            if (texture == null)
                return;

            float z = layerZ + LabelZOffsetTowardCamera;
            Matrix labelOffset = Matrix.CreateTranslation(LabelX, LabelBaseY + LabelStepY * layerIndex, 0);
            Matrix depth = Matrix.CreateTranslation(0, 0, z);
            Matrix finalMatrix = labelOffset * depth * rotationMatrix;

            device.DepthStencilState = depthState;
            device.BlendState = BlendState.NonPremultiplied;

            _basicEffect.World = finalMatrix;
            _basicEffect.Texture = texture;
            _basicEffect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _labelVertices, 0, 2);
        }

        private static int MapLayerIndex(int drawIndex, int totalCount)
        {
            // Map draw order to source layer: furthest uses highest index, nearest uses lowest.
            return (totalCount - 1) - drawIndex;
        }
    }
}

