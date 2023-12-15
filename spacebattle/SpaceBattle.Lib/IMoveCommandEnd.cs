namespace SpaceBattle.Lib;

public interface IMoveCommandEndable
{
    IUObject Target { get; }
    Dictionary<string, object> RequiredValues { get; }
}
