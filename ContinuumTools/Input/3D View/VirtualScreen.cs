using ContinuumTools.Network;
using ContinuumTools.States;
using Last_Known_Reality.Reality;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ContinuumTools.Input._3D_View
{
    public static class VirtualScreen
    {
        static BasicEffect basicEffect;
        static VertexPositionTexture[] vertices;
        static Vector3 rotationAngles;
        static Vector3 translation;
        static readonly float rotationSpeed = 0.01f;
        static readonly float zoomSpeed = 0.1f;
        static Vector3 cameraPosition = new(0, 0, 6);
        static Vector3 lastMousePosition;
        static bool autoRotate = true;
        static double localTimeMS = 0;

        public static void Initialize()
        {
            // Pre-populate video layers
            Video.InitializeLayers();

            // Create a basic effect
            basicEffect = new(Renderer.GetGraphicsDevice())
            {
                TextureEnabled = true
            };

            // Define the vertices of the 3D quad and their UV coordinates
            vertices = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(-2.40f, 1.35f, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(2.40f, 1.35f, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-2.40f, -1.35f, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(2.40f, -1.35f, 0), new Vector2(1, 1))
            };
        }

        public static void Update()
        {
            // Auto fetch
            localTimeMS += GameTimePlus.GetElapsedRelativeTimeMs(0);
            if (localTimeMS > 500)
            {
                localTimeMS = 0;
                Protocol.GetVideo();
            }

            if (autoRotate)
            {
                float amplitude = 0.7f;     // The amplitude of rocking
                float frequency = 0.0001f;  // The speed of rocking
                float time = (float)GameTimePlus.GetTotalRelativeTimeMs(0);

                float rockingAngle = amplitude * MathF.Sin(frequency * time);

                rotationAngles.Y = rockingAngle;
            }
        }

        public static void UpdateInput()
        {
            if (InputMouse.GetCurrentMouseX() < 8 || InputMouse.GetCurrentMouseX() > 8 + 650 ||
                InputMouse.GetCurrentMouseY() < 29 || InputMouse.GetCurrentMouseY() > 29 + 365)
                return;

            var mouseState = Mouse.GetState();
            var scrollWheelValue = mouseState.ScrollWheelValue;

            if (scrollWheelValue > lastMousePosition.Z)
            {
                cameraPosition.Z -= zoomSpeed;
            }
            else if (scrollWheelValue < lastMousePosition.Z)
            {
                cameraPosition.Z += zoomSpeed;
            }

            Vector3 currentMousePosition = new(mouseState.X, mouseState.Y, scrollWheelValue);
            Vector3 mouseDelta;

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                autoRotate = false;
                // Calculate the change in mouse position
                currentMousePosition = new(mouseState.X, mouseState.Y, scrollWheelValue);
                mouseDelta = currentMousePosition - lastMousePosition;

                // Update rotation angles based on mouse movement
                rotationAngles.Y += mouseDelta.X * rotationSpeed; // Y-axis rotation
                rotationAngles.X += mouseDelta.Y * rotationSpeed; // X-axis rotation
            }
            else if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                mouseDelta = currentMousePosition - lastMousePosition;
                translation.X += mouseDelta.X * 0.01f;
                translation.Y -= mouseDelta.Y * 0.01f;
            }

            lastMousePosition = currentMousePosition;
        }

        public static void Draw()
        {
            // Set up the basic effect
            basicEffect.View = Matrix.CreateLookAt(cameraPosition, new Vector3(), Vector3.Up);
            basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, Renderer.GetGraphicsDevice().Viewport.AspectRatio, 0.01f, 20);

            // Create a rotation matrix for each axis
            Matrix rotationX = Matrix.CreateRotationX(rotationAngles.X);
            Matrix rotationY = Matrix.CreateRotationY(rotationAngles.Y);
            Matrix rotationZ = Matrix.CreateRotationZ(rotationAngles.Z);
            Matrix pan = Matrix.CreateTranslation(translation);

            // Apply the rotation to the world matrix
            Matrix rotationMatrix = pan * rotationX * rotationY * rotationZ;

            for (byte i = 0; i < 8; i++)
            {
                if (i < Video.PaletteCount)
                {
                    float z = i * 0.25f;
                    Matrix depth = Matrix.CreateTranslation(0, 0, z);
                    Matrix finalMatrix = depth * rotationMatrix;

                    basicEffect.World = finalMatrix;
                    basicEffect.Texture = Video.Layers[i];

                    // Start rendering with the basic effect
                    basicEffect.CurrentTechnique.Passes[0].Apply();

                    Renderer.GetGraphicsDevice().RasterizerState = new RasterizerState()
                    {
                        CullMode = CullMode.None,
                        MultiSampleAntiAlias = true,
                    };
                } else
                {
                    basicEffect.Texture = null;
                }
                

                // Draw the quad
                Renderer.GetGraphicsDevice().DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
            }
        }
    }
}
