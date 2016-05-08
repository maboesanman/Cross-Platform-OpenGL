using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
//using OpenTK.Graphics.ES20;
using OpenTK.Graphics.OpenGL4;

namespace Cross_Platform_OpenGL.Windows
{
    class GameView : GameWindow
    {
        
        Game myGame;
        public GameView()
        {
            myGame = new Game();
            
            Console.WriteLine(GL.GetString(StringName.Version));
            Console.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));
            //Console.WriteLine(GL.GetString(All.ShaderCompiler));
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Title = "Cross_Platform_OpenGL";
            myGame.SetupGL();
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            myGame.Draw(Width, Height);
            SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            myGame.Update(Width/Height);
        }
    }
}
