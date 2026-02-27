import json
import os

class ClassDiagramGenerator:
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
        dot_content = "digraph ClassDiagram {\n"
        dot_content += "    rankdir=TB;\n"
        dot_content += "    node [shape=record, style=filled, fillcolor=lightblue];\n\n"
        
        # 生成类节点
        for cls in self.analysis_data.get('classes', []):
            class_name = cls['name']
            namespace = cls['namespace']
            methods = cls['methods']
            
            # 生成方法字符串
            methods_str = "|\n"
            for method in methods[:10]:  # 限制显示的方法数量
                methods_str += "{" + method + "}" + "|\n"
            
            # 生成类节点
            label = "{" + namespace + "\n" + class_name + methods_str + "}"
            dot_content += "    " + class_name + " [label=\"" + label + "\"];\n"
        
        # 生成继承关系
        for cls in self.analysis_data.get('classes', []):
            class_name = cls['name']
            base_classes = cls['base_classes']
            
            for base_class in base_classes:
                # 只处理在分析结果中存在的基类
                if any(c['name'] == base_class for c in self.analysis_data.get('classes', [])):
                    dot_content += f"    {base_class} -> {class_name} [arrowhead=empty];\n"
        
        dot_content += "}"
        
        # 保存DOT文件
        with open(self.output_file, 'w', encoding='utf-8') as f:
            f.write(dot_content)
        
        print(f"Class diagram DOT file generated: {self.output_file}")
    
    def generate_png(self):
        """使用Graphviz生成PNG图片"""
        png_file = self.output_file.replace('.dot', '.png')
        command = f"dot -Tpng {self.output_file} -o {png_file}"
        
        try:
            os.system(command)
            print(f"Class diagram PNG file generated: {png_file}")
            return png_file
        except Exception as e:
            print(f"Error generating PNG: {e}")
            return None

if __name__ == "__main__":
    import sys
    if len(sys.argv) != 3:
        print("Usage: python generate_class_diagram.py <analysis_file> <output_file>")
        sys.exit(1)
    
    analysis_file = sys.argv[1]
    output_file = sys.argv[2]
    
    generator = ClassDiagramGenerator(analysis_file, output_file)
    generator.load_analysis_data()
    generator.generate_dot_file()
    generator.generate_png()
