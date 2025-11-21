using System;
// вынес line в отдельный файл
public struct Line : IEquatable<Line>
{
    public (long X, long Y) Point1 { get; }
    public (long X, long Y) Point2 { get; }
    
    public Line((long, long) point1, (long, long) point2)
    {
        if (point1.CompareTo(point2) > 0)
        {
            Point1 = point2;
            Point2 = point1;
        }
        else
        {
            Point1 = point1;
            Point2 = point2;
        }
    }
    
    public bool Equals(Line other)
    {
        return Point1.Equals(other.Point1) && Point2.Equals(other.Point2);
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Line line && Equals(line);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Point1, Point2);
    }
    
    public static bool operator ==(Line left, Line right)
    {
        return left.Equals(right);
    }
    
    public static bool operator !=(Line left, Line right)
    {
        return !left.Equals(right);
    }
}

public static class TupleExtensions
{
    public static int CompareTo(this (long, long) tuple1, (long, long) tuple2)
    {
        int result = tuple1.Item1.CompareTo(tuple2.Item1);
        if (result == 0)
            result = tuple1.Item2.CompareTo(tuple2.Item2);
        return result;
    }
}