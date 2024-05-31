using Hwdtech;
using Scriban;

namespace SpaceBattle.Lib;

public class AdapterBuilder
{
    readonly Type _targetType;
    readonly Type _newTargetType;

    public AdapterBuilder(Type targetType, Type newTargetType)
    {
        _targetType = targetType;
        _newTargetType = newTargetType;
    }

    public string Build()
    {
        var template = Template.Parse(IoC.Resolve<string>("Template"));
        return template.Render(new
        {
            target_type = _targetType.Name,
            new_target_type = _newTargetType.Name,
            properties = _newTargetType.GetProperties().ToList()
        });
    }
}
