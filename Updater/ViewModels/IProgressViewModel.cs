namespace Updater.ViewModels
{
    public interface IProgressViewModel
    {
        string Label1 { get; set; }
        string Label2 { get; set; }
        string Label3 { get; set; }
        double Progress1 { get; set; }
        double Progress2 { get; set; }
    }
}