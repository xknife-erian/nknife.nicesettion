using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ConsoleUi;
using ConsoleUi.Console;
using YamlDotNet.Serialization;

namespace Example.ConsoleApp
{
    public class Program
    {
        public static readonly string DatabaseSettingFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatabaseSetting.yaml");

        public static async Task Main(string[] args)
        {
            var container = new ContainerBuilder();

            // 演示将两个选项数据文件读取并载入
            container.RegisterOptionYamlFiles("CoderSetting.yaml", DatabaseSettingFilePath)
                // 将上一步载入的数据绑定到实体类
                .RegisterOption<CoderSetting>()
                // 将上一步载入的数据绑定到实体类
                .RegisterOption<DatabaseSetting>();

            container.RegisterType<ConsoleMenuRunner>();
            container.RegisterType<MainMenu>();

            using var builder = container.Build();
            {
                var runner = builder.Resolve<ConsoleMenuRunner>();
                var mainMenu = builder.Resolve<MainMenu>();
                await runner.Run(mainMenu);
            }
        }
    }

    public class MainMenu : SimpleMenu
    {
        private readonly CoderSetting _coderSetting;

        public MainMenu(CoderSetting coderSetting)
        {
            _coderSetting = coderSetting;
        }

        public void ReadCoderSetting(IMenuContext context)
        {
            context.UserInterface.Info("---");
            Console.WriteLine(_coderSetting.Guid.ToString());
            context.UserInterface.Info("---");
            Console.WriteLine(_coderSetting.SqlSettings[0].Type.ToString());
            Console.WriteLine(_coderSetting.SqlSettings[0].AString);
            Console.WriteLine(_coderSetting.SqlSettings[0].BString);
            Console.WriteLine(_coderSetting.SqlSettings[0].DateTime.ToString("u"));
            Console.WriteLine(_coderSetting.SqlSettings[0].Byte.ToString());
            context.UserInterface.Info("---");
            Console.WriteLine(_coderSetting.DatabaseSettings[0].Type.ToString());
            Console.WriteLine(_coderSetting.DatabaseSettings[0].AString);
            Console.WriteLine(_coderSetting.DatabaseSettings[0].BString);
            Console.WriteLine(_coderSetting.DatabaseSettings[0].DateTime.ToString("u"));
            Console.WriteLine(_coderSetting.DatabaseSettings[0].Byte.ToString());
            context.UserInterface.Info("---");
            Console.WriteLine(_coderSetting.DatabaseSettingMap.Values.ElementAt(0).Type.ToString());
            Console.WriteLine(_coderSetting.DatabaseSettingMap.Values.ElementAt(0).AString);
            Console.WriteLine(_coderSetting.DatabaseSettingMap.Values.ElementAt(0).BString);
            Console.WriteLine(_coderSetting.DatabaseSettingMap.Values.ElementAt(0).DateTime.ToString("u"));
            Console.WriteLine(_coderSetting.DatabaseSettingMap.Values.ElementAt(0).Byte.ToString());
        }

        public void WriteCoderSetting(IMenuContext context)
        {
            var st = new CoderSetting();
            st.Demo();
            var yaml = YamlOption.BuildOptionData(st);
            Console.WriteLine(yaml);
        }
    }

    public class CoderSetting
    {
        /// <summary>
        /// 生成模拟数据
        /// </summary>
        public void Demo()
        {
            Guid = Guid.NewGuid();
            SqlSettings = new SqlSetting[3];
            for (var i = 0; i < SqlSettings.Length; i++) 
                SqlSettings[i] = new SqlSetting();
            DatabaseSettings = new List<DatabaseSetting>();
            for (var i = 0; i < 3; i++) 
                DatabaseSettings.Add(new DatabaseSetting());
            DatabaseSettingMap = new Dictionary<string, DatabaseSetting>();
            for (var i = 0; i < 5; i++) 
                DatabaseSettingMap.Add($"Map{i}", new DatabaseSetting());
        }

        public Guid Guid { get; set; }
        public SqlSetting[] SqlSettings { get; set; }
        public List<DatabaseSetting> DatabaseSettings { get; set; }
        public Dictionary<string, DatabaseSetting> DatabaseSettingMap { get; set; }
    }

    public class SqlSetting : SettingBase
    {
    }

    public class DatabaseSetting : SettingBase
    {
    }

    public class BookSetting : SettingBase
    {
    }

    public abstract class SettingBase
    {
        private static readonly Random _Random = new Random((int) DateTime.Now.Ticks);

        protected SettingBase()
        {
            Type = GetType();
            Guid = Guid.NewGuid();
            AString = Guid.NewGuid().ToString("B");
            BString = Guid.NewGuid().ToString("B").ToUpper();
            DateTime = DateTime.Now.AddDays(-_Random.Next(1000, 9999)).AddSeconds(-_Random.Next(100000, 9999999));
            Int = _Random.Next(100, 999);
            UInt = (uint) Math.Abs(_Random.Next(_Random.Next(10000, 99999)));
            Short = (short) _Random.Next(0, 255);
            UShort = (ushort) _Random.Next(0, 255);
            Double = _Random.NextDouble();
            Float = (float) _Random.NextDouble();
            Decimal = (decimal) _Random.NextDouble();
            Byte = Guid.NewGuid().ToByteArray()[0];
            Bytes = Guid.NewGuid().ToByteArray();
        }

        public Guid Guid { get; set; }
        public Type Type { get; set; }
        public string AString { get; set; }
        public string BString { get; set; }
        public DateTime DateTime { get; set; }
        public int Int { get; set; }
        public uint UInt { get; set; }
        public short Short { get; set; }
        public ushort UShort { get; set; }
        public float Float { get; set; }
        public decimal Decimal { get; set; }
        public double Double { get; set; }
        public byte Byte { get; set; }
        public byte[] Bytes { get; set; }
    }
}