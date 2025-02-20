﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard
{
    /// <summary>
    /// Manages assets. Preloads all assets in assets folder, storing their 
    /// path relative to the assets folder as keys to the assets. Assets
    /// are retrievable through their relative paths and types. Asset data is
    /// preloaded and should be accessed through the singleton.
    /// </summary>
    class AssetManager2
    {
        // Thread safe singleton implementation
        // from https://csharpindepth.com/articles/singleton
        private AssetManager2()
        {
            // Load all textures in assets path
            loadAssetsFolder();
        }
        public static AssetManager2 Instance { get { return Nested.instance; } }
        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
            internal static readonly AssetManager2 instance = new AssetManager2();
        }
        /// <summary>
        /// Keys are relative paths. Relative to assets folder as configured in envar.cfg.
        /// </summary>
        private static Dictionary<string,Texture> _textures = new Dictionary<string,Texture>();

        private void loadAssetsFolder() { loadTexturesFromFolder(Bootstrap.getEnvironmentalVariable("assetpath")); }


        /// <summary>
        /// Walks down the given directory, adding all compatible files as textures. Errors are logged to debug. 
        /// </summary>
        /// <param name="absoluteFolderPath">Absolute path to the directory from which to load. 
        /// Adds the path relative from the assets folder as key for the texture in the directory.</param>
        private void loadTexturesFromFolder(string absoluteFolderPath) 
        {
            string[] files = Directory.GetFiles(absoluteFolderPath);
            string[] dirs = Directory.GetDirectories(absoluteFolderPath);

            foreach (string d in dirs)
            {
                loadTexturesFromFolder(absoluteFolderPath + Path.PathSeparator + d);
            }

            foreach (string f in files)
            {
                // Add path relative to assets path as key to the texture generated by loading the target file.
                _textures.Add(
                    Path.GetRelativePath(
                        Bootstrap.getEnvironmentalVariable("assetpath")
                        ,
                        absoluteFolderPath + Path.PathSeparator + f)
                    ,
                    new Texture(absoluteFolderPath + Path.PathSeparator + f));
            }
        }
        /// <summary>
        /// Reload all assets from assets folder.
        /// </summary>
        public void ReloadAssets() 
        {
            Discard();
            loadAssetsFolder();
        }
        /// <summary>
        /// Discard all loaded assets.
        /// </summary>
        public void Discard() 
        { 
            _textures.Clear();
        }

        /// <summary>
        /// Getter. Returns Texture associated with the path relative to the 
        /// assets folder as configured in envar.cfg.
        /// </summary>
        /// <param name="relativePath">Path relative to assets folder</param>
        /// <returns>Texture loaded from the given filepath during instantiation.</returns>
        public Texture getTexture(string relativePath) 
        {
            return _textures[relativePath];
        }
    }
}
