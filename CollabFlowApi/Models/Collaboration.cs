using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CollabFlowApi;

[Table("collaborations")]
public class Collaboration
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string Title { get; set; } = string.Empty;

    public Deadline Deadline { get; set; } = Deadline.DefaultDeadline();
    public Fee Fee { get; set; } = new Fee(0, "EUR");
    public Partner Partner { get; set; } = new Partner("", "", "", "", "", "");
    public Script Script { get; set; } = new Script("");
    public string Notes { get; set; } = "";
    public CollabState State { get; set; } = CollabState.FirstTalks;

    [Required]
    public string UserId { get; set; } = default!;
}

[Owned]
public class Script
{
    public string Content { get; set; } = string.Empty;
    public Script() { }
    public Script(string content) => Content = content;
}

[Owned]
public class Partner
{
    public string CompanyName { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string CustomerNumber { get; set; } = string.Empty;

    public Partner() { }
    public Partner(string name, string email, string phone,
                   string companyName, string industry, string customerNumber)
    {
        Name = name;
        Email = email;
        Phone = phone;
        CompanyName = companyName;
        Industry = industry;
        CustomerNumber = customerNumber;
    }
}

[Owned]
public class Fee
{
    public double Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public Fee() { }
    public Fee(double amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
}

[Owned]
public class Deadline
{
    public DateTime Date { get; set; }
    public bool SendNotification { get; set; }
    public int? NotifyDaysBefore { get; set; }
    public Deadline(DateTime date, bool sendNotification, int? notifyDaysBefore = 1)
    {
        Date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        SendNotification = sendNotification;
        NotifyDaysBefore = notifyDaysBefore;
    }

    public static Deadline DefaultDeadline() =>
        new Deadline(DateTime.UtcNow, true, 1);
}

public enum CollabState
{
    FirstTalks,
    ContractToSign,
    ScriptToProduce,
    InProduction,
    ContentEditing,
    ContentFeedback,
    Finished
}
