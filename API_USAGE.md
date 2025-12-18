# HtmlPdfComparer API 使用文档

## 📚 目录

- [快速开始](#快速开始)
- [核心API](#核心api)
  - [HtmlPdfComparer 类](#htmlpdfcomparer-类)
  - [CompareOptions 配置](#compareoptions-配置)
  - [CompareResult 结果](#compareresult-结果)
- [使用场景](#使用场景)
- [最佳实践](#最佳实践)

---

## 快速开始

### 安装包

```bash
dotnet add package HtmlPDFContrastImage.Window
```

### 基本用法

```csharp
using HtmlPDFContrastImage.Window;

// 读取文件
byte[] htmlContent = await File.ReadAllBytesAsync("test.html");
byte[] pdfContent = await File.ReadAllBytesAsync("test.pdf");

// 创建对比器
using var comparer = new HtmlPdfComparer();

// 执行对比
var result = await comparer.CompareAsync(
    htmlBytes: htmlContent,
    pdfBytes: pdfContent
);

// 检查结果
if (result.MatchRateByHtml >= 0.95)
{
    Console.WriteLine("✅ 文档对比成功!");
}
```

---

## 核心API

### HtmlPdfComparer 类

主对比器类,提供HTML/PDF文档图片对比功能,比如pdf文件有10张图片，html里面有10张图片，就按照顺序一一对比是否里面的图片完全一致，返回对比结果。

#### 构造函数

```csharp
public HtmlPdfComparer(
    CompareOptions? options = null,
    HttpClient? httpClient = null,
    int maxConcurrency = 1
)
```

**参数:**
- `options`: 对比配置选项(可选,默认使用 `CompareOptions.Default`)
- `httpClient`: HTTP客户端(可选,用于URL下载,默认创建新实例)
- `maxConcurrency`: 最大并发数(默认1)

**示例:**

```csharp
// 使用默认配置
using var comparer1 = new HtmlPdfComparer();

// 使用自定义配置
var options = new CompareOptions
{
    SimilarityThreshold = 0.90,
    HashThreshold = 0.95
};
using var comparer2 = new HtmlPdfComparer(options);

// 使用自定义HttpClient
var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
using var comparer3 = new HtmlPdfComparer(httpClient: httpClient);
```

#### CompareAsync 方法

执行HTML和PDF文档的图片对比。

```csharp
public async Task<CompareResult> CompareAsync(
    string? htmlSource = null,
    byte[]? htmlBytes = null,
    string? pdfSource = null,
    byte[]? pdfBytes = null,
    IEnumerable<byte[]>? filterImageBytes = null,
    CancellationToken cancellationToken = default
)
```

**参数:**
- `htmlSource`: HTML来源(文件路径、URL或null)
- `htmlBytes`: HTML字节数组(可选)
- `pdfSource`: PDF来源(文件路径、URL或null)
- `pdfBytes`: PDF字节数组(可选)
- `filterImageBytes`: 要过滤的图片字节数组列表(可选)
- `cancellationToken`: 取消令牌(可选)

**返回值:**
- `CompareResult`: 对比结果

**使用场景:**

##### 1. 使用byte[]数组

```csharp
byte[] htmlContent = await File.ReadAllBytesAsync("test.html");
byte[] pdfContent = await File.ReadAllBytesAsync("test.pdf");

var result = await comparer.CompareAsync(
    htmlBytes: htmlContent,
    pdfBytes: pdfContent
);
```

##### 2. 使用文件路径

```csharp
var result = await comparer.CompareAsync(
    htmlSource: "C:\\Documents\\test.html",
    pdfSource: "C:\\Documents\\test.pdf"
);
```

##### 3. 使用URL

```csharp
var result = await comparer.CompareAsync(
    htmlSource: "https://example.com/document.html",
    pdfSource: "https://example.com/document.pdf"
);
```

##### 4. 混合使用

```csharp
// HTML从文件读取, PDF从URL下载
var result = await comparer.CompareAsync(
    htmlSource: "C:\\Documents\\test.html",
    pdfSource: "https://example.com/document.pdf"
);

// HTML从byte[], PDF从文件
byte[] htmlContent = GetHtmlFromDatabase();
var result = await comparer.CompareAsync(
    htmlBytes: htmlContent,
    pdfSource: "C:\\Documents\\test.pdf"
);
```

##### 5. 带图片过滤

```csharp
// 准备要过滤的Logo图片
List<byte[]> logos = new();
logos.Add(await File.ReadAllBytesAsync("logo1.png"));
logos.Add(await File.ReadAllBytesAsync("logo2.png"));

var result = await comparer.CompareAsync(
    htmlBytes: htmlContent,
    pdfBytes: pdfContent,
    filterImageBytes: logos
);
```

##### 6. 支持取消

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

try
{
    var result = await comparer.CompareAsync(
        htmlBytes: htmlContent,
        pdfBytes: pdfContent,
        cancellationToken: cts.Token
    );
}
catch (OperationCanceledException)
{
    Console.WriteLine("对比操作已取消");
}
```

---

### CompareOptions 配置

对比器的配置选项。

#### 属性

```csharp
public record CompareOptions
{
    public ImmutableList<string> ExcludeImageNames { get; init; }
    public ImmutableList<string> ExcludeImagePaths { get; init; }
    public double HashThreshold { get; init; }
    public double SimilarityThreshold { get; init; }
    public MatchAlgorithm MatchAlgorithm { get; init; }
    public SimilarityMethod SimilarityMethod { get; init; }
}
```

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `ExcludeImageNames` | ImmutableList\<string\> | 空列表 | 要排除的图片文件名列表 |
| `ExcludeImagePaths` | ImmutableList\<string\> | 空列表 | 要排除的图片路径列表 |
| `HashThreshold` | double | 0.95 | 感知哈希相似度阈值(0.0-1.0) |
| `SimilarityThreshold` | double | 0.85 | 图片相似度阈值(0.0-1.0) |
| `MatchAlgorithm` | MatchAlgorithm | Hungarian | 匹配算法 |
| `SimilarityMethod` | SimilarityMethod | PerceptualHash | 相似度计算方法 |

#### 配置示例

```csharp
using System.Collections.Immutable;

// 1. 使用默认配置
var options1 = CompareOptions.Default;

// 2. 自定义部分配置
var options2 = new CompareOptions
{
    SimilarityThreshold = 0.90,
    HashThreshold = 0.95
};

// 3. 完整配置
var options3 = new CompareOptions
{
    // 排除特定名称的图片
    ExcludeImageNames = ImmutableList.Create("logo.png", "header.png", "footer.png"),
    
    // 排除特定路径的图片
    ExcludeImagePaths = ImmutableList.Create(
        "C:\\Images\\logo1.png",
        "C:\\Images\\logo2.png"
    ),
    
    // 感知哈希阈值(用于快速过滤相似图片)
    HashThreshold = 0.95,
    
    // 相似度阈值(用于精确匹配)
    SimilarityThreshold = 0.85,
    
    // 使用匈牙利算法(推荐)
    MatchAlgorithm = MatchAlgorithm.Hungarian,
    
    // 使用感知哈希方法(推荐)
    SimilarityMethod = SimilarityMethod.PerceptualHash
};

// 4. 从现有配置修改
var options4 = CompareOptions.Default with
{
    SimilarityThreshold = 0.90,
    ExcludeImageNames = ImmutableList.Create("logo.png")
};
```

#### 阈值调优建议

**SimilarityThreshold (相似度阈值)**

| 值范围 | 适用场景 | 说明 |
|--------|----------|------|
| 0.95-1.0 | 严格匹配 | 要求图片几乎完全一致,适用于无损转换场景 |
| 0.85-0.95 | 标准匹配(推荐) | 容忍轻微的压缩和格式转换,适用于大多数场景 |
| 0.70-0.85 | 宽松匹配 | 容忍较大的质量损失,适用于低质量图片 |
| < 0.70 | 不推荐 | 可能导致误匹配 |

**HashThreshold (哈希阈值)**

| 值范围 | 适用场景 | 说明 |
|--------|----------|------|
| 0.95-1.0 | 精确过滤 | 只过滤几乎相同的图片 |
| 0.90-0.95 | 标准过滤(推荐) | 过滤相似度高的图片(如Logo) |
| < 0.90 | 宽松过滤 | 可能过滤掉部分不同的图片 |

---

### CompareResult 结果

对比操作的结果。

#### 属性

```csharp
public class CompareResult
{
    public bool Success { get; init; }
    public int HtmlImageCount { get; init; }
    public int PdfImageCount { get; init; }
    public int MatchedCount { get; init; }
    public int UnmatchedHtmlCount { get; init; }
    public int UnmatchedPdfCount { get; init; }
    public double MatchRateByHtml { get; init; }
    public double MatchRateByPdf { get; init; }
    public TimeSpan ElapsedTime { get; init; }
    public List<string> Errors { get; init; }
    public List<ImagePairInfo> MatchedPairs { get; init; }
}
```

| 属性 | 类型 | 说明 |
|------|------|------|
| `Success` | bool | 对比是否成功 |
| `HtmlImageCount` | int | HTML文档中的图片总数 |
| `PdfImageCount` | int | PDF文档中的图片总数 |
| `MatchedCount` | int | 成功匹配的图片对数 |
| `UnmatchedHtmlCount` | int | HTML中未匹配的图片数 |
| `UnmatchedPdfCount` | int | PDF中未匹配的图片数 |
| `MatchRateByHtml` | double | 基于HTML的匹配率(0.0-1.0) |
| `MatchRateByPdf` | double | 基于PDF的匹配率(0.0-1.0) |
| `ElapsedTime` | TimeSpan | 处理耗时 |
| `Errors` | List\<string\> | 错误信息列表 |
| `MatchedPairs` | List\<ImagePairInfo\> | 匹配的图片对详细信息 |

#### 结果判断示例

```csharp
var result = await comparer.CompareAsync(...);

// 1. 判断对比是否成功
if (!result.Success)
{
    Console.WriteLine("对比失败:");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  - {error}");
    }
    return;
}

// 2. 判断匹配率
if (result.MatchRateByHtml >= 0.95 && result.MatchRateByPdf >= 0.95)
{
    Console.WriteLine("✅ 文档对比成功! 图片高度一致");
}
else if (result.MatchRateByHtml >= 0.80)
{
    Console.WriteLine("⚠️ 文档基本一致,但存在部分差异");
}
else
{
    Console.WriteLine("❌ 文档差异较大,请人工检查");
}

// 3. 检查未匹配图片
if (result.UnmatchedHtmlCount > 0)
{
    Console.WriteLine($"HTML中有 {result.UnmatchedHtmlCount} 张图片未匹配");
}

if (result.UnmatchedPdfCount > 0)
{
    Console.WriteLine($"PDF中有 {result.UnmatchedPdfCount} 张图片未匹配");
}

// 4. 查看详细匹配信息
Console.WriteLine($"\n匹配详情:");
foreach (var pair in result.MatchedPairs)
{
    Console.WriteLine($"  HTML[{pair.HtmlIndex}] ↔ PDF[{pair.PdfIndex}]: {pair.Similarity:P2}");
}

// 5. 性能监控
Console.WriteLine($"\n处理耗时: {result.ElapsedTime.TotalSeconds:F2} 秒");
```

#### ImagePairInfo 类

```csharp
public class ImagePairInfo
{
    public int HtmlIndex { get; init; }       // HTML图片索引
    public int PdfIndex { get; init; }        // PDF图片索引
    public double Similarity { get; init; }   // 相似度分数(0.0-1.0)
}
```

---

## 使用场景

### 场景1: 批量文档对比

```csharp
async Task BatchCompareAsync(List<string> htmlFiles, List<string> pdfFiles)
{
    using var comparer = new HtmlPdfComparer();
    var results = new List<CompareResult>();

    for (int i = 0; i < htmlFiles.Count; i++)
    {
        var result = await comparer.CompareAsync(
            htmlSource: htmlFiles[i],
            pdfSource: pdfFiles[i]
        );
        
        results.Add(result);
        
        Console.WriteLine($"[{i+1}/{htmlFiles.Count}] {Path.GetFileName(htmlFiles[i])}: " +
                         $"匹配率 {result.MatchRateByHtml:P2}");
    }

    // 统计
    var successCount = results.Count(r => r.MatchRateByHtml >= 0.95);
    Console.WriteLine($"\n总计: {results.Count} 个文档, {successCount} 个成功");
}
```

### 场景2: 集成到测试框架

```csharp
[Test]
public async Task DocumentConversion_ShouldPreserveImages()
{
    // Arrange
    var htmlContent = GenerateHtml();
    var pdfContent = ConvertToPdf(htmlContent);
    
    using var comparer = new HtmlPdfComparer(new CompareOptions
    {
        SimilarityThreshold = 0.95
    });

    // Act
    var result = await comparer.CompareAsync(
        htmlBytes: htmlContent,
        pdfBytes: pdfContent
    );

    // Assert
    Assert.That(result.Success, Is.True);
    Assert.That(result.MatchRateByHtml, Is.GreaterThanOrEqualTo(0.95));
    Assert.That(result.MatchRateByPdf, Is.GreaterThanOrEqualTo(0.95));
}
```

### 场景3: Web API集成

```csharp
[HttpPost("compare")]
public async Task<IActionResult> CompareDocuments(
    [FromForm] IFormFile htmlFile,
    [FromForm] IFormFile pdfFile)
{
    using var htmlStream = new MemoryStream();
    using var pdfStream = new MemoryStream();
    
    await htmlFile.CopyToAsync(htmlStream);
    await pdfFile.CopyToAsync(pdfStream);

    using var comparer = new HtmlPdfComparer();
    var result = await comparer.CompareAsync(
        htmlBytes: htmlStream.ToArray(),
        pdfBytes: pdfStream.ToArray()
    );

    return Ok(new
    {
        success = result.Success,
        matchRate = result.MatchRateByHtml,
        htmlImageCount = result.HtmlImageCount,
        pdfImageCount = result.PdfImageCount,
        matchedCount = result.MatchedCount
    });
}
```

### 场景4: 定时任务

```csharp
public class DocumentComparisonJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var options = new CompareOptions
        {
            SimilarityThreshold = 0.90,
            ExcludeImageNames = ImmutableList.Create("logo.png", "watermark.png")
        };

        using var comparer = new HtmlPdfComparer(options);
        
        var documentPairs = await GetPendingDocumentsAsync();
        
        foreach (var pair in documentPairs)
        {
            var result = await comparer.CompareAsync(
                htmlSource: pair.HtmlPath,
                pdfSource: pair.PdfPath
            );

            await SaveResultAsync(pair.Id, result);
            
            if (result.MatchRateByHtml < 0.95)
            {
                await SendAlertAsync(pair.Id, result);
            }
        }
    }
}
```

---

## 最佳实践

### 1. 资源管理

```csharp
// ✅ 推荐: 使用using语句
using var comparer = new HtmlPdfComparer();
var result = await comparer.CompareAsync(...);

// ❌ 不推荐: 忘记释放资源
var comparer = new HtmlPdfComparer();
var result = await comparer.CompareAsync(...);
// comparer未释放,可能导致内存泄漏
```

### 2. 异常处理

```csharp
try
{
    var result = await comparer.CompareAsync(...);
    
    if (!result.Success)
    {
        // 处理业务失败
        LogErrors(result.Errors);
    }
}
catch (FileNotFoundException ex)
{
    // 文件不存在
    Console.WriteLine($"文件未找到: {ex.FileName}");
}
catch (HttpRequestException ex)
{
    // URL下载失败
    Console.WriteLine($"下载失败: {ex.Message}");
}
catch (OperationCanceledException)
{
    // 操作被取消
    Console.WriteLine("操作已取消");
}
catch (Exception ex)
{
    // 其他异常
    Console.WriteLine($"未知错误: {ex.Message}");
}
```

### 3. 性能优化

```csharp
// 1. 复用HttpClient
var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
using var comparer = new HtmlPdfComparer(httpClient: httpClient);

// 2. 使用取消令牌避免长时间等待
var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
var result = await comparer.CompareAsync(..., cancellationToken: cts.Token);

// 3. 批量处理时复用comparer实例
using var comparer = new HtmlPdfComparer();
foreach (var pair in documentPairs)
{
    var result = await comparer.CompareAsync(...);
    // 处理结果
}
```

### 4. 配置调优

```csharp
// 根据实际情况调整阈值
var options = new CompareOptions
{
    // 高质量转换: 使用高阈值
    SimilarityThreshold = 0.95,
    
    // 有损压缩: 降低阈值
    // SimilarityThreshold = 0.80,
    
    // Logo过滤: 使用高哈希阈值
    HashThreshold = 0.95,
    
    // 推荐使用匈牙利算法(全局最优)
    MatchAlgorithm = MatchAlgorithm.Hungarian,
    
    // 推荐使用感知哈希(抗变形)
    SimilarityMethod = SimilarityMethod.PerceptualHash
};
```

### 5. 日志记录

```csharp
var result = await comparer.CompareAsync(...);

_logger.LogInformation(
    "文档对比完成: HTML={HtmlCount}, PDF={PdfCount}, 匹配={MatchCount}, " +
    "匹配率={MatchRate:P2}, 耗时={Elapsed}ms",
    result.HtmlImageCount,
    result.PdfImageCount,
    result.MatchedCount,
    result.MatchRateByHtml,
    result.ElapsedTime.TotalMilliseconds
);

if (!result.Success)
{
    _logger.LogError("对比失败: {Errors}", string.Join(", ", result.Errors));
}
```

---

## 常见问题

### Q1: 如何提高匹配率?

```csharp
// 1. 降低相似度阈值
var options = new CompareOptions { SimilarityThreshold = 0.80 };

// 2. 过滤Logo等干扰图片
var logos = new List<byte[]> { logo1, logo2 };
var result = await comparer.CompareAsync(..., filterImageBytes: logos);

// 3. 排除特定名称的图片
var options = new CompareOptions
{
    ExcludeImageNames = ImmutableList.Create("logo.png", "watermark.png")
};
```

### Q2: 如何处理大文件?

```csharp
// 使用流式读取,避免一次性加载到内存
using var htmlStream = File.OpenRead("large.html");
using var pdfStream = File.OpenRead("large.pdf");

using var htmlMs = new MemoryStream();
using var pdfMs = new MemoryStream();

await htmlStream.CopyToAsync(htmlMs);
await pdfStream.CopyToAsync(pdfMs);

var result = await comparer.CompareAsync(
    htmlBytes: htmlMs.ToArray(),
    pdfBytes: pdfMs.ToArray()
);
```

### Q3: 如何调试匹配失败?

```csharp
var result = await comparer.CompareAsync(...);

// 查看详细错误
if (!result.Success)
{
    Console.WriteLine("错误列表:");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  - {error}");
    }
}

// 查看匹配详情
Console.WriteLine("\n匹配详情:");
foreach (var pair in result.MatchedPairs)
{
    Console.WriteLine($"HTML[{pair.HtmlIndex}] ↔ PDF[{pair.PdfIndex}]: " +
                     $"相似度={pair.Similarity:P2}");
}

// 检查未匹配数量
Console.WriteLine($"\n未匹配: HTML={result.UnmatchedHtmlCount}, " +
                 $"PDF={result.UnmatchedPdfCount}");
```

---

## 版本历史

### 1.0.0 (2025-12-18)
- ✨ 初始版本发布
- ✅ 支持HTML/PDF图片对比
- ✅ 支持图片过滤功能
- ✅ 支持感知哈希算法
- ✅ 支持匈牙利算法匹配
