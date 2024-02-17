namespace SpaceBattle.Lib;

public interface IUObject
{
    object GetProperty(string key);
    void SetProperty(string key, object value);
}
