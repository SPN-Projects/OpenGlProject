using GameEngine.Logging;
using GameEngine.Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GameEngine.Configs;
public class ConfigManager
{
    /// <summary>
    /// Tries to load the config file at the given path by deserializing the config object to a .yaml file.
    /// If the file does not exist it will create a new config file with the default values
    /// </summary>
    /// <typeparam name="T">The type of the config</typeparam>
    /// <param name="path">The path to the config file</param>
    /// <returns> The config object or <see langword="null"/> if the config could not be loaded</returns>
    public static T? LoadConfig<T>(string path) where T : class, new()
    {
        try
        {
            PathChecker.EnsureFile(path);

            var deserialize = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            var yamlConfig = File.ReadAllText(path);
            var config = deserialize.Deserialize<T>(yamlConfig);

            return config;
        }
        catch (Exception e)
        {
            Logger.EngineLogger.Error($"Failed to load config file: {path}", e.Message);
        }

        try
        {
            var newConfig = new T();
            SaveConfig(path, newConfig);

            return newConfig;
        }
        catch (Exception e)
        {
            Logger.EngineLogger.Error($"Failed to create new config file: {path}", e.Message);
        }

        return null;
    }

    /// <summary>
    /// Save the config to a file at the given path
    /// </summary>
    /// <typeparam name="T">The type of the config</typeparam>
    /// <param name="path">The path to save the config file</param>
    /// <param name="config">The config to save</param>
    /// <exception cref="Exception">Throws an exception if the config file could not be saved</exception>
    public static void SaveConfig<T>(string path, T config) where T : class
    {
        try
        {
            var serialize = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            var yamlConfig = serialize.Serialize(config);

            _ = Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            File.WriteAllText(path, yamlConfig);
        }
        catch (Exception e)
        {
            Logger.EngineLogger.Error($"Failed to save config file: {path}", e.Message);
            throw new Exception("Failed to save config file.", e);
        }
    }
}
