using GameEngine.Graphics.Textures;

namespace GameEngine.Extensions;
public static class TextureListExtensions
{
    public static int[] ToCapacitySamplers<T>(this List<T> textures) where T : Texture
    {
        var samplers = new int[textures.Capacity];
        for (var i = 0; i < textures.Capacity; i++)
        {
            samplers[i] = i;
        }

        return samplers;
    }
}
