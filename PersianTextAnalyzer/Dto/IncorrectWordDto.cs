namespace PersianTextAnalyzer.Dto;

public class IncorrectWordDto
{
    public string Word { get; set; }
    public List<string> Suggestion { get; set; }
}