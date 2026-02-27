import json
import os

class ArchitectureDiagramGenerator:
    def __init__(self, analysis_file, output_file):
        self.analysis_file = analysis_file
        self.output_file = output_file
        self.analysis_data = {}
    
    def load_analysis_data(self):
        """加载分析数据"""
        with open(self.analysis_file, 'r', encoding='utf-8') as f:
            self.analysis_data = json.load(f)
    
    def generate_dot_file(self):
        """生成Graphviz DOT文件"""
        dot_content = "digraph ArchitectureDiagram {\n"
        dot_content += "    rankdir=TB;\n"
        dot_content += "    node [shape=box, style=filled, fillcolor=lightgreen];\n\n"
        
        # 定义架构层
        layers = {
            "presentation": "表示层",
            "business": "业务逻辑层",
            "data": "数据访问层",
            "core": "核心层"
        }
        
        # 生成架构层节点
        for layer, layer_name in layers.items():
            dot_content += f"    {layer} [label=\"{layer_name}\", shape=rectangle, style=filled, fillcolor=lightyellow, penwidth=2];\n"
        
        # 生成层之间的关系
        layer_order = ["presentation", "business", "data", "core"]
        for i in range(len(layer_order) - 1):
            dot_content += f"    {layer_order[i]} -> {layer_order[i+1]} [arrowhead=normal];\n"
        
        # 生成各层的组件
        for layer, layer_name in layers.items():
            components = self.analysis_data.get('architecture', {}).get(layer, [])
            if components:
                dot_content += f"\n    subgraph cluster_{layer} {{\n"
                dot_content += f"        label=\"{layer_name}组件\";\n"
                dot_content += f"        style=filled;\n"
                dot_content += f"        fillcolor=lightgray;\n"
                
                for component in components:
                    component_name = component.replace('\\', '_').replace('/', '_')
                    dot_content += f"        {component_name} [label=\"{component}\", shape=box, style=filled, fillcolor=lightblue];\n"
                
                dot_content += f"    }}\n"
                
                # 连接组件到对应的层
                for component in components:
                    component_name = component.replace('\\', '_').replace('/', '_')
                    dot_content += f"    {component_name} -> {layer} [style=dotted];\n"
        
        # 生成项目依赖关系
        dot_content += "\n    subgraph cluster_dependencies {\n"
        dot_content += "        label=\"项目依赖\";\n"
        dot_content += "        style=filled;\n"
        dot_content += "        fillcolor=lightgray;\n"
        
        for dep in self.analysis_data.get('dependencies', []):
            project_name = dep['project'].replace(' ', '_')
            dot_content += f"        {project_name} [label=\"{dep['project']}\", shape=box, style=filled, fillcolor=lightpink];\n"
        
        # 生成项目间的依赖关系
        for dep in self.analysis_data.get('dependencies', []):
            project_name = dep['project'].replace(' ', '_')
            for ref in dep.get('project_references', []):
                ref_name = os.path.splitext(os.path.basename(ref))[0].replace(' ', '_')
                dot_content += f"        {project_name} -> {ref_name} [arrowhead=normal];\n"
        
        dot_content += "    }}\n"
        
        dot_content += "}"
        
        # 保存DOT文件
        with open(self.output_file, 'w', encoding='utf-8') as f:
            f.write(dot_content)
        
        print(f"Architecture diagram DOT file generated: {self.output_file}")
    
    def generate_png(self):
        """使用Graphviz生成PNG图片"""
        png_file = self.output_file.replace('.dot', '.png')
        command = f"dot -Tpng {self.output_file} -o {png_file}"
        
        try:
            os.system(command)
            print(f"Architecture diagram PNG file generated: {png_file}")
            return png_file
        except Exception as e:
            print(f"Error generating PNG: {e}")
            return None

if __name__ == "__main__":
    import sys
    if len(sys.argv) != 3:
        print("Usage: python generate_architecture_diagram.py <analysis_file> <output_file>")
        sys.exit(1)
    
    analysis_file = sys.argv[1]
    output_file = sys.argv[2]
    
    generator = ArchitectureDiagramGenerator(analysis_file, output_file)
    generator.load_analysis_data()
    generator.generate_dot_file()
    generator.generate_png()
