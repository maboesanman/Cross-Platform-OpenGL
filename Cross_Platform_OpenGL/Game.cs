using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using OpenTK.Graphics.ES20;
using OpenTK;

#if __ANDROID__
using OpenTK.Platform;
using Android.Opengl;
#endif

namespace Cross_Platform_OpenGL
{
    class Game
    {
        #region openGL variables
        
        enum Uniform
        {
            ModelViewProjection_Matrix,
            Normal_Matrix,
            Count
        }

        enum Attribute
        {
            Vertex,
            Normal,
            Count
        }

        System.Diagnostics.Stopwatch stopwatch;

        int[] uniforms = new int[(int)Uniform.Count];
        #region vert data
        float[] cubeVertexData = {
			// Data layout for each line below is:
			// positionX, positionY, positionZ,     normalX, normalY, normalZ,
			0.5f, -0.5f, -0.5f,        1.0f, 0.0f, 0.0f,
            0.5f, 0.5f, -0.5f,         1.0f, 0.0f, 0.0f,
            0.5f, -0.5f, 0.5f,         1.0f, 0.0f, 0.0f,
            0.5f, -0.5f, 0.5f,         1.0f, 0.0f, 0.0f,
            0.5f, 0.5f, -0.5f,          1.0f, 0.0f, 0.0f,
            0.5f, 0.5f, 0.5f,         1.0f, 0.0f, 0.0f,

            0.5f, 0.5f, -0.5f,         0.0f, 1.0f, 0.0f,
            -0.5f, 0.5f, -0.5f,        0.0f, 1.0f, 0.0f,
            0.5f, 0.5f, 0.5f,          0.0f, 1.0f, 0.0f,
            0.5f, 0.5f, 0.5f,          0.0f, 1.0f, 0.0f,
            -0.5f, 0.5f, -0.5f,        0.0f, 1.0f, 0.0f,
            -0.5f, 0.5f, 0.5f,         0.0f, 1.0f, 0.0f,

            -0.5f, 0.5f, -0.5f,        -1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,       -1.0f, 0.0f, 0.0f,
            -0.5f, 0.5f, 0.5f,         -1.0f, 0.0f, 0.0f,
            -0.5f, 0.5f, 0.5f,         -1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,       -1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, 0.5f,        -1.0f, 0.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,       0.0f, -1.0f, 0.0f,
            0.5f, -0.5f, -0.5f,        0.0f, -1.0f, 0.0f,
            -0.5f, -0.5f, 0.5f,        0.0f, -1.0f, 0.0f,
            -0.5f, -0.5f, 0.5f,        0.0f, -1.0f, 0.0f,
            0.5f, -0.5f, -0.5f,        0.0f, -1.0f, 0.0f,
            0.5f, -0.5f, 0.5f,         0.0f, -1.0f, 0.0f,

            0.5f, 0.5f, 0.5f,          0.0f, 0.0f, 1.0f,
            -0.5f, 0.5f, 0.5f,         0.0f, 0.0f, 1.0f,
            0.5f, -0.5f, 0.5f,         0.0f, 0.0f, 1.0f,
            0.5f, -0.5f, 0.5f,         0.0f, 0.0f, 1.0f,
            -0.5f, 0.5f, 0.5f,         0.0f, 0.0f, 1.0f,
            -0.5f, -0.5f, 0.5f,        0.0f, 0.0f, 1.0f,

            0.5f, -0.5f, -0.5f,        0.0f, 0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,       0.0f, 0.0f, -1.0f,
            0.5f, 0.5f, -0.5f,         0.0f, 0.0f, -1.0f,
            0.5f, 0.5f, -0.5f,         0.0f, 0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,       0.0f, 0.0f, -1.0f,
            -0.5f, 0.5f, -0.5f,        0.0f, 0.0f, -1.0f
        };
        #endregion
        int program;
        Matrix4 modelViewProjectionMatrix;
        Matrix3 normalMatrix;
        float rotation;

        uint vertexArray;
        uint vertexBuffer;


        #endregion

        public Game()
        {
            Setup();
        }
        public void Setup()
        {
            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
        }
        public void SetupGL()
        {
            LoadShaders(out program);
            GL.Enable(EnableCap.DepthTest);

            GL.Oes.GenVertexArrays(1, out vertexArray);
            GL.Oes.BindVertexArray(vertexArray);
            //GLES20.GlBindBuffer(GLES20.GlArrayBuffer, vertexBuffer);
            GL.GenBuffers(1, out vertexBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(cubeVertexData.Length * sizeof(float)), cubeVertexData, BufferUsage.StaticDraw);

            GL.EnableVertexAttribArray(0);//this may break on android
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 24, new IntPtr(0));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 24, new IntPtr(12));
            
            GL.Oes.BindVertexArray(0);
        }
        public void Update(float aspect)
        {
            var projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(65.0f), aspect, 0.1f, 100.0f);
            
            var baseModelViewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, -4.0f);
            baseModelViewMatrix = Matrix4.CreateFromAxisAngle(new Vector3(0.0f, 1.0f, 0.0f), rotation) * baseModelViewMatrix;
            
            // Compute the model view matrix for the object rendered with ES2
            var modelViewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, 1.5f);
            modelViewMatrix = Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 1.0f, 1.0f), rotation) * modelViewMatrix;
            modelViewMatrix = modelViewMatrix * baseModelViewMatrix;

            normalMatrix = new Matrix3(Matrix4.Transpose(Matrix4.Invert(modelViewMatrix)));

            modelViewProjectionMatrix = modelViewMatrix * projectionMatrix;
            rotation = (float)stopwatch.Elapsed.TotalSeconds;
            //rotation += (float)TimeSinceLastUpdate * 0.5f;
        }
        public void Draw(float width, float height)
        {

            GL.ClearColor(0.65f, 0.65f, 0.65f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Oes.BindVertexArray(vertexArray);

            // Render the object
            GL.UseProgram(program);

            GL.UniformMatrix4(uniforms[(int)Uniform.ModelViewProjection_Matrix], false, ref modelViewProjectionMatrix);
            GL.UniformMatrix3(uniforms[(int)Uniform.Normal_Matrix], false, ref normalMatrix);

            GL.DrawArrays(BeginMode.Triangles, 0, 36);
        }
        public void TearDown()
        {

        }
        public void TearDownGL()
        {
            GL.DeleteBuffers(1, ref vertexBuffer);
            
            GL.Oes.DeleteVertexArrays(1, ref vertexArray);
            
            if (program > 0)
            {
                GL.DeleteProgram(program);
                program = 0;
            }
        }
        public bool LoadShaders(out int program)
        {
            int vertShader, fragShader;

            // Create shader program.
            program = GL.CreateProgram();

            // Create and compile vertex shader.
            if (!CompileShader(ShaderType.VertexShader, Shaders.GetVertexShader(), out vertShader))
            {
                Console.WriteLine("Failed to compile vertex shader");
                return false;
            }
            // Create and compile fragment shader.
            if (!CompileShader(ShaderType.FragmentShader, Shaders.GetFragmentShader(), out fragShader))
            {
                Console.WriteLine("Failed to compile fragment shader");
                return false;
            }

            // Attach vertex shader to program.
            GL.AttachShader(program, vertShader);

            // Attach fragment shader to program.
            GL.AttachShader(program, fragShader);

            // Bind attribute locations.
            // This needs to be done prior to linking.
            GL.BindAttribLocation(program, 0, "position");
            GL.BindAttribLocation(program, 1, "normal");
              
            // Link program.
            if (!LinkProgram(program))
            {
                Console.WriteLine("Failed to link program: {0:x}", program);

                if (vertShader != 0)
                    GL.DeleteShader(vertShader);

                if (fragShader != 0)
                    GL.DeleteShader(fragShader);

                if (program != 0)
                {
                    GL.DeleteProgram(program);
                    program = 0;
                }
                return false;
            }
            // Get uniform locations.
            uniforms[(int)Uniform.ModelViewProjection_Matrix] = GL.GetUniformLocation(program, "modelViewProjectionMatrix");
            uniforms[(int)Uniform.Normal_Matrix] = GL.GetUniformLocation(program, "normalMatrix");

            // Release vertex and fragment shaders.
            if (vertShader != 0)
            {
                GL.DetachShader(program, vertShader);
                GL.DeleteShader(vertShader);
            }

            if (fragShader != 0)
            {
                GL.DetachShader(program, fragShader);
                GL.DeleteShader(fragShader);
            }

            return true;
        }
        bool CompileShader(ShaderType type, string src, out int shader)
        {
            shader = GL.CreateShader(type);
            GL.ShaderSource(shader, src);
            GL.CompileShader(shader);

#if DEBUG
            int logLength = 0;
            GL.GetShader(shader, ShaderParameter.InfoLogLength, out logLength);
            if (logLength > 0)
            {
                Console.WriteLine("Shader compile log:\n{0}", GL.GetShaderInfoLog(shader));
            }
#endif

            int status = 0;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                GL.DeleteShader(shader);
                return false;
            }

            return true;
        }

        bool LinkProgram(int prog)
        {
            GL.LinkProgram(prog);

#if DEBUG
            int logLength = 0;
            GL.GetProgram(prog, ProgramParameter.InfoLogLength, out logLength);
            if (logLength > 0)
                Console.WriteLine("Program link log:\n{0}", GL.GetProgramInfoLog(prog));
#endif
            int status = 0;
            GL.GetProgram(prog, ProgramParameter.LinkStatus, out status);
            return status != 0;
        }

        bool ValidateProgram(int prog)
        {
            int logLength, status = 0;

            GL.ValidateProgram(prog);
            GL.GetProgram(prog, ProgramParameter.InfoLogLength, out logLength);
            if (logLength > 0)
            {
                var log = new System.Text.StringBuilder(logLength);
                GL.GetProgramInfoLog(prog, logLength, out logLength, log);
                Console.WriteLine("Program validate log:\n{0}", log);
            }

            GL.GetProgram(prog, ProgramParameter.LinkStatus, out status);
            return status != 0;
        }
    }
}
