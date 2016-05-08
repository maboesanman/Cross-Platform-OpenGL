using System;
using System.Diagnostics;

using Foundation;
using GLKit;
using OpenGLES;
using OpenTK;
using OpenTK.Graphics.ES20;

namespace Cross_Platform_OpenGL.iOS
{
    [Register("GameViewController")]
    public class GameViewController : GLKViewController, IGLKViewDelegate
    {
        

        Game myGame = new Game();

        EAGLContext context { get; set; }

        //GLKBaseEffect effect { get; set; }

        [Export("initWithCoder:")]
        public GameViewController(NSCoder coder) : base(coder)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            
            context = new EAGLContext(EAGLRenderingAPI.OpenGLES2);

            if (context == null)
            {
                Debug.WriteLine("Failed to create ES context");
            }

            var view = (GLKView)View;
            view.Context = context;
            view.DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24;

            SetupGL();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            TearDownGL();

            if (EAGLContext.CurrentContext == context)
                EAGLContext.SetCurrentContext(null);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            if (IsViewLoaded && View.Window == null)
            {
                View = null;

                TearDownGL();

                if (EAGLContext.CurrentContext == context)
                {
                    EAGLContext.SetCurrentContext(null);
                }
            }

            // Dispose of any resources that can be recreated.
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        void SetupGL()
        {
            EAGLContext.SetCurrentContext(context);
            myGame.SetupGL();
            
        }

        void TearDownGL()
        {
            EAGLContext.SetCurrentContext(context);
            myGame.TearDownGL();
            
        }

        #region GLKView and GLKViewController delegate methods

        public override void Update()
        {
            myGame.Update((float)View.Bounds.Size.Width/(float)View.Bounds.Size.Height);
        }

        void IGLKViewDelegate.DrawInRect(GLKView view, CoreGraphics.CGRect rect)
        {
            myGame.Draw((float)View.Bounds.Size.Width, (float)View.Bounds.Size.Height);
        }
        
        #endregion

        
    }
}