namespace SpaceBattle.Lib;

public class FuelEnumerator : IEnumerator<object>
{
    readonly IList<double> _uObjectsFuelVolumes;
    int _fuelIndex;

    public FuelEnumerator(IList<double> uObjectsFuelVolumes) => _uObjectsFuelVolumes = uObjectsFuelVolumes;

    public object Current => _uObjectsFuelVolumes[_fuelIndex];

    public bool MoveNext() => ++_fuelIndex < _uObjectsFuelVolumes.Count;

    public void Reset() => _fuelIndex = 0;

    public void Dispose() => GC.SuppressFinalize(this);
}
