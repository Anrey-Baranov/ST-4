using Stateless;

namespace BugPro;

public class Bug
{
    //Состояния бага
    public enum State
    {
        New,           //Новый дефект
        Assigned,      //Назначен
        InProgress,    //В работе
        Fixed,         //Исправлен
        ReadyForTest,  //Готов к тестированию
        Closed,        //Закрыт
        Reopened,      //Переоткрыт
        Rejected,      //Отклонен
        NeedInfo       //Требуется информация
    }

    public enum Trigger
    {
        Assign,        //Назначить
        StartWork,     //Начать работу
        Fix,           //Исправить
        Test,          //Тестировать
        Close,         //Закрыть
        Reopen,        //Переоткрыть
        Reject,        //Отклонить
        NeedMoreInfo   //Запросить информацию
    }

    private readonly StateMachine<State, Trigger> _machine;
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _assignTrigger;

    public Bug(State initialState = State.New)
    {
        _machine = new StateMachine<State, Trigger>(initialState);
        _assignTrigger = _machine.SetTriggerParameters<string>(Trigger.Assign);

        ConfigureMachine();
    }

    private void ConfigureMachine()
    {
        _machine.Configure(State.New)
            .Permit(Trigger.Assign, State.Assigned)
            .Permit(Trigger.Reject, State.Rejected)
            .Permit(Trigger.NeedMoreInfo, State.NeedInfo);

        _machine.Configure(State.Assigned)
            .Permit(Trigger.StartWork, State.InProgress)
            .Permit(Trigger.Reject, State.Rejected);

        _machine.Configure(State.InProgress)
            .Permit(Trigger.Fix, State.Fixed)
            .Permit(Trigger.Reject, State.Rejected);

        _machine.Configure(State.Fixed)
            .Permit(Trigger.Test, State.ReadyForTest);

        _machine.Configure(State.ReadyForTest)
            .Permit(Trigger.Close, State.Closed)
            .Permit(Trigger.Reopen, State.Reopened)
            .Permit(Trigger.NeedMoreInfo, State.NeedInfo);

        _machine.Configure(State.Reopened)
            .Permit(Trigger.Assign, State.Assigned)
            .Permit(Trigger.Reject, State.Rejected);

        _machine.Configure(State.Closed)
            .Permit(Trigger.Reopen, State.Reopened);

        _machine.Configure(State.Rejected)
            .Permit(Trigger.Assign, State.Assigned)
            .Permit(Trigger.Reopen, State.Reopened);

        _machine.Configure(State.NeedInfo)
            .Permit(Trigger.Assign, State.Assigned)
            .Permit(Trigger.Reject, State.Rejected);
    }

    public State CurrentState => _machine.State;

    public void Assign(string developer)
    {
        _machine.Fire(_assignTrigger, developer);
    }

    public void StartWork() => _machine.Fire(Trigger.StartWork);
    public void Fix() => _machine.Fire(Trigger.Fix);
    public void Test() => _machine.Fire(Trigger.Test);
    public void Close() => _machine.Fire(Trigger.Close);
    public void Reopen() => _machine.Fire(Trigger.Reopen);
    public void Reject() => _machine.Fire(Trigger.Reject);
    public void NeedMoreInfo() => _machine.Fire(Trigger.NeedMoreInfo);

    public bool CanAssign => _machine.CanFire(Trigger.Assign);
    public bool CanStartWork => _machine.CanFire(Trigger.StartWork);
    public bool CanFix => _machine.CanFire(Trigger.Fix);
    public bool CanTest => _machine.CanFire(Trigger.Test);
    public bool CanClose => _machine.CanFire(Trigger.Close);
    public bool CanReopen => _machine.CanFire(Trigger.Reopen);
    public bool CanReject => _machine.CanFire(Trigger.Reject);
    public bool CanNeedMoreInfo => _machine.CanFire(Trigger.NeedMoreInfo);
}

public class Program
{
    public static void Main()
    {
        var bug = new Bug();
        Console.WriteLine($"Начальное состояние: {bug.CurrentState}");

        bug.Assign("Иван Иванов");
        Console.WriteLine($"После назначения: {bug.CurrentState}");

        bug.StartWork();
        Console.WriteLine($"После начала работы: {bug.CurrentState}");

        bug.Fix();
        Console.WriteLine($"После исправления: {bug.CurrentState}");

        bug.Test();
        Console.WriteLine($"После тестирования: {bug.CurrentState}");

        bug.Close();
        Console.WriteLine($"После закрытия: {bug.CurrentState}");

        bug.Reopen();
        Console.WriteLine($"После переоткрытия: {bug.CurrentState}");
    }
}