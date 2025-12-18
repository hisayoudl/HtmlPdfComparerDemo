# HtmlPdfComparerDemo

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

HTML/PDF文档图片对比工具演示项目 - 展示如何使用 `HtmlPDFContrastImage.Window` NuGet包进行文档图片对比。

## 📖 项目简介

本项目演示了如何使用 `HtmlPDFContrastImage.Window` 包来对比HTML和PDF文档中的图片内容,验证两者的一致性。适用于文档转换质量检查、内容校验等场景。

## ✨ 核心功能

- ✅ **HTML/PDF图片对比** - 自动提取并对比HTML和PDF中的所有图片
- ✅ **图片过滤功能** - 支持过滤Logo等重复图片,避免干扰对比结果
- ✅ **感知哈希算法** - 使用pHash算法进行图片相似度计算,抗压缩和微小变形
- ✅ **匈牙利算法匹配** - 智能匹配图片对,找到最优匹配方案
- ✅ **灵活配置** - 支持自定义相似度阈值、匹配算法等参数
- ✅ **详细报告** - 提供完整的对比结果,包括匹配率、未匹配图片列表等

## 🚀 快速开始

### 环境要求

- .NET 8.0 SDK 或更高版本
- Windows操作系统 (依赖Windows平台的PDF转换组件)

### 安装依赖

```bash
# 克隆项目
git clone <your-repo-url>
cd HtmlPdfComparerDemo

# 还原依赖
dotnet restore
```

### 运行示例

```bash
dotnet run
```

## 📚 使用示例

### 示例1: 基本对比

最简单的使用方式 - 对比两个文档中的图片:

```csharp
using HtmlPDFContrastImage.Window;

// 读取文件内容
byte[] htmlContent = await File.ReadAllBytesAsync("test.html");
byte[] pdfContent = await File.ReadAllBytesAsync("test.pdf");

// 创建对比器(使用默认配置)
using var comparer = new HtmlPdfComparer();

// 执行对比
var result = await comparer.CompareAsync(
    htmlBytes: htmlContent,
    pdfBytes: pdfContent
);

// 输出结果
Console.WriteLine($"HTML图片数: {result.HtmlImageCount}");
Console.WriteLine($"PDF图片数: {result.PdfImageCount}");
Console.WriteLine($"成功匹配: {result.MatchedCount} 对");
Console.WriteLine($"匹配率: {result.MatchRateByHtml:P2}");
```

### 示例2: 带图片过滤

过滤掉Logo等重复图片,只对比文档主体内容:

```csharp
// 准备要过滤的Logo图片
List<byte[]> filterImages = new();
filterImages.Add(await File.ReadAllBytesAsync("logo1.png"));
filterImages.Add(await File.ReadAllBytesAsync("logo2.png"));

// 执行对比(带图片过滤)
var result = await comparer.CompareAsync(
    htmlBytes: htmlContent,
    pdfBytes: pdfContent,
    filterImageBytes: filterImages  // 传入要过滤的图片
);
```

### 示例3: 自定义配置

根据实际需求调整对比参数:

```csharp
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
var result = await comparer.CompareAsync(...);
```

## 🔧 配置说明

### CompareOptions 配置项

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `SimilarityThreshold` | double | 0.85 | 图片相似度阈值(0-1),越大越严格 |
| `HashThreshold` | double | 0.95 | 感知哈希阈值,用于过滤相似图片 |
| `MatchAlgorithm` | MatchAlgorithm | Hungarian | 匹配算法:Hungarian(匈牙利算法)或Greedy(贪婪算法) |
| `SimilarityMethod` | SimilarityMethod | PerceptualHash | 相似度计算方法:PerceptualHash或StructuralSimilarity |
| `ExcludeImageNames` | string[] | 空数组 | 要排除的图片文件名列表 |

### CompareResult 结果字段

| 字段 | 类型 | 说明 |
|------|------|------|
| `HtmlImageCount` | int | HTML文档中的图片总数 |
| `PdfImageCount` | int | PDF文档中的图片总数 |
| `MatchedCount` | int | 成功匹配的图片对数 |
| `MatchRateByHtml` | double | 基于HTML的匹配率(0-1) |
| `MatchRateByPdf` | double | 基于PDF的匹配率(0-1) |
| `UnmatchedHtmlCount` | int | HTML中未匹配的图片数 |
| `UnmatchedPdfCount` | int | PDF中未匹配的图片数 |

## 🎯 应用场景

1. **文档转换质量检查** - 验证PDF转HTML、Word转PDF等转换过程中图片是否完整
2. **版本对比** - 对比文档不同版本的图片变化
3. **内容校验** - 确保发布的HTML版本与PDF版本内容一致
4. **自动化测试** - 集成到CI/CD流程,自动检查文档质量

## 🛠️ 技术原理

### 图片提取

- **HTML图片提取**: 使用HtmlAgilityPack解析HTML,提取`<img>`标签中的base64图片
- **PDF图片提取**: PDF → RTF → HTML转换链,使用SautinSoft.PdfFocus提取
  - 采用Flowing模式渲染,保证图片顺序正确
  - 支持智能多策略提取,选择最佳结果

### 图片对比

1. **感知哈希(pHash)**: 计算图片的感知哈希值,抗压缩和微小变形
   - 将图片缩放到32x32灰度图
   - 计算DCT(离散余弦变换)
   - 提取低频信息作为哈希值

2. **相似度计算**: 比较两个哈希值的汉明距离
   - 距离越小,图片越相似
   - 阈值可配置,适应不同精度需求

3. **匹配算法**: 使用匈牙利算法找到最优匹配
   - 构建相似度矩阵
   - 求解最大权重二部图匹配
   - 保证全局最优解

### 图片过滤

- 支持通过byte[]数组过滤多张Logo图片
- 使用临时文件机制传递过滤配置
- 自动清理临时资源

## 📁 项目结构

```
HtmlPdfComparerDemo/
├── Program.cs              # 主程序,包含3个示例
├── HtmlPdfComparerDemo.csproj
├── README.md               # 本文档
└── LICENSE                 # MIT许可证
```

## 🧪 测试数据说明

项目中的示例使用以下测试数据:

```测试文件
/files/00453116-1-2-1-0-1#3/
├── 00453116-1-2-1-0-1#3.html  # HTML文档
└── 00453116-1-2-1-0-1#3.pdf   # PDF文档
、、过滤标题图片在对比
/test/logo/
├── logo1.png                   # Logo图片1
└── logo2.png                   # Logo图片2
```

**测试结果预期**:
- 无过滤: HTML 37张, PDF 42张
- 带过滤: HTML 36张, PDF 36张, 100%匹配

## 📝 常见问题

### Q1: 为什么匹配率不是100%?

**A**: 可能的原因:
1. PDF和HTML中的图片数量本身不一致
2. 图片质量差异较大(压缩、缩放等)
3. 相似度阈值设置过高,可以适当降低`SimilarityThreshold`
4. 存在Logo等重复图片干扰,建议使用图片过滤功能

### Q2: 如何提高对比精度?

**A**: 建议:
1. 使用感知哈希算法(`SimilarityMethod.PerceptualHash`)
2. 调整相似度阈值到0.90-0.95之间
3. 过滤掉Logo等重复图片
4. 确保HTML和PDF是从同一源文档生成的

### Q3: 支持哪些图片格式?

**A**: 支持常见图片格式:
- PNG、JPEG、BMP、GIF等
- HTML中的base64嵌入图片
- PDF中的嵌入图片

### Q4: 可以用于大批量文档对比吗?

**A**: 可以,建议:
1. 控制并发数,避免内存溢出
2. 使用异步方法,提高效率
3. 及时释放资源(`using`语句)
4. 分批处理大量文档

## 🤝 贡献

欢迎提交Issue和Pull Request!

## 📄 许可证

本项目采用 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件

## 🔗 相关链接


## 📧 联系方式


---

**注意**: 本项目依赖Windows平台的PDF转换组件,不支持Linux/macOS。如需跨平台支持,请关注后续版本更新。
