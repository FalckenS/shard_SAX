using Shard.Shard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameWindowSettings = OpenTK.Windowing.Desktop.GameWindowSettings;
namespace Shard.MyApplication
{
    internal class Application
    {
        public static void Main(string[] args) 
        {
            string[] strs = { "One", "Twwwwwwwo", "Three", "Four" };
            Animation<string> anim = new Animation<string>(strs);

            



            Bootstrap.setup();
            GLWindow w = new GLWindow(800, 800, "waa");
            AssetManager2 am = AssetManager2.Instance;
            
            w.Run();
            
        }
    }
}
