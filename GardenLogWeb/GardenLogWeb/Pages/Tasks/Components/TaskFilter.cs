namespace GardenLogWeb.Pages.Tasks.Components;

public class TaskFilter
{
    private bool _showCompletedTasks = false;
    private Func<bool, Task> _onShowCompletedTasks;

    public List<CheckableEnum> WorkLogReasonEnum { get; set; } = new();
    public bool ShowCompletedTasks
    {
        get
        {
            return _showCompletedTasks;
        }
        set
        {
            _showCompletedTasks = value;
            _onShowCompletedTasks(_showCompletedTasks);
        }
    }

    public TaskFilter(IVerifyService verifyService, Func<bool, Task> OnShowCompletedTasks)
    {
        _onShowCompletedTasks = OnShowCompletedTasks;
        var reasons = verifyService.GetCodeList<WorkLogReasonEnum>(true);
        foreach (var code in reasons)
        {
            WorkLogReasonEnum.Add(new CheckableEnum(code));
        }
    }
}
