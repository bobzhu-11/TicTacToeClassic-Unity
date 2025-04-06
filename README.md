# 井字棋小游戏-Tic-Tac-Toe Classic

<img src="https://raw.githubusercontent.com/bobzhu-11/TicTacToeClassic-Unity/main/screenshot.png" alt="井字棋游戏截图" width="400" />


## 在线版本（itch-Unity webGL build）
[Play Tic-Tac-Toe Classic on itch.io](https://bbzhu.itch.io/tictactoeclassic)

## 项目简介
这是一个基于 Unity 引擎开发的井字棋游戏项目，支持人机对战（AI 对战）以及双人对战模式。旨在制作一个简单易用、易于扩展的井字棋游戏示例。

## 游戏玩法
1. **开始游戏**:
   - 在单人模式中，选择是否要使用X或O进行游戏
   - 在双人模式中，游戏立即开始，X先手
   
2. **落子**:
   - 点击任何空白格子放置你的标记（X或O）
   - 玩家轮流落子，直到有人获胜或棋盘填满

3. **获胜条件**:
   - 在水平、垂直或对角线上连成三个相同标记
   - 获胜组合会被高亮显示
   - 平局情况下，所有格子会变暗

4. **重新开始**:
   - 随时点击重新开始按钮开始新游戏

## 技术实现
游戏使用Unity和C#构建，主要组件包括：

- **GameController**：管理核心游戏逻辑、回合处理和胜利条件检查
- **AIPlayer**：实现不同难度的AI对手算法：
  - 简单模式采用随机选择
  - 中等模式采用策略性阻挡和获胜
  - 困难模式采用极小极大算法
- **Cell**：处理单个网格单元和玩家交互
- **SoundManager**：控制所有游戏音效和静音功能

## 构建信息
- 使用Unity构建
- 在itch.io上提供WebGL版本
- 完全兼容桌面和移动浏览器

## 许可证
- 本项目采用 MIT 许可证 开源，任何人均可自由使用、修改和分发代码（请在分发时附上原始许可证信息）。

## 更新日志
- v1.0 初始版本

- 完成基本的井字棋游戏逻辑和 UI
- 集成音效管理
- 实现 AI 不同难度模式，包括随机、中等策略及极小极大算法

## 参考
- https://en.wikipedia.org/wiki/Minimax
