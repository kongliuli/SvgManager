# SVG颜色标准化工具 - .NET 10 WinForms项目设计

## 项目概述

基于参考文档设计的SVG颜色标准化工具，使用.NET 10 WinForms实现，支持从多种数据源加载SVG文件并自动将所有颜色格式统一转换为RGB/RGBA格式。

## 核心功能

- **多数据源支持**：本地文件夹、SQLite数据库、达梦数据库
- **颜色格式统一**：rgb/rgba、6位/8位16进制、命名颜色 → 统一转为 rgb/rgba
- **批量处理能力**：支持大规模文件处理
- **可视化界面**：实时查看处理进度和颜色变更详情
- **详细报告**：导出处理结果和颜色变更记录

## 技术栈

- **.NET 10.0**
- **WinForms**
- **System.Data.SQLite**
- **System.Xml.Linq**
- **DmProvider**（达梦数据库，可选）

## 项目结构

```
SvgColorNormalizer/
├── Models/                    # 数据模型
│   ├── SvgData.cs
│   ├── SvgProcessResult.cs
│   └── ColorChange.cs
├── Core/                      # 核心处理逻辑
│   ├── ColorNormalizer.cs     # 颜色转换器
│   ├── SvgProcessor.cs        # SVG处理引擎
│   └── SvgBatchProcessor.cs   # 批量处理器
├── DataSources/               # 数据源适配器
│   ├── ISvgSource.cs          # 数据源接口
│   ├── FolderSvgSource.cs     # 文件夹数据源
│   ├── SqliteSvgSource.cs     # SQLite数据源
│   └── DmSvgSource.cs         # 达梦数据库数据源
├── UI/                        # 界面层
│   ├── MainForm.cs            # 主窗体
│   └── Program.cs             # 程序入口
├── Test/                      # 测试数据
│   ├── sample1.svg
│   ├── sample2.svg
│   └── test.sql
├── SvgColorNormalizer.csproj  # 项目文件
└── README.md                  # 说明文档
```

## 实现计划

### 1. 创建项目结构
- 创建.NET 10 WinForms项目
- 建立目录结构
- 配置项目文件

### 2. 实现数据模型
- 创建SvgData、SvgProcessResult、ColorChange等模型类

### 3. 实现核心处理逻辑
- 实现ColorNormalizer：颜色格式转换
- 实现SvgProcessor：SVG文件处理
- 实现SvgBatchProcessor：批量处理

### 4. 实现数据源适配器
- 实现ISvgSource接口
- 实现FolderSvgSource：本地文件夹
- 实现SqliteSvgSource：SQLite数据库
- 实现DmSvgSource：达梦数据库

### 5. 实现用户界面
- 创建MainForm主窗体
- 设计工具栏、文件列表、颜色变更详情、预览面板
- 实现加载、处理、导出功能

### 6. 配置项目文件
- 设置.NET 10目标框架
- 添加必要的NuGet包

### 7. 创建测试数据
- 创建示例SVG文件
- 创建SQL测试脚本

### 8. 编译和测试
- 编译项目
- 测试各项功能
- 验证颜色转换正确性

## 技术改进

1. **使用.NET 10特性**：
   - 利用最新的C#语法特性
   - 改进的异步处理
   - 增强的内存管理

2. **性能优化**：
   - 改进批量处理的并行性能
   - 优化大文件处理
   - 减少内存使用

3. **用户体验**：
   - 更现代的UI设计
   - 改进的进度显示
   - 更友好的错误处理

4. **扩展性**：
   - 模块化设计便于添加新功能
   - 支持更多数据源类型
   - 可定制的颜色转换规则

## 预期成果

- 功能完整的SVG颜色标准化工具
- 支持多数据源的批量处理
- 直观的用户界面
- 详细的处理报告
- 基于.NET 10的现代化实现