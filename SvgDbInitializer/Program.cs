using System;
using System.Linq;
using System.Threading.Tasks;
using SvgColorNormalizer.Core.DataSources;

namespace SvgDbInitializer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("SVG数据库初始化工具");
            Console.WriteLine("====================");

            try
            {
                string dbPath = "svg_assets.db";
                Console.WriteLine($"正在初始化数据库: {dbPath}");

                var source = new SqliteSvgSource(dbPath);

                // 初始化数据库表结构
                Console.WriteLine("正在创建表结构...");
                await source.InitializeDatabase();

                // 插入示例数据
                Console.WriteLine("正在插入示例数据...");
                await source.InsertSampleData();

                // 验证数据插入
                Console.WriteLine("正在验证数据...");
                var svgDataList = await source.LoadAsync();
                var count = svgDataList.Count();

                Console.WriteLine($"\n初始化完成！");
                Console.WriteLine($"成功插入 {count} 个SVG示例数据");
                Console.WriteLine($"数据库文件: {dbPath}");

                Console.WriteLine("\n按任意键退出...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
        }
    }
}