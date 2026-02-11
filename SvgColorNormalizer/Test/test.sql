-- 创建测试数据库
CREATE TABLE svg_assets (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    svg_content TEXT NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- 插入测试数据
INSERT INTO svg_assets (name, svg_content) VALUES
('示例图形1', '<svg width="100" height="100" xmlns="http://www.w3.org/2000/svg">
    <rect x="10" y="10" width="80" height="80" fill="#FF0000" stroke="#00FF00"/>
</svg>'),

('示例图形2', '<svg width="150" height="100" xmlns="http://www.w3.org/2000/svg">
    <circle cx="50" cy="50" r="40" fill="rgb(255, 128, 0)" stroke="#0000FF"/>
    <rect x="100" y="20" width="40" height="60" fill="#FF000080"/>
</svg>'),

('复杂渐变', '<svg width="200" height="100" xmlns="http://www.w3.org/2000/svg">
    <defs>
        <linearGradient id="grad">
            <stop offset="0%" stop-color="#FF0000"/>
            <stop offset="100%" stop-color="#0000FF"/>
        </linearGradient>
    </defs>
    <rect x="10" y="10" width="180" height="80" fill="url(#grad)"/>
</svg>');