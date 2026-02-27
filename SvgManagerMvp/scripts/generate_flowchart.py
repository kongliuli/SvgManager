import os

class FlowchartGenerator:
    def __init__(self, output_file):
        self.output_file = output_file
    
    def generate_dot_file(self):
        """生成Graphviz DOT文件"""
        dot_content = "digraph Flowchart {\n"
        dot_content += "    rankdir=TB;\n"
        dot_content += "    node [shape=box, style=filled, fillcolor=lightyellow];\n"
        dot_content += "    edge [fontsize=10];\n\n"
        
        # 定义流程图节点
        nodes = {
            "start": "开始",
            "load_data": "加载SVG数据",
            "choose_source": "选择数据源",
            "load_folder": "从文件夹加载",
            "load_db": "从数据库加载",
            "load_api": "从API加载",
            "process_svg": "处理SVG文件",
            "batch_process": "批量处理",
            "single_process": "单个处理",
            "normalize_color": "颜色标准化",
            "save_result": "保存处理结果",
            "save_file": "保存到文件",
            "save_db": "保存到数据库",
            "export_report": "导出报告",
            "end": "结束"
        }
        
        # 生成节点
        for node_id, label in nodes.items():
            dot_content += f"    {node_id} [label=\"{label}\"];\n"
        
        # 生成流程连接
        edges = [
            ("start", "load_data"),
            ("load_data", "choose_source"),
            ("choose_source", "load_folder", "文件夹"),
            ("choose_source", "load_db", "数据库"),
            ("choose_source", "load_api", "API"),
            ("load_folder", "process_svg"),
            ("load_db", "process_svg"),
            ("load_api", "process_svg"),
            ("process_svg", "batch_process", "多个文件"),
            ("process_svg", "single_process", "单个文件"),
            ("batch_process", "normalize_color"),
            ("single_process", "normalize_color"),
            ("normalize_color", "save_result"),
            ("save_result", "save_file", "保存文件"),
            ("save_result", "save_db", "保存数据库"),
            ("save_result", "export_report", "导出报告"),
            ("save_file", "end"),
            ("save_db", "end"),
            ("export_report", "end")
        ]
        
        for edge in edges:
            if len(edge) == 3:
                dot_content += f"    {edge[0]} -> {edge[1]} [label=\"{edge[2]}\"];\n"
            else:
                dot_content += f"    {edge[0]} -> {edge[1]};\n"
        
        dot_content += "}"
        
        # 保存DOT文件
        with open(self.output_file, 'w', encoding='utf-8') as f:
            f.write(dot_content)
        
        print(f"Flowchart DOT file generated: {self.output_file}")
    
    def generate_png(self):
        """使用Graphviz生成PNG图片"""
        png_file = self.output_file.replace('.dot', '.png')
        command = f"dot -Tpng {self.output_file} -o {png_file}"
        
        try:
            os.system(command)
            print(f"Flowchart PNG file generated: {png_file}")
            return png_file
        except Exception as e:
            print(f"Error generating PNG: {e}")
            return None

if __name__ == "__main__":
    import sys
    if len(sys.argv) != 2:
        print("Usage: python generate_flowchart.py <output_file>")
        sys.exit(1)
    
    output_file = sys.argv[1]
    
    generator = FlowchartGenerator(output_file)
    generator.generate_dot_file()
    generator.generate_png()
