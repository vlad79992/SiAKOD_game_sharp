using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;

internal class GameLogic
{
    private RenderWindow window;
    private Camera camera;
    private Controls controls;
    private Render render;
    
    private Dictionary<Line, bool> lines = new Dictionary<Line, bool>();
    private bool isBlueTurn = true;
    private bool vsComputer = true;
    
    public GameLogic(RenderWindow window, Camera camera, Controls controls, Render render, bool vsComputer = true)
    {
        this.window = window;
        this.camera = camera;
        this.controls = controls;
        this.render = render;
        this.vsComputer = vsComputer;
        
        window.MouseButtonPressed += OnMouseButtonPressed;
    }
    
    private void OnMouseButtonPressed(object? sender, MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Left)
        {
            // В режиме против компьютера ходит только синий
            if (vsComputer && !isBlueTurn)
                return;
                
            // В режиме двух игроков ходят по очереди оба
            var worldCoords = controls.GetWorldCoords((e.X, e.Y), window.Size);
            if (TryAddLine(worldCoords, isBlueTurn))
            {
                // Если играем против компьютера и синий поставил линию
                if (vsComputer && isBlueTurn) // Исправлено: проверяем isBlueTurn
                {
                    // Передаем ход компьютеру
                    isBlueTurn = false;
                    ComputerMove();
                }
            }
        }
    }
    
    private bool TryAddLine((double X, double Y) worldCoords, bool isBlue)
    {
        double distX = Math.Abs(worldCoords.X - Math.Round(worldCoords.X));
        double distY = Math.Abs(worldCoords.Y - Math.Round(worldCoords.Y));
        
        long lowerX = (long)Math.Floor(worldCoords.X);
        long upperX = (long)Math.Ceiling(worldCoords.X);
        long lowerY = (long)Math.Floor(worldCoords.Y);
        long upperY = (long)Math.Ceiling(worldCoords.Y);
        
        Line? newLine = null;
        
        if (distX < distY && distX < 0.1f)
        {
            long nearestX = (long)Math.Round(worldCoords.X);
            newLine = new Line((nearestX, lowerY), (nearestX, upperY));
        }
        else if (distY < distX && distY < 0.1f)
        {
            long nearestY = (long)Math.Round(worldCoords.Y);
            newLine = new Line((lowerX, nearestY), (upperX, nearestY));
        }
        
        if (newLine != null && !lines.ContainsKey(newLine.Value))
        {
            lines[newLine.Value] = isBlue;
            render.AddLine(newLine.Value, isBlue);
            
            // В режиме двух игроков меняем ход, в режиме против компьютера ход меняется в ComputerMove
            if (!vsComputer)
            {
                isBlueTurn = !isBlueTurn;
            }
            
            Console.WriteLine($"Line added: {newLine.Value.Point1} -> {newLine.Value.Point2}, Color: {(isBlue ? "Blue" : "Red")}");
            return true;
        }
        return false;
    }
    
    private void ComputerMove()
    {
        Console.WriteLine("Computer thinking...");
        
        Line? criticalBlock = FindCriticalBlock();
        if (criticalBlock != null && !lines.ContainsKey(criticalBlock.Value))
        {
            TryAddLineFromComputer(criticalBlock.Value);
            Console.WriteLine($"Critical block! Computer added: {criticalBlock.Value.Point1} -> {criticalBlock.Value.Point2}");
            return;
        }
        
        Line? lastBlueLine = null;
        foreach (var line in lines.Reverse())
        {
            if (line.Value)
            {
                lastBlueLine = line.Key;
                break;
            }
        }
        
        if (lastBlueLine == null) 
        {
            MakeRandomMove();
            return;
        }
        
        List<Line> possibleMoves = new List<Line>();
        
        if (lastBlueLine.Value.Point1.Item1 == lastBlueLine.Value.Point2.Item1)
        {
            possibleMoves.AddRange(GetHorizontalBlockingLines(lastBlueLine.Value));
        }
        else
        {
            possibleMoves.AddRange(GetVerticalBlockingLines(lastBlueLine.Value));
        }
        
        var freeMoves = possibleMoves.Where(line => !lines.ContainsKey(line)).ToList();
        
        if (freeMoves.Count > 0)
        {
            Line? blockingMove = FindTopLeftBlockingMove(freeMoves, lastBlueLine.Value);
        
            if (blockingMove != null)
            {
                TryAddLineFromComputer(blockingMove.Value);
                return;
            }
            
            var random = new Random();
            var computerLine = freeMoves[random.Next(freeMoves.Count)];
            TryAddLineFromComputer(computerLine);
        }
        else
        {
            MakeRandomMove();
        }
    }

    private Line? FindTopLeftBlockingMove(List<Line> freeMoves, Line lastBlueLine)
    {
        long blueX1 = lastBlueLine.Point1.Item1;
        long blueY1 = lastBlueLine.Point1.Item2;
        long blueX2 = lastBlueLine.Point2.Item1;
        long blueY2 = lastBlueLine.Point2.Item2;
        
        if (blueX1 == blueX2)
        {
            long x = blueX1;
            long topY = Math.Min(blueY1, blueY2);
            
            var topLeftBlock = freeMoves.FirstOrDefault(line => 
                line.Point1.Item1 == x - 1 && line.Point1.Item2 == topY && 
                line.Point2.Item1 == x && line.Point2.Item2 == topY);
            
            if (topLeftBlock.Point1 != (0, 0) || topLeftBlock.Point2 != (0, 0))
                return topLeftBlock;
                
            var topBlocks = freeMoves.Where(line => 
                (line.Point1.Item2 == topY || line.Point2.Item2 == topY)).ToList();
                
            if (topBlocks.Count > 0)
                return topBlocks[0];
        }
        else
        {
            long y = blueY1;
            long leftX = Math.Min(blueX1, blueX2);
            
            var topLeftBlock = freeMoves.FirstOrDefault(line => 
                line.Point1.Item1 == leftX && line.Point1.Item2 == y - 1 && 
                line.Point2.Item1 == leftX && line.Point2.Item2 == y);
            
            if (topLeftBlock.Point1 != (0, 0) || topLeftBlock.Point2 != (0, 0))
                return topLeftBlock;
                
            var leftBlocks = freeMoves.Where(line => 
                (line.Point1.Item1 == leftX || line.Point2.Item1 == leftX)).ToList();
                
            if (leftBlocks.Count > 0)
                return leftBlocks[0];
        }
        
        return null;
    }

    private List<Line> GetHorizontalBlockingLines(Line verticalBlueLine)
    {
        var moves = new List<Line>();
        long x = verticalBlueLine.Point1.Item1;
        long y1 = verticalBlueLine.Point1.Item2;
        long y2 = verticalBlueLine.Point2.Item2;
        
        long topY = Math.Min(y1, y2);
        long bottomY = Math.Max(y1, y2);
        
        moves.Add(new Line((x - 1, topY), (x, topY)));
        moves.Add(new Line((x, topY), (x + 1, topY)));
        moves.Add(new Line((x - 1, bottomY), (x, bottomY)));
        moves.Add(new Line((x, bottomY), (x + 1, bottomY)));
        
        return moves;
    }

    private List<Line> GetVerticalBlockingLines(Line horizontalBlueLine)
    {
        var moves = new List<Line>();
        long y = horizontalBlueLine.Point1.Item2;
        long x1 = horizontalBlueLine.Point1.Item1;
        long x2 = horizontalBlueLine.Point2.Item1;
        
        long leftX = Math.Min(x1, x2);
        long rightX = Math.Max(x1, x2);
        
        moves.Add(new Line((leftX, y - 1), (leftX, y)));
        moves.Add(new Line((rightX, y - 1), (rightX, y)));
        moves.Add(new Line((leftX, y), (leftX, y + 1)));
        moves.Add(new Line((rightX, y), (rightX, y + 1)));
        
        return moves;
    }

    private Line? FindCriticalBlock()
    {
        var blueLines = lines.Where(l => l.Value).Select(l => l.Key).ToList();
        
        foreach (var blueLine in blueLines)
        {
            var potentialClosure = FindPotentialClosure(blueLine);
            if (potentialClosure != null && !lines.ContainsKey(potentialClosure.Value))
            {
                return potentialClosure;
            }
        }
        
        return null;
    }

    private Line? FindPotentialClosure(Line blueLine)
    {
        if (blueLine.Point1.Item1 == blueLine.Point2.Item1)
        {
            long x = blueLine.Point1.Item1;
            long y1 = blueLine.Point1.Item2;
            long y2 = blueLine.Point2.Item2;
            
            if (IsBlueLine(new Line((x-1, y1), (x, y1))) && 
                IsBlueLine(new Line((x-1, y2), (x, y2))) &&
                !lines.ContainsKey(new Line((x-1, y1), (x-1, y2))))
            {
                return new Line((x-1, y1), (x-1, y2));
            }
            
            if (IsBlueLine(new Line((x, y1), (x+1, y1))) && 
                IsBlueLine(new Line((x, y2), (x+1, y2))) &&
                !lines.ContainsKey(new Line((x+1, y1), (x+1, y2))))
            {
                return new Line((x+1, y1), (x+1, y2));
            }
        }
        else
        {
            long y = blueLine.Point1.Item2;
            long x1 = blueLine.Point1.Item1;
            long x2 = blueLine.Point2.Item1;
            
            if (IsBlueLine(new Line((x1, y-1), (x1, y))) && 
                IsBlueLine(new Line((x2, y-1), (x2, y))) &&
                !lines.ContainsKey(new Line((x1, y-1), (x2, y-1))))
            {
                return new Line((x1, y-1), (x2, y-1));
            }
            
            if (IsBlueLine(new Line((x1, y), (x1, y+1))) && 
                IsBlueLine(new Line((x2, y), (x2, y+1))) &&
                !lines.ContainsKey(new Line((x1, y+1), (x2, y+1))))
            {
                return new Line((x1, y+1), (x2, y+1));
            }
        }
        
        return null;
    }

    private bool IsBlueLine(Line line)
    {
        return lines.ContainsKey(line) && lines[line];
    }

    private void MakeRandomMove()
    {
        var visiblePoints = camera.VisiblePoints.ToArray();
        var random = new Random();
        var possibleMoves = new List<Line>();
        
        foreach (var point in visiblePoints)
        {
            var rightLine = new Line(point, (point.X + 1, point.Y));
            if (!lines.ContainsKey(rightLine))
                possibleMoves.Add(rightLine);
                
            var downLine = new Line(point, (point.X, point.Y + 1));
            if (!lines.ContainsKey(downLine))
                possibleMoves.Add(downLine);
        }
        
        if (possibleMoves.Count > 0)
        {
            var randomLine = possibleMoves[random.Next(possibleMoves.Count)];
            TryAddLineFromComputer(randomLine);
        }
        else
        {
            // Если совсем нет ходов, передаем ход синему
            isBlueTurn = true;
            Console.WriteLine("Computer has no moves, passing turn to Blue");
        }
    }
    
    private void TryAddLineFromComputer(Line line)
    {
        lines[line] = false;
        render.AddLine(line, false);
        isBlueTurn = true; // Возвращаем ход синему игроку
        
        Console.WriteLine($"Computer added: {line.Point1} -> {line.Point2}, Color: Red");
    }
    
    public bool CheckForBlueWin()
    {
        var graph = new Dictionary<(long, long), List<(long, long)>>();
        
        foreach (var line in lines)
        {
            if (line.Value)
            {
                var point1 = line.Key.Point1;
                var point2 = line.Key.Point2;
                
                if (!graph.ContainsKey(point1))
                    graph[point1] = new List<(long, long)>();
                if (!graph.ContainsKey(point2))
                    graph[point2] = new List<(long, long)>();
                    
                graph[point1].Add(point2);
                graph[point2].Add(point1);
            }
        }
        
        return HasCycle(graph);
    }
    
    private bool HasCycle(Dictionary<(long, long), List<(long, long)>> graph)
    {
        var visited = new HashSet<(long, long)>();
        
        foreach (var vertex in graph.Keys)
        {
            if (!visited.Contains(vertex))
            {
                if (HasCycleDFS(vertex, (-1, -1), graph, visited, new HashSet<(long, long)>()))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    private bool HasCycleDFS((long, long) current, (long, long) parent, 
                           Dictionary<(long, long), List<(long, long)>> graph,
                           HashSet<(long, long)> visited, HashSet<(long, long)> recursionStack)
    {
        visited.Add(current);
        recursionStack.Add(current);
        
        if (graph.ContainsKey(current))
        {
            foreach (var neighbor in graph[current])
            {
                if (!visited.Contains(neighbor))
                {
                    if (HasCycleDFS(neighbor, current, graph, visited, recursionStack))
                        return true;
                }
                else if (neighbor != parent && recursionStack.Contains(neighbor))
                {
                    return true;
                }
            }
        }
        
        recursionStack.Remove(current);
        return false;
    }
    
    public void PrintAllLines()
    {
        Console.WriteLine("All lines:");
        foreach (var line in lines)
        {
            Console.WriteLine($"{line.Key.Point1} -> {line.Key.Point2} : {(line.Value ? "Blue" : "Red")}");
        }
    }
    
    public int GetBlueLinesCount() => lines.Count(l => l.Value);
    public int GetRedLinesCount() => lines.Count(l => !l.Value);
    
    public void SetVsComputer(bool vsComputerMode)
    {
        this.vsComputer = vsComputerMode;
        // При переключении режима сбрасываем очередь хода
        isBlueTurn = true;
        Console.WriteLine($"Game mode changed to: {(vsComputerMode ? "VS Computer" : "Two Players")}");
    }
    
    public bool GetVsComputer() => vsComputer;
}