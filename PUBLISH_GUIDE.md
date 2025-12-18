# GitHub 发布指南

本文档说明如何将 HtmlPdfComparerDemo 项目发布到 GitHub。

## 📋 准备工作

### 1. 确保已安装 Git

```bash
git --version
```

如果未安装,请从 [git-scm.com](https://git-scm.com/) 下载安装。

### 2. 配置 Git 用户信息(如果尚未配置)

```bash
git config --global user.name "你的用户名"
git config --global user.email "你的邮箱@example.com"
```

## 🚀 发布步骤

### 步骤1: 在 GitHub 上创建仓库

1. 访问 [GitHub](https://github.com)
2. 点击右上角 **"+"** → **"New repository"**
3. 填写仓库信息:
   - **Repository name**: `HtmlPdfComparerDemo`
   - **Description**: `HTML/PDF文档图片对比工具演示项目 - 展示如何使用 HtmlPDFContrastImage.Window NuGet包`
   - **Visibility**: Public (公开) 或 Private (私有)
   - **不要**勾选 "Initialize this repository with a README" (因为本地已有README)
4. 点击 **"Create repository"**

### 步骤2: 关联远程仓库

复制GitHub上显示的仓库URL(例如: `https://github.com/你的用户名/HtmlPdfComparerDemo.git`)

在项目目录执行:

```bash
cd "d:\MyProject\图片校验20251118\HtmlPdfComparerDemo"
git remote add origin https://github.com/你的用户名/HtmlPdfComparerDemo.git
```

### 步骤3: 推送代码到 GitHub

```bash
# 推送到main分支(如果GitHub默认分支是main)
git branch -M main
git push -u origin main

# 或者推送到master分支(如果GitHub默认分支是master)
git push -u origin master
```

**首次推送可能需要登录:**
- 用户名: 你的GitHub用户名
- 密码: 建议使用 Personal Access Token (不是账号密码)

### 步骤4: 创建 Personal Access Token (如需要)

如果推送时要求输入密码,需要创建 Personal Access Token:

1. 访问 GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
2. 点击 **"Generate new token"** → **"Generate new token (classic)"**
3. 填写信息:
   - **Note**: `HtmlPdfComparerDemo`
   - **Expiration**: 选择过期时间
   - **Scopes**: 勾选 `repo` (完整的仓库访问权限)
4. 点击 **"Generate token"**
5. **立即复制Token** (只显示一次)
6. 使用Token作为密码进行推送

### 步骤5: 验证发布成功

访问你的GitHub仓库页面,应该能看到以下文件:

```
HtmlPdfComparerDemo/
├── .gitignore
├── API_USAGE.md
├── HtmlPdfComparerDemo.csproj
├── LICENSE
├── Program.cs
└── README.md
```

## 📝 后续更新

当你修改代码后,可以使用以下命令更新GitHub仓库:

```bash
# 1. 查看修改状态
git status

# 2. 添加修改的文件
git add .

# 3. 提交修改
git commit -m "描述你的修改内容"

# 4. 推送到GitHub
git push
```

## 🏷️ 创建 Release (可选)

为项目创建正式版本:

1. 在GitHub仓库页面,点击 **"Releases"** → **"Create a new release"**
2. 填写版本信息:
   - **Tag version**: `v1.0.0`
   - **Release title**: `v1.0.0 - 初始版本`
   - **Description**: 描述版本特性
3. 点击 **"Publish release"**

## 📄 添加 README 徽章 (可选)

在 README.md 顶部添加徽章,让项目更专业:

```markdown
[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![GitHub stars](https://img.shields.io/github/stars/你的用户名/HtmlPdfComparerDemo.svg)](https://github.com/你的用户名/HtmlPdfComparerDemo/stargazers)
```

## 🔗 项目链接配置

### 更新 README.md 中的链接

将 README.md 中的占位符替换为实际链接:

```markdown
- NuGet包: [HtmlPDFContrastImage.Window](https://www.nuget.org/packages/HtmlPDFContrastImage.Window)
- 问题反馈: [GitHub Issues](https://github.com/你的用户名/HtmlPdfComparerDemo/issues)
```

### 更新联系方式

```markdown
- 提交 GitHub Issue
- Email: 你的邮箱@example.com
```

## 🎯 推荐的仓库设置

### 1. 添加 Topics

在GitHub仓库页面,点击设置图标添加Topics:
- `csharp`
- `dotnet`
- `pdf`
- `html`
- `image-comparison`
- `nuget-package`
- `document-processing`

### 2. 启用 GitHub Pages (可选)

如果想展示文档:
1. Settings → Pages
2. Source: Deploy from a branch
3. Branch: main / docs
4. Save

### 3. 设置仓库描述

在仓库页面顶部点击"Edit"添加描述:
```
HTML/PDF文档图片对比工具演示 - 使用HtmlPDFContrastImage.Window包进行文档质量检查
```

## 🛠️ 常见问题

### Q1: 推送时提示 "Permission denied"

**解决方案:**
- 检查远程仓库URL是否正确
- 使用Personal Access Token作为密码
- 或配置SSH密钥

### Q2: 推送时提示 "fatal: refusing to merge unrelated histories"

**解决方案:**
```bash
git pull origin main --allow-unrelated-histories
git push -u origin main
```

### Q3: 如何删除敏感信息?

如果不小心提交了敏感信息:
```bash
# 从历史中移除文件
git filter-branch --force --index-filter \
  "git rm --cached --ignore-unmatch 文件路径" \
  --prune-empty --tag-name-filter cat -- --all

# 强制推送
git push origin --force --all
```

## 📧 获取帮助

如有问题:
1. 查看 [GitHub文档](https://docs.github.com/)
2. 访问 [Git官方文档](https://git-scm.com/doc)
3. 在项目仓库提交Issue

---

**祝你发布成功! 🎉**
