using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using OpenTK.Graphics.ES20;
using OpenTK;

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

            OpenTK.Graphics.ES20.GL.Enable(EnableCap.DepthTest);

            OpenTK.Graphics.ES20.GL.Oes.GenVertexArrays(1, out vertexArray);
            OpenTK.Graphics.ES20.GL.Oes.BindVertexArray(vertexArray);

            OpenTK.Graphics.ES20.GL.GenBuffers(1, out vertexBuffer);
            OpenTK.Graphics.ES20.GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            OpenTK.Graphics.ES20.GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(cubeVertexData.Length * sizeof(float)), cubeVertexData, BufferUsage.StaticDraw);

            OpenTK.Graphics.ES20.GL.EnableVertexAttribArray(0);//this may break on android
            OpenTK.Graphics.ES20.GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 24, new IntPtr(0));
            OpenTK.Graphics.ES20.GL.EnableVertexAttribArray(1);
            OpenTK.Graphics.ES20.GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 24, new IntPtr(12));

            OpenTK.Graphics.ES20.GL.Oes.BindVertexArray(0);
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

            OpenTK.Graphics.ES20.GL.ClearColor(0.65f, 0.65f, 0.65f, 1.0f);
            OpenTK.Graphics.ES20.GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            OpenTK.Graphics.ES20.GL.Oes.BindVertexArray(vertexArray);

            // Render the object
            OpenTK.Graphics.ES20.GL.UseProgram(program);

            OpenTK.Graphics.ES20.GL.UniformMatrix4(uniforms[(int)Uniform.ModelViewProjection_Matrix], false, ref modelViewProjectionMatrix);
            OpenTK.Graphics.ES20.GL.UniformMatrix3(uniforms[(int)Uniform.Normal_Matrix], false, ref normalMatrix);

            OpenTK.Graphics.ES20.GL.DrawArrays(BeginMode.Triangles, 0, 36);
        }
        public void TearDown()
        {

        }
        public void TearDownGL()
        {
            OpenTK.Graphics.ES20.GL.DeleteBuffers(1, ref vertexBuffer);
            OpenTK.Graphics.ES20.GL.Oes.DeleteVertexArrays(1, ref vertexArray);
            
            if (program > 0)
            {
                OpenTK.Graphics.ES20.GL.DeleteProgram(program);
                program = 0;
            }
        }
        public bool LoadShaders(out int program)
        {
            int vertShader, fragShader;

            // Create shader program.
            program = OpenTK.Graphics.ES20.GL.CreateProgram();

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
            OpenTK.Graphics.ES20.GL.AttachShader(program, vertShader);

            // Attach fragment shader to program.
            OpenTK.Graphics.ES20.GL.AttachShader(program, fragShader);

            // Bind attribute locations.
            // This needs to be done prior to linking.
            OpenTK.Graphics.ES20.GL.BindAttribLocation(program, 0, "position");
            OpenTK.Graphics.ES20.GL.BindAttribLocation(program, 1, "normal");
              
            // Link program.
            if (!LinkProgram(program))
            {
                Console.WriteLine("Failed to link program: {0:x}", program);

                if (vertShader != 0)
                    OpenTK.Graphics.ES20.GL.DeleteShader(vertShader);

                if (fragShader != 0)
                    OpenTK.Graphics.ES20.GL.DeleteShader(fragShader);

                if (program != 0)
                {
                    OpenTK.Graphics.ES20.GL.DeleteProgram(program);
                    program = 0;
                }
                return false;
            }
            // Get uniform locations.
            uniforms[(int)Uniform.ModelViewProjection_Matrix] = OpenTK.Graphics.ES20.GL.GetUniformLocation(program, "modelViewProjectionMatrix");
            uniforms[(int)Uniform.Normal_Matrix] = OpenTK.Graphics.ES20.GL.GetUniformLocation(program, "normalMatrix");

            // Release vertex and fragment shaders.
            if (vertShader != 0)
            {
                OpenTK.Graphics.ES20.GL.DetachShader(program, vertShader);
                OpenTK.Graphics.ES20.GL.DeleteShader(vertShader);
            }

            if (fragShader != 0)
            {
                OpenTK.Graphics.ES20.GL.DetachShader(program, fragShader);
                OpenTK.Graphics.ES20.GL.DeleteShader(fragShader);
            }

            return true;
        }
        bool CompileShader(ShaderType type, string src, out int shader)
        {
            shader = OpenTK.Graphics.ES20.GL.CreateShader(type);
            OpenTK.Graphics.ES20.GL.ShaderSource(shader, src);
            OpenTK.Graphics.ES20.GL.CompileShader(shader);

#if DEBUG
            int logLength = 0;
            OpenTK.Graphics.ES20.GL.GetShader(shader, ShaderParameter.InfoLogLength, out logLength);
            if (logLength > 0)
            {
                Console.WriteLine("Shader compile log:\n{0}", OpenTK.Graphics.ES20.GL.GetShaderInfoLog(shader));
            }
#endif

            int status = 0;
            OpenTK.Graphics.ES20.GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                OpenTK.Graphics.ES20.GL.DeleteShader(shader);
                return false;
            }

            return true;
        }

        bool LinkProgram(int prog)
        {
            OpenTK.Graphics.ES20.GL.LinkProgram(prog);

#if DEBUG
            int logLength = 0;
            OpenTK.Graphics.ES20.GL.GetProgram(prog, ProgramParameter.InfoLogLength, out logLength);
            if (logLength > 0)
                Console.WriteLine("Program link log:\n{0}", OpenTK.Graphics.ES20.GL.GetProgramInfoLog(prog));
#endif
            int status = 0;
            OpenTK.Graphics.ES20.GL.GetProgram(prog, ProgramParameter.LinkStatus, out status);
            return status != 0;
        }

        bool ValidateProgram(int prog)
        {
            int logLength, status = 0;

            OpenTK.Graphics.ES20.GL.ValidateProgram(prog);
            OpenTK.Graphics.ES20.GL.GetProgram(prog, ProgramParameter.InfoLogLength, out logLength);
            if (logLength > 0)
            {
                var log = new System.Text.StringBuilder(logLength);
                OpenTK.Graphics.ES20.GL.GetProgramInfoLog(prog, logLength, out logLength, log);
                Console.WriteLine("Program validate log:\n{0}", log);
            }

            OpenTK.Graphics.ES20.GL.GetProgram(prog, ProgramParameter.LinkStatus, out status);
            return status != 0;
        }
    }
}
