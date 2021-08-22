using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

// ReSharper disable once CheckNamespace
namespace Autofac
{
    public static class AutofacYamlOptionExtensions
    {
        private static readonly Dictionary<string, string> _YamlContentMap = new Dictionary<string, string>();
        private static readonly Dictionary<string, object> _OptionMap = new Dictionary<string, object>();

        /// <summary>
        /// 向系统内部注册以Yaml书写的选项数据文件，注册后，需要链式调用<see cref="RegisterOption{T}"/>以绑定到具体的选项实体类中。
        /// </summary>
        /// <param name="container"></param>
        /// <param name="fileNames">选项数据文件，可以为多个。需要注意的是，文件名必须与选项实体类的类名完全一致。</param>
        /// <returns></returns>
        public static ContainerBuilder RegisterOptionYamlFiles(this ContainerBuilder container, params string[] fileNames)
        {
            if (null == fileNames)
                return container;
            foreach (var fileName in fileNames)
            {
                if (!File.Exists(fileName))
                    break;
                var content = File.ReadAllText(fileName);
                var start = fileName.LastIndexOf(Path.DirectorySeparatorChar) + 1;
                if (start < 0)
                    start = 0;
                var end = fileName.LastIndexOf('.');
                var optionName = fileName.Substring(start, end - start);
                _YamlContentMap.Add(optionName, content);
            }

            return container;
        }

        /// <summary>
        /// 将指定的选项实体类注册到Autofac架构中。注意，选项实体类是以单例形式存在于系统架构中。
        /// 如果指定选项实体类的选项数据没有在本函数之前被<see cref="RegisterOptionYamlFiles"/>载入的话，本函数将跳过注册。
        /// </summary>
        /// <typeparam name="T">指定选项实体类</typeparam>
        /// <param name="container"></param>
        public static ContainerBuilder RegisterOption<T>(this ContainerBuilder container)
        {
            var type = typeof(T);
            var key = type.Name;
            if (!_YamlContentMap.ContainsKey(key))
                // 如果指定选项实体类的选项数据没有在本函数之前被载入的话，本函数将跳过注册。
                return container;
            var deserializer = new Deserializer();
            container.Register<T>(
                    c =>
                    {
                        if (!_OptionMap.ContainsKey(key))
                        {
                            var content = _YamlContentMap[key];
                            var option = deserializer.Deserialize<T>(content);
                            _OptionMap.Add(key, option);
                        }

                        return (T) _OptionMap[key];
                    })
                .AsSelf().AsImplementedInterfaces().SingleInstance();
            return container;
        }
    }

    /// <summary>
    /// 以Yaml表达的选项数据。静态帮助类。
    /// </summary>
    public static class YamlOption
    {
        /// <summary>
        /// 将指定的对象生成以Yaml表达的选项数据
        /// </summary>
        /// <typeparam name="T">指定的对象</typeparam>
        /// <param name="option">指定的对象实例</param>
        /// <returns></returns>
        public static string BuildOptionData<T>(T option) 
            where T : class
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));
            var ys = new Serializer();
            var yaml = ys.Serialize(option);
            return yaml;
        }
    }
}