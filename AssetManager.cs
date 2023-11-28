using Raylib_cs;
using System.Runtime.InteropServices;
using RayLibECS.Events;

namespace RayLibECS;

public class AssetManager
{
    public ulong AssetCacheLimit;
    public ulong VramLimit;
    
    private EventBus _eventBus = EventBus.Instance;

    private bool _dirty;

    private const string Texturepath = "./Assets/Textures/";
    private const string Audiopath = "./Assets/Audio/";
    private const string Fontpath = "./Assets/Fonts/";

    private string[] _fontFiles;
    private string[] _texture2DFiles;
    private string[] _soundFiles;
    
    private Dictionary<string,Font> _loadedFonts;
    private Dictionary<string,Texture2D> _loadedTextures;
    private Dictionary<string,Sound> _loadedSounds;

    private Dictionary<string, ulong> _assetSizeDict;

    private Dictionary<string,int> _assetFrequency;

    internal AssetManager(ulong cacheLimit, ulong vramLimit){
        _dirty = true;
        AssetCacheLimit = cacheLimit;
        VramLimit = vramLimit;

        _assetSizeDict = new Dictionary<string,ulong>();
        
        _assetFrequency = new Dictionary<string, int>();
        _fontFiles = Directory.EnumerateFiles("./Assets/Fonts").ToArray();
        _texture2DFiles = Directory.EnumerateFiles("./Assets/Textures").ToArray();
        _soundFiles = Directory.EnumerateFiles("./Assets/Audio").ToArray();
        
        _loadedFonts = new Dictionary<string, Font>();
        _loadedTextures = new Dictionary<string, Texture2D>();
        _loadedSounds = new Dictionary<string, Sound>();
    }
    

    private void ManageAssetCache()
    {
        ulong size =_assetSizeDict.Values.Aggregate(0UL,(acc, value) => acc + value);

        while (size < AssetCacheLimit){
            var leastPolled = _assetFrequency.MinBy(e=>e.Value).Key;
            _assetFrequency.Remove(leastPolled);
            size = size - _assetSizeDict[leastPolled];
            _assetSizeDict.Remove(leastPolled);
            if(_loadedFonts.ContainsKey(leastPolled))
            {
                Raylib.UnloadFont(_loadedFonts[leastPolled]);
                _loadedFonts.Remove(leastPolled);
            }
            else if(_loadedTextures.ContainsKey(leastPolled))
            { 
                Raylib.UnloadTexture(_loadedTextures[leastPolled]);
                _loadedTextures.Remove(leastPolled);
            }
            else if (_loadedSounds.ContainsKey(leastPolled))
            {
                Raylib.UnloadSound(_loadedSounds[leastPolled]);
                _loadedSounds.Remove(leastPolled);
            }
        }
    }

    private void RegisterAssetUsage(string id){
        if (!_assetFrequency.TryAdd(id, 1))
        {
            _assetFrequency[id] += 1;
        }
    }

    public void ResetUsage(){
        _assetFrequency.Clear();
    }

    public Texture2D GetTexture(string file){
        var filename = Texturepath + file;
        if(_loadedTextures.TryGetValue(file, out var texture)){
            return texture;
        }
        if (!_texture2DFiles.Contains(file)) throw new Exception($"Texture {file} does not exist");
        var fileSize = new FileInfo(file);
        _assetSizeDict.Add(filename,(ulong)fileSize.Length);
        _loadedTextures.Add(file,Raylib.LoadTexture(filename));
        return _loadedTextures[file];
    }
    
    public Font GetFont(string file){
        var filename = Fontpath + file;
        if(_loadedFonts.TryGetValue(file, out var font)){
            return font;
        }
        if (!_fontFiles.Contains(file)) throw new Exception($"Font {file} does not exist");
        var fileSize = new FileInfo(file);
        _assetSizeDict.Add(filename,(ulong)fileSize.Length);
        _loadedFonts.Add(file,Raylib.LoadFont(filename));
        return _loadedFonts[file];
    }


    public Sound GetAudio(string file){
        var filename = Audiopath + file;
        if(_loadedSounds.TryGetValue(file, out var audio)){
            return audio;
        }
        if (!_soundFiles.Contains(file)) throw new Exception($"Sound {file} does not exist");
        var fileSize = new FileInfo(file);
        _assetSizeDict.Add(filename,(ulong)fileSize.Length);
        _loadedSounds.Add(file,Raylib.LoadSound(filename));
        return _loadedSounds[file];
    }

    public void ResolveAssetDelta(){
        
    }
    
    public void Run(){
        
    }
}
