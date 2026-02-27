import os
import re
import json
from typing import Dict, List, Any

class ProjectAnalyzer:
    def __init__(self, project_path):
        self.project_path = project_path
        self.project_structure = {}
        self.classes = []
        self.dependencies = []
        self.architecture = {}
    
    def analyze(self):
        """分析项目结构"""
        self._analyze_directory_structure()
        self._analyze_csharp_files()
        self._analyze_dependencies()
        self._analyze_architecture()
        return {
            "project_structure": self.project_structure,
            "classes": self.classes,
            "dependencies": self.dependencies,
            "architecture": self.architecture
        }
    
    def _analyze_directory_structure(self):
        """分析目录结构"""
        for root, dirs, files in os.walk(self.project_path):
            # 排除一些不需要分析的目录
            dirs[:] = [d for d in dirs if d not in ['.git', 'bin', 'obj', 'Properties', 'node_modules']]
            
            relative_path = os.path.relpath(root, self.project_path)
            if relative_path == '.':
                relative_path = ''
            
            self.project_structure[relative_path] = {
                "directories": dirs,
                "files": files
            }
    
    def _analyze_csharp_files(self):
        """分析C#文件，提取类信息"""
        for root, dirs, files in os.walk(self.project_path):
            # 排除一些不需要分析的目录
            dirs[:] = [d for d in dirs if d not in ['.git', 'bin', 'obj', 'Properties', 'node_modules']]
            
            for file in files:
                if file.endswith('.cs'):
                    file_path = os.path.join(root, file)
                    relative_path = os.path.relpath(file_path, self.project_path)
                    self._parse_csharp_file(file_path, relative_path)
    
    def _parse_csharp_file(self, file_path, relative_path):
        """解析C#文件，提取类信息"""
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()
        except:
            return
        
        # 提取命名空间
        namespace_match = re.search(r'namespace\s+([\w\.]+)', content)
        namespace = namespace_match.group(1) if namespace_match else ''
        
        # 提取类定义
        class_pattern = r'(?:public|private|protected)\s+class\s+([\w]+)\s*(?:\s*:\s*([\w\s,]+))?'
        class_matches = re.findall(class_pattern, content)
        
        for class_name, base_classes in class_matches:
            base_classes_list = [bc.strip() for bc in base_classes.split(',')] if base_classes else []
            
            # 提取方法
            method_pattern = r'(?:public|private|protected)\s+(?:static\s+)?(?:async\s+)?[\w<>]+\s+([\w]+)\s*\([^)]*\)'
            methods = re.findall(method_pattern, content)
            
            self.classes.append({
                "name": class_name,
                "namespace": namespace,
                "file": relative_path,
                "base_classes": base_classes_list,
                "methods": methods
            })
    
    def _analyze_dependencies(self):
        """分析项目依赖关系"""
        # 分析csproj文件
        for root, dirs, files in os.walk(self.project_path):
            for file in files:
                if file.endswith('.csproj'):
                    file_path = os.path.join(root, file)
                    relative_path = os.path.relpath(file_path, self.project_path)
                    self._parse_csproj_file(file_path, relative_path)
    
    def _parse_csproj_file(self, file_path, relative_path):
        """解析csproj文件，提取依赖关系"""
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()
        except:
            return
        
        # 提取项目名称
        project_name_match = re.search(r'<ProjectName>([^<]+)</ProjectName>', content)
        project_name = project_name_match.group(1) if project_name_match else os.path.splitext(os.path.basename(file_path))[0]
        
        # 提取项目引用
        project_reference_pattern = r'<ProjectReference\s+Include="([^"]+)"/>'
        project_references = re.findall(project_reference_pattern, content)
        
        # 提取包引用
        package_reference_pattern = r'<PackageReference\s+Include="([^"]+)"\s+Version="([^"]+)"/>'
        package_references = re.findall(package_reference_pattern, content)
        
        self.dependencies.append({
            "project": project_name,
            "file": relative_path,
            "project_references": project_references,
            "package_references": package_references
        })
    
    def _analyze_architecture(self):
        """分析项目架构"""
        # 基于目录结构和文件类型分析架构
        layers = {
            "presentation": [],  # UI层
            "business": [],      # 业务逻辑层
            "data": [],          # 数据访问层
            "core": []           # 核心层
        }
        
        for root, dirs, files in os.walk(self.project_path):
            relative_path = os.path.relpath(root, self.project_path)
            
            # 根据目录名称判断层级
            if 'UI' in dirs or 'Controllers' in dirs or 'Forms' in dirs:
                layers["presentation"].append(relative_path)
            elif 'Core' in dirs or 'Business' in dirs or 'Services' in dirs:
                layers["business"].append(relative_path)
            elif 'Data' in dirs or 'Repositories' in dirs:
                layers["data"].append(relative_path)
            elif 'Models' in dirs or 'Entities' in dirs:
                layers["core"].append(relative_path)
        
        self.architecture = layers

if __name__ == "__main__":
    import sys
    if len(sys.argv) != 2:
        print("Usage: python analyze_project.py <project_path>")
        sys.exit(1)
    
    project_path = sys.argv[1]
    analyzer = ProjectAnalyzer(project_path)
    result = analyzer.analyze()
    
    # 保存分析结果
    with open('project_analysis.json', 'w', encoding='utf-8') as f:
        json.dump(result, f, indent=2, ensure_ascii=False)
    
    print("Project analysis completed. Results saved to project_analysis.json")
