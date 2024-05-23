namespace SpaceBattle.Lib;

public class PositionsEnumerator : IEnumerator<object>
{
    readonly IList<Vector> _uObjectPositions;
    int _positionIndex;

    public PositionsEnumerator(IList<Vector> uObjectPositions) => _uObjectPositions = uObjectPositions;

    public object Current => _uObjectPositions[_positionIndex];

    public bool MoveNext() => ++_positionIndex < _uObjectPositions.Count;

    public void Reset() => _positionIndex = 0;

    public void Dispose() => GC.SuppressFinalize(this);
}
