namespace SpaceBattle.Lib;

using Hwdtech;

public class SetupGameStrategy : IStrategy
{
    public object Invoke(params object[] args)
    {
        object scope = IoC.Resolve<object>("SpaceBattle.GetScope", args[0]);
        object oldScope = IoC.Resolve<object>("Scopes.Current");

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Command.Get.StartMove", (object[] anyArgs) =>
            new MoveCommandStart(IoC.Resolve<IMoveCommandStartable>("SpaceBattle.Get.MoveCommandStartable", anyArgs[0], anyArgs[1]))
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Command.Get.StopMove", (object[] anyArgs) =>
            new EndMoveCommand(IoC.Resolve<IMoveCommandEndable>("SpaceBattle.Get.MoveCommandEndable", anyArgs[0], anyArgs[1]))
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Command.Get.Rotate", (object[] anyArgs) =>
            new RotateCommand(IoC.Resolve<IRotatable>("SpaceBattle.Get.Rotatable", anyArgs[0], anyArgs[1]))
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Command.Get.Shot", (object[] anyArgs) =>
            new ShotCommand(IoC.Resolve<IShotable>("SpaceBattle.Get.Shotable", anyArgs[0], anyArgs[1]))
        ).Execute();


        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.StartMove", (object[] anyArgs) =>
        {
            IoC.Resolve<ICommand>("Command.Get.StartMove", anyArgs[0], anyArgs[1]).Execute();
            return new object();
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.StopMove", (object[] anyArgs) =>
        {
            IoC.Resolve<ICommand>("Command.Get.StopMove", anyArgs[0], anyArgs[1]).Execute();
            return new object();
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Rotate", (object[] anyArgs) =>
        {
            IoC.Resolve<ICommand>("Command.Get.Rotate", anyArgs[0], anyArgs[1]).Execute();
            return new object();
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.Shot", (object[] anyArgs) =>
        {
            IoC.Resolve<ICommand>("Command.Get.Shot", anyArgs[0], anyArgs[1]).Execute();
            return new object();
        }).Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        return new object();
    }
}
