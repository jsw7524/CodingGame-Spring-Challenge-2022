public class RotationMoveCommand : ICommand
{
    public int X { get; set; }
    public int Y { get; set; }
    public double _phase;
    static double _offset;
    private int _radius = 800 - 100;
    public RotationMoveCommand(int x, int y, int total, int sn)
    {
        _offset += 0.1;
        _phase = (Math.PI / total);
        X = x + (int)(_radius * Math.Cos(_phase * sn + _offset));
        Y = y + (int)(_radius * Math.Sin(_phase * sn + _offset)); ;
    }
    public string Execute()
    {
        return $"MOVE {X} {Y}";
    }
}
