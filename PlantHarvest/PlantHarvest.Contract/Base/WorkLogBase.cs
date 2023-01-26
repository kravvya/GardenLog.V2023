namespace PlantHarvest.Contract.Base;

public abstract record WorkLogBase
{
    public string Log { get;  set; }
    public WorkLogEntityEnum RelatedEntity { get;  set; }
    public string RelatedEntityid { get;  set; }
    public DateTime EnteredDateTime { get;  set; }
    public DateTime EventDateTime { get;  set; }
    public WorkLogReasonEnum Reason { get;  set; }
}
