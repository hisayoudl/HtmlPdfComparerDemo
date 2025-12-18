using HtmlPDFContrastImage.Window;

namespace HtmlPdfComparerDemo;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== HTML/PDF 图片对比工具演示 ===\n");

        // 示例1: 基本对比 - 使用本地文件
        await Example1_BasicComparison();

        // 示例2: 带图片过滤的对比
        await Example2_WithImageFiltering();

        // 示例3: 自定义配置
        await Example3_CustomConfiguration();

        Console.WriteLine("\n所有示例执行完成!");
    }

    /// <summary>
    /// 示例1: 基本对比 - 使用本地文件
    /// </summary>
    static async Task Example1_BasicComparison()
    {
        Console.WriteLine("【示例1】基本对比");
        Console.WriteLine("----------------------------------------");

        try
        {
            // 准备测试文件路径
            string htmlPath = @"D:\MyProject\图片校验20251118\OracleMongoQuery\files\00453116-1-2-1-0-1#3\00453116-1-2-1-0-1#3.html";
            string pdfPath = @"D:\MyProject\图片校验20251118\OracleMongoQuery\files\00453116-1-2-1-0-1#3\00453116-1-2-1-0-1#3.pdf";

            if (!File.Exists(htmlPath) || !File.Exists(pdfPath))
            {
                Console.WriteLine("⚠️ 测试文件不存在,跳过此示例");
                Console.WriteLine($"   请确保以下文件存在:");
                Console.WriteLine($"   - {htmlPath}");
                Console.WriteLine($"   - {pdfPath}");
                Console.WriteLine();
                return;
            }

            // 读取文件内容
            byte[] htmlContent = await File.ReadAllBytesAsync(htmlPath);
            byte[] pdfContent = await File.ReadAllBytesAsync(pdfPath);

            // 创建对比器(使用默认配置)
            using var comparer = new HtmlPdfComparer();

            // 执行对比
            Console.WriteLine("正在对比文件...");
            var result = await comparer.CompareAsync(
                htmlBytes: htmlContent,
                pdfBytes: pdfContent
            );

            // 输出结果
            PrintResult(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 错误: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// 示例2: 带图片过滤的对比
    /// </summary>
    static async Task Example2_WithImageFiltering()
    {
        Console.WriteLine("【示例2】带图片过滤的对比");
        Console.WriteLine("----------------------------------------");

        try
        {
            // 准备测试文件
            string htmlPath = @"D:\MyProject\图片校验20251118\OracleMongoQuery\files\00453116-1-2-1-0-1#3\00453116-1-2-1-0-1#3.html";
            string pdfPath = @"D:\MyProject\图片校验20251118\OracleMongoQuery\files\00453116-1-2-1-0-1#3\00453116-1-2-1-0-1#3.pdf";

            if (!File.Exists(htmlPath) || !File.Exists(pdfPath))
            {
                Console.WriteLine("⚠️ 测试文件不存在,跳过此示例\n");
                return;
            }

            // 准备要过滤的Logo图片
            string[] logoFiles = {
                @"D:\MyProject\图片校验20251118\OracleMongoQuery\test\logo\logo1.png",
                @"D:\MyProject\图片校验20251118\OracleMongoQuery\test\logo\logo2.png"
            };

            // 读取Logo图片为byte[]数组
            List<byte[]> filterImages = new();
            foreach (var logoFile in logoFiles)
            {
                if (File.Exists(logoFile))
                {
                    filterImages.Add(await File.ReadAllBytesAsync(logoFile));
                    Console.WriteLine($"✓ 加载过滤图片: {Path.GetFileName(logoFile)}");
                }
            }

            if (filterImages.Count == 0)
            {
                Console.WriteLine("⚠️ 未找到Logo文件,将不使用过滤功能");
            }

            // 读取文件内容
            byte[] htmlContent = await File.ReadAllBytesAsync(htmlPath);
            byte[] pdfContent = await File.ReadAllBytesAsync(pdfPath);

            // 创建对比器
            using var comparer = new HtmlPdfComparer();

            // 执行对比(带图片过滤)
            Console.WriteLine("\n正在对比文件(应用图片过滤)...");
            var result = await comparer.CompareAsync(
                htmlBytes: htmlContent,
                pdfBytes: pdfContent,
                filterImageBytes: filterImages  // 传入要过滤的图片
            );

            // 输出结果
            PrintResult(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 错误: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// 示例3: 自定义配置
    /// </summary>
    static async Task Example3_CustomConfiguration()
    {
        Console.WriteLine("【示例3】自定义配置");
        Console.WriteLine("----------------------------------------");

        try
        {
            // 准备测试文件
            string htmlPath = @"D:\MyProject\图片校验20251118\OracleMongoQuery\files\00453116-1-2-1-0-1#3\00453116-1-2-1-0-1#3.html";
            string pdfPath = @"D:\MyProject\图片校验20251118\OracleMongoQuery\files\00453116-1-2-1-0-1#3\00453116-1-2-1-0-1#3.pdf";

            if (!File.Exists(htmlPath) || !File.Exists(pdfPath))
            {
                Console.WriteLine("⚠️ 测试文件不存在,跳过此示例\n");
                return;
            }

            // 读取文件内容
            byte[] htmlContent = await File.ReadAllBytesAsync(htmlPath);
            byte[] pdfContent = await File.ReadAllBytesAsync(pdfPath);

            // 创建自定义配置
            var options = new CompareOptions
            {
                // 图片相似度阈值 (0-1之间,越大越严格)
                SimilarityThreshold = 0.95,

                // 感知哈希阈值 (用于过滤相似图片)
                HashThreshold = 0.95,

                // 匹配算法 (默认使用匈牙利算法)
                MatchAlgorithm = MatchAlgorithm.Hungarian,

                // 相似度计算方法
                SimilarityMethod = SimilarityMethod.PerceptualHash,

                // 排除特定名称的图片
                ExcludeImageNames = System.Collections.Immutable.ImmutableList.Create("logo.png", "header.png")
            };

            // 创建对比器(使用自定义配置)
            using var comparer = new HtmlPdfComparer(options);

            // 执行对比
            Console.WriteLine("正在对比文件(使用自定义配置)...");
            var result = await comparer.CompareAsync(
                htmlBytes: htmlContent,
                pdfBytes: pdfContent
            );

            // 输出结果
            PrintResult(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 错误: {ex.Message}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// 打印对比结果
    /// </summary>
    static void PrintResult(CompareResult result)
    {
        Console.WriteLine("\n📊 对比结果:");
        Console.WriteLine($"   HTML图片数: {result.HtmlImageCount} 张");
        Console.WriteLine($"   PDF图片数:  {result.PdfImageCount} 张");
        Console.WriteLine($"   成功匹配:   {result.MatchedCount} 对");
        Console.WriteLine($"   匹配率(HTML): {result.MatchRateByHtml:P2}");
        Console.WriteLine($"   匹配率(PDF):  {result.MatchRateByPdf:P2}");

        if (result.UnmatchedHtmlCount > 0)
        {
            Console.WriteLine($"\n   ⚠️ HTML未匹配图片: {result.UnmatchedHtmlCount} 张");
        }

        if (result.UnmatchedPdfCount > 0)
        {
            Console.WriteLine($"\n   ⚠️ PDF未匹配图片: {result.UnmatchedPdfCount} 张");
        }

        if (result.MatchRateByHtml >= 0.95 && result.MatchRateByPdf >= 0.95)
        {
            Console.WriteLine("\n   ✅ 对比成功! 图片匹配率达到95%以上");
        }
        else if (result.MatchedCount == 0)
        {
            Console.WriteLine("\n   ⚠️ 警告: 没有找到匹配的图片!");
        }
    }
}
