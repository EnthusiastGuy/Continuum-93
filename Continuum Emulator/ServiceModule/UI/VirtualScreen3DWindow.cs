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
        private BasicEffect _borderEffect;
        private VertexPositionTexture[] _vertices;
        private VertexPositionColor[] _borderVertices;
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
        private static readonly DepthStencilState TransparentDepthState = new DepthStencilState
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
        private static readonly Color BorderColor = Color.BlueViolet;
        
        // Skybox fields
        private BasicEffect _skyboxEffect;
        private VertexPositionTexture[] _skyboxVertices;
        private Texture2D _skyboxTexture;
        private const float SkyboxSize = 50f;

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

            // Create a basic effect for borders
            _borderEffect = new BasicEffect(device)
            {
                TextureEnabled = false,
                VertexColorEnabled = true,
                Alpha = 1.0f
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

            // Define border vertices (line loop around the quad)
            _borderVertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(-2.40f, 1.35f, 0), BorderColor),   // Top-left
                new VertexPositionColor(new Vector3(2.40f, 1.35f, 0), BorderColor),    // Top-right
                new VertexPositionColor(new Vector3(2.40f, -1.35f, 0), BorderColor),   // Bottom-right
                new VertexPositionColor(new Vector3(-2.40f, -1.35f, 0), BorderColor),  // Bottom-left
                new VertexPositionColor(new Vector3(-2.40f, 1.35f, 0), BorderColor)    // Back to top-left to close loop
            };

            BuildLabelGeometry();
            GenerateLayerLabelTextures();
            InitializeSkybox(device);

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
            _borderEffect?.Dispose();
            _skyboxEffect?.Dispose();
            _skyboxTexture?.Dispose();
            if (_layerLabelTextures != null)
            {
                foreach (var tex in _layerLabelTextures)
                {
                    tex?.Dispose();
                }
            }
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
                0.01f,  // Near plane
                100f);  // Far plane - increased to properly render skybox at all zoom levels

            // Create rotation matrices
            Matrix rotationX = Matrix.CreateRotationX(_rotationAngles.X);
            Matrix rotationY = Matrix.CreateRotationY(_rotationAngles.Y);
            Matrix rotationZ = Matrix.CreateRotationZ(_rotationAngles.Z);
            Matrix pan = Matrix.CreateTranslation(_translation);

            // Apply the rotation to the world matrix
            Matrix rotationMatrix = pan * rotationX * rotationY * rotationZ;

            // Render skybox first (at infinite distance)
            if (_skyboxEffect != null && _skyboxVertices != null && _skyboxTexture != null)
            {
                RenderSkybox(device, rotationMatrix);
            }

            // Enable alpha blending to support transparency in layers 1-7
            // Use NonPremultiplied blend state for proper alpha blending with transparent textures
            device.BlendState = BlendState.NonPremultiplied;

            int layerCount = Math.Min(Video.PaletteCount, 8);

            // Then draw layers 1-7 back-to-front (transparent, closer to camera)
            // Depth test stays enabled to respect actual 3D ordering; depth writes stay disabled
            device.DepthStencilState = DepthStencilState.None;

            // Draw layers back-to-front (nearest to farthest) for proper transparency
            for (int layerIndex = layerCount - 1; layerIndex >= 0; layerIndex--)
            {
                byte drawIdx = (byte)layerIndex;
                int srcIndex = MapLayerIndex(drawIdx, layerCount);
                if (srcIndex >= 0 && srcIndex < Video.Layers.Count && Video.Layers[srcIndex] != null)
                {
                    // Invert z: layer 7 at z=0 (closest), layer 0 at z=1.75 (farthest)
                    float z = (7 - drawIdx) * 0.25f;
                    Matrix depth = Matrix.CreateTranslation(0, 0, z);
                    Matrix finalMatrix = depth * rotationMatrix;

                    _basicEffect.World = finalMatrix;
                    _basicEffect.Texture = Video.Layers[srcIndex];

                    _basicEffect.CurrentTechnique.Passes[0].Apply();

                    device.RasterizerState = new RasterizerState()
                    {
                        CullMode = CullMode.None,
                        MultiSampleAntiAlias = true,
                    };

                    device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertices, 0, 2);

                    DrawLayerBorder(device, finalMatrix);
                    DrawLayerLabel(device, drawIdx, (byte)(layerCount - srcIndex - 1), z, rotationMatrix, DepthStencilState.None);
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

            // Preserve current state
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
                    Color.BlueViolet,
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

        private void DrawLayerBorder(GraphicsDevice device, Matrix worldMatrix)
        {
            if (_borderEffect == null || _borderVertices == null)
                return;

            // Set up border effect with same view/projection as main effect
            _borderEffect.World = worldMatrix;
            _borderEffect.View = _basicEffect.View;
            _borderEffect.Projection = _basicEffect.Projection;

            // Apply the effect and draw the border as a line strip
            _borderEffect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserPrimitives(PrimitiveType.LineStrip, _borderVertices, 0, _borderVertices.Length - 1);
        }

        private void DrawLayerLabel(GraphicsDevice device, byte placementIndex, byte labelIndex, float layerZ, Matrix rotationMatrix, DepthStencilState depthState)
        {
            if (_layerLabelTextures == null ||
                labelIndex >= _layerLabelTextures.Length ||
                _layerLabelTextures[labelIndex] == null ||
                _labelVertices == null)
                return;

            // Keep label on same plane, tiny nudge toward camera to avoid z-fighting
            float z = layerZ + LabelZOffsetTowardCamera;
            Matrix labelOffset = Matrix.CreateTranslation(LabelX, LabelBaseY + LabelStepY * placementIndex, 0);
            Matrix depth = Matrix.CreateTranslation(0, 0, z);
            Matrix finalMatrix = labelOffset * depth * rotationMatrix;

            device.DepthStencilState = depthState;
            device.BlendState = BlendState.NonPremultiplied;

            _basicEffect.World = finalMatrix;
            _basicEffect.Texture = _layerLabelTextures[labelIndex];
            _basicEffect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _labelVertices, 0, 2);
        }

        private void InitializeSkybox(GraphicsDevice device)
        {
            // Create skybox effect
            _skyboxEffect = new BasicEffect(device)
            {
                TextureEnabled = true,
                VertexColorEnabled = false,
                Alpha = 1.0f
            };

            // Generate starfield texture
            _skyboxTexture = GenerateStarfieldTexture(device, 1024, 1024);

            // Create skybox cube vertices
            // We'll create a large cube around the scene
            float s = SkyboxSize;
            _skyboxVertices = new VertexPositionTexture[]
            {
                // Front face
                new VertexPositionTexture(new Vector3(-s, s, -s), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(s, s, -s), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-s, -s, -s), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(s, -s, -s), new Vector2(1, 1)),
                
                // Back face
                new VertexPositionTexture(new Vector3(s, s, s), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(-s, s, s), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(s, -s, s), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-s, -s, s), new Vector2(1, 1)),
                
                // Left face
                new VertexPositionTexture(new Vector3(-s, s, s), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(-s, s, -s), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-s, -s, s), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-s, -s, -s), new Vector2(1, 1)),
                
                // Right face
                new VertexPositionTexture(new Vector3(s, s, -s), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(s, s, s), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(s, -s, -s), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(s, -s, s), new Vector2(1, 1)),
                
                // Top face
                new VertexPositionTexture(new Vector3(-s, s, s), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(s, s, s), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-s, s, -s), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(s, s, -s), new Vector2(1, 1)),
                
                // Bottom face
                new VertexPositionTexture(new Vector3(-s, -s, -s), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(s, -s, -s), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-s, -s, s), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(s, -s, s), new Vector2(1, 1))
            };
        }

        private Texture2D GenerateStarfieldTexture(GraphicsDevice device, int width, int height)
        {
            Texture2D texture = new Texture2D(device, width, height);
            Color[] data = new Color[width * height];
            Random random = new Random(42); // Fixed seed for consistent starfield

            // Fill with deep space color (very dark blue/black)
            Color spaceColor = new Color(5, 8, 15);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = spaceColor;
            }

            // Add stars of various sizes and brightness
            int numStars = width * height / 100; // Density of stars
            
            for (int i = 0; i < numStars; i++)
            {
                int x = random.Next(width);
                int y = random.Next(height);
                int idx = y * width + x;

                // Random star brightness and color
                float brightness = (float)random.NextDouble();
                
                if (brightness > 0.98f)
                {
                    // Very bright stars (rare)
                    Color starColor = GetStarColor(random, 240 + random.Next(15));
                    data[idx] = starColor;
                    
                    // Add glow around bright stars
                    if (x > 0) data[idx - 1] = Color.Lerp(data[idx - 1], starColor, 0.3f);
                    if (x < width - 1) data[idx + 1] = Color.Lerp(data[idx + 1], starColor, 0.3f);
                    if (y > 0) data[idx - width] = Color.Lerp(data[idx - width], starColor, 0.3f);
                    if (y < height - 1) data[idx + width] = Color.Lerp(data[idx + width], starColor, 0.3f);
                }
                else if (brightness > 0.95f)
                {
                    // Bright stars
                    data[idx] = GetStarColor(random, 200 + random.Next(40));
                }
                else if (brightness > 0.85f)
                {
                    // Medium stars
                    data[idx] = GetStarColor(random, 150 + random.Next(50));
                }
                else if (brightness > 0.7f)
                {
                    // Dim stars
                    data[idx] = GetStarColor(random, 100 + random.Next(50));
                }
                else
                {
                    // Faint stars
                    data[idx] = GetStarColor(random, 60 + random.Next(40));
                }
            }

            // Add some nebula-like color variations (very subtle)
            for (int i = 0; i < 50; i++)
            {
                int centerX = random.Next(width);
                int centerY = random.Next(height);
                int radius = 20 + random.Next(60);
                
                Color nebulaColor = random.Next(3) switch
                {
                    0 => new Color(20, 15, 30, 30), // Purple
                    1 => new Color(15, 20, 35, 30), // Blue
                    _ => new Color(30, 15, 20, 30)  // Red
                };

                for (int dy = -radius; dy <= radius; dy++)
                {
                    for (int dx = -radius; dx <= radius; dx++)
                    {
                        int px = centerX + dx;
                        int py = centerY + dy;
                        
                        if (px >= 0 && px < width && py >= 0 && py < height)
                        {
                            float distance = MathF.Sqrt(dx * dx + dy * dy);
                            if (distance <= radius)
                            {
                                float falloff = 1.0f - (distance / radius);
                                falloff = falloff * falloff; // Smooth falloff
                                
                                int idx = py * width + px;
                                data[idx] = Color.Lerp(data[idx], nebulaColor, falloff * 0.15f);
                            }
                        }
                    }
                }
            }

            texture.SetData(data);
            return texture;
        }

        private Color GetStarColor(Random random, int baseBrightness)
        {
            // Stars have different colors based on temperature
            float temp = (float)random.NextDouble();
            
            if (temp > 0.95f)
            {
                // Blue-white hot stars
                return new Color(baseBrightness, baseBrightness, 255);
            }
            else if (temp > 0.8f)
            {
                // White stars
                return new Color(baseBrightness, baseBrightness, baseBrightness);
            }
            else if (temp > 0.5f)
            {
                // Yellow-white stars
                return new Color(baseBrightness, baseBrightness, (int)(baseBrightness * 0.8f));
            }
            else if (temp > 0.2f)
            {
                // Orange stars
                return new Color(baseBrightness, (int)(baseBrightness * 0.8f), (int)(baseBrightness * 0.5f));
            }
            else
            {
                // Red stars
                return new Color(baseBrightness, (int)(baseBrightness * 0.6f), (int)(baseBrightness * 0.4f));
            }
        }

        private void RenderSkybox(GraphicsDevice device, Matrix rotationMatrix)
        {
            // Set up skybox effect
            // Skybox rotates slightly with the scene but doesn't translate
            // We only want to apply rotation, not translation
            Matrix skyboxRotation = Matrix.CreateRotationX(_rotationAngles.X * 0.2f) * 
                                    Matrix.CreateRotationY(_rotationAngles.Y * 0.2f);
            
            _skyboxEffect.World = skyboxRotation;
            _skyboxEffect.View = _basicEffect.View;
            _skyboxEffect.Projection = _basicEffect.Projection;
            _skyboxEffect.Texture = _skyboxTexture;

            // Disable depth writes but enable depth test
            // This ensures skybox is always behind everything
            device.DepthStencilState = new DepthStencilState
            {
                DepthBufferEnable = true,
                DepthBufferWriteEnable = false,
                DepthBufferFunction = CompareFunction.LessEqual
            };
            
            device.BlendState = BlendState.Opaque;
            device.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None
            };

            // Draw each face of the skybox cube
            for (int face = 0; face < 6; face++)
            {
                _skyboxEffect.CurrentTechnique.Passes[0].Apply();
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _skyboxVertices, face * 4, 2);
            }
        }

        private static int MapLayerIndex(int drawIndex, int totalCount)
        {
            // Map draw order to source layer: furthest uses highest index, nearest uses lowest.
            return drawIndex;
        }
    }
}

