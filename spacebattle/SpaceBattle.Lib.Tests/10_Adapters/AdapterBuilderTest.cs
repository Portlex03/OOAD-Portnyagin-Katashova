using Hwdtech.Ioc;
using Hwdtech;
using Moq;

namespace SpaceBattle.Lib.Test;

public class AdapterBuilderTest
{
    readonly string _template;
    public AdapterBuilderTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        _template =
@"public class {{new_target_type}}Adapter : {{new_target_type}}
{
    readonly private {{target_type}} _obj;
    public {{new_target_type}}Adapter({{target_type}} obj) => _obj = obj;
{{for property in (properties)}}
    public {{property.property_type.name}} {{property.name}}
    {
{{if property.can_read}}
        get => IoC.Resolve<{{property.property_type.name}}>(""Game.Get.Property"", ""{{property.name}}"", _obj);
{{end}}
{{if property.can_write}}
        set => IoC.Resolve<ICommand>(""Game.Set.Property"", ""{{property.name}}"", _obj, value).Execute();
{{end}}
    }
{{end}}
}";
    }

    [Fact]
    public void BuildString()
    {
        var expected =
@"public class IMovableAdapter : IMovable
{
    readonly private IUObject _obj;
    public IMovableAdapter(IUObject obj) => _obj = obj;

    public Vector Position
    {

        get => IoC.Resolve<Vector>(""Game.Get.Property"", ""Position"", _obj);


        set => IoC.Resolve<ICommand>(""Game.Set.Property"", ""Position"", _obj, value).Execute();

    }

    public Vector Velocity
    {

        get => IoC.Resolve<Vector>(""Game.Get.Property"", ""Velocity"", _obj);


    }

}";
        var getTemplateCmd = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Template",
            (object[] args) => getTemplateCmd.Object.Execute(args)
        ).Execute();

        getTemplateCmd.Setup(cmd => cmd.Execute()).Returns(_template);

        var builder = new AdapterBuilder(
            targetType: typeof(IUObject),
            newTargetType: typeof(IMovable)
        );
        var result = builder.Build();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Impossible_To_Get_Template_Test()
    {
        var getTemplateCmd = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Template",
            (object[] args) => getTemplateCmd.Object.Execute(args)
        ).Execute();

        getTemplateCmd.Setup(cmd => cmd.Execute()).Throws<Exception>();

        var builder = new AdapterBuilder(
            targetType: typeof(IUObject),
            newTargetType: typeof(IMovable)
        );
        Assert.Throws<Exception>(builder.Build);
    }
}
