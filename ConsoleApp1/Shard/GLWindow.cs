using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.IO;
using StbImageSharp;
using System.Reflection.Metadata;
using OpenTK.Mathematics;
using OpenTK.Compute.OpenCL;
using SharpFont;
using System.Xml.Linq;
using Shard.Shard;

namespace Shard
{
    internal class GLWindow : GameWindow
    {
        private ShardInput _input;

        private float[] _verts = default;
        private float[] _indcs = default;

        

        //TEMP
        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        float[] textureCoordinates =
        {
            0.0f, 0.0f, //ll
            1.0f, 0.0f, //lr
            0.0f, 1.0f, //ul
            1.0f, 1.0f, //ur
        };

        Shader _texture2dShader;
        int _width;
        int _height;
        int _vertexBufferObject;
        int _vertexArrayObject;
        int _elementBufferObject;
        int _texture;
        ImageResult _image;
        Sprite _spr;
        Sprite _spr2;
        Sprite _spr3;
        Sprite _spr4;
        Animation<Sprite> sAnim;
        Animation<TextureRegion> texRegAnim;
        TextureRegion _textureRegion;
        Animation<Sprite> sheetSpritesAnim;
        int program;

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.UseProgram(program);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            string sep = Char.ToString(System.IO.Path.DirectorySeparatorChar);
            string shadersPath = Path.GetFullPath(Path.Combine(Bootstrap.getEnvironmentalVariable("assetpath"), @"..\ConsoleApp1\Shard\Shaders\"));
            string vertShaderPath = shadersPath + sep + "texture2d.vert";
            string fragShaderPath = shadersPath + sep + "texture2d.frag";
            _texture2dShader = new Shader(vertShaderPath, fragShaderPath);
            _texture2dShader.Use();


            int[] ints = { 400,500,600,700,750};
            Animation<int> xanim = new Animation<int>(ints);
            xanim.MilliSecondsBetweenKeyFrames = 1000;
            xanim.Play();
            _spr = new Sprite(AssetManager2.getTexture("background.jpg"), 0, 0,800,800);
            _spr2 = new Sprite(AssetManager2.getTexture("invader1.png"),400,400);
            Sprite[] sprites = {
            new Sprite(AssetManager2.getTexture("invader1.png"),400,400),
            new Sprite(AssetManager2.getTexture("invader2.png"),500,400),
            new Sprite(AssetManager2.getTexture("invader1.png"),600,400),
            new Sprite(AssetManager2.getTexture("invader2.png"),700,400),
            new Sprite(AssetManager2.getTexture("invader1.png"),750,400)
            };
            sAnim =new Animation<Sprite>(sprites);
            sAnim.MilliSecondsBetweenKeyFrames =1000;
            sAnim.Play();

            TextureSheet sheet = new TextureSheet(AssetManager2.getTexture("spritesheet.png"),6,6);
            List<Sprite> sheetsprites = new List<Sprite>();
            foreach (TextureRegion regin in sheet.TextureRegionsList)
            {
                sheetsprites.Add(new Sprite(regin,300,300,100,100));
            }

            texRegAnim = new Animation<TextureRegion>(sheet.TextureRegionsList);
            texRegAnim.MilliSecondsBetweenKeyFrames=100;
            texRegAnim.Play();

            sheetSpritesAnim = new Animation<Sprite>(sheetsprites);
            sheetSpritesAnim.MilliSecondsBetweenKeyFrames=100;
            sheetSpritesAnim.Play();
            _textureRegion = texRegAnim.GetKeyFrame(Bootstrap.getCurrentMillis(),PlayMode.FORWARD_LOOP);
            _spr4 = new Sprite(_textureRegion,200,200,100,100);
            


        }

        protected override void OnUnload()
        {
            base.OnUnload();
            
        }

        //VBO


        //TEMP

        public GLWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        public GLWindow(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() 
        { ClientSize = (width, height), Title = title }) { _width = width; _height = height; }


        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.BindVertexArray(_vertexArrayObject);

            //_spr.Use();
            _spr3 = sheetSpritesAnim.GetKeyFrame(Bootstrap.getCurrentMillis(), PlayMode.FORWARD_LOOP);
            //_shader.Use();
            renderSprite(_spr);
            _spr2 = sAnim.GetKeyFrame(Bootstrap.getCurrentMillis(),PlayMode.FORWARD_LOOP);
            
            _spr4.SetTextureRegion(texRegAnim.GetKeyFrame(Bootstrap.getCurrentMillis(), PlayMode.FORWARD_LOOP));
            renderSprite(_spr2);
            renderSprite(_spr3);
            renderSprite(_spr4);
            //GL.DrawElements(PrimitiveType.Triangles,_indices.Length,DrawElementsType.UnsignedInt,0);
            SwapBuffers();
        }

        private void renderSprite(Sprite sprite) 
        {
            float[] vertices = {
                sprite.X                , sprite.Y              ,0, sprite.RegionX                      ,sprite.RegionY,                    // Bottom left
                sprite.X+sprite.Width   , sprite.Y              ,0, sprite.RegionX+sprite.RegionWidth   ,sprite.RegionY,                    // Bottom right
                sprite.X+sprite.Width   , sprite.Y+sprite.Height,0, sprite.RegionX+sprite.RegionWidth   ,sprite.RegionY+sprite.RegionHeight,// Top right
                sprite.X                , sprite.Y+sprite.Height,0, sprite.RegionX                      ,sprite.RegionY+sprite.RegionHeight // Top left
            };
            
            uint[] indices = {
                0,1,2,
                0,3,2
            };

            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            var vertexLocation = _texture2dShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _texture2dShader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            var transform = Matrix4.Identity;
            transform = transform * Matrix4.CreateTranslation(-_width/2, -_height/2, 0);
            transform = transform * Matrix4.CreateScale(1f/(_width/2), 1f/(_height/2), 1f);

            var textureTransform = Matrix2.Identity;
            textureTransform = textureTransform * Matrix2.CreateScale(1f/sprite.TextureWidth,1f/sprite.TextureHeight);

            sprite.Use();
            _texture2dShader.Use();
            _texture2dShader.SetMatrix4("transform", transform);
            _texture2dShader.SetMatrix2("textureTransform", textureTransform);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);
            GL.Viewport(0,0,e.Width,e.Height);
            _width = e.Width;
            _height = e.Height;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            // Pass key info to event
            base.OnKeyDown(e);
            System.Console.WriteLine(e.Key);
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            System.Console.WriteLine("MOUSEDOWN");
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
        }
    }
}
