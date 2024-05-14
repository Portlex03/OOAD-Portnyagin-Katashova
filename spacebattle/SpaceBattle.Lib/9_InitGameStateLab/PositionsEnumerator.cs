namespace SpaceBattle.Lib;
using Hwdtech;

public class PositionsEnumerator : IEnumerator<object>
{
    readonly IList<Vector> _positions;
    int _positionIndex;

    public PositionsEnumerator() => _positions = IoC.Resolve<List<Vector>>("UObjectsPositions");

    public object Current => _positions[_positionIndex];

    public bool MoveNext() => ++_positionIndex < _positions.Count;

    public void Reset() => _positionIndex = 0;

    public void Dispose() => throw new NotImplementedException();
}
