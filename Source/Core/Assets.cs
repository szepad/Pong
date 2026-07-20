namespace Pong.Core
{
    public static class Assets
    {
        public static readonly Dictionary<string, Texture2D> Tex = [];
        public static readonly Dictionary<string, Sound> Sounds = [];
        public static readonly Dictionary<string, Font> Fonts = [];

        private static readonly string[] TextureFormats = [".png", ".jpg", ".jpeg", ".bmp", ".gif"];
        private static readonly string[] SoundFormats = [".wav", ".ogg", ".flac"];
        private static readonly string[] FontFormats = [".ttf", ".otf"];
        private static readonly string[] IgnoreFormats = [".ico"];

        public static void Load(string root)
        {
            if (!Directory.Exists(root))
            {
                Raylib.TraceLog(TraceLogLevel.Warning, $"'{Path.GetFullPath(root)}' folder doesn't exist");
                return;
            }

            var files = SearchDir(root);
            foreach (string file in files)
            {
                string ext = Path.GetExtension(file);
                string name = Path.GetFileNameWithoutExtension(file);

                if (TextureFormats.Contains(ext))
                {
                    Tex[name] = Raylib.LoadTexture(file);
                    Raylib.TraceLog(TraceLogLevel.Info, $"ASSETS: Loaded texture '{file}'");
                }
                else if (SoundFormats.Contains(ext))
                {
                    Sounds[name] = Raylib.LoadSound(file);
                    Raylib.TraceLog(TraceLogLevel.Info, $"ASSETS: Loaded sound '{file}'");
                }
                else if (FontFormats.Contains(ext))
                {
                    Fonts[name] = Raylib.LoadFont(file);
                    Raylib.TraceLog(TraceLogLevel.Info, $"ASSETS: Loaded font '{file}'");
                }
                else if (!IgnoreFormats.Contains(ext))
                {
                    Raylib.TraceLog(TraceLogLevel.Warning, $"ASSETS: Unsupported file format '{file}'");
                }
            }
        }

        private static List<string> SearchDir(string root)
        {
            List<string> files = [];
            files.AddRange(Directory.GetFiles(root));

            foreach (string dir in Directory.GetDirectories(root))
            {
                files.AddRange(SearchDir(dir));
            }

            return files;
        }
    }
}
