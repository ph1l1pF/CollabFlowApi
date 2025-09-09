namespace CollabFlowApi;

public class Collaboration
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public Deadline Deadline { get; set; } = Deadline.DefaultDeadline();
    public Fee Fee { get; set; } = new Fee(0, "EUR");
    public Partner Partner { get; set; } = new Partner("", "", "", "", "", "");
    public Script Script { get; set; } = new Script("");
    public string Notes { get; set; } = "";
    public CollabState State { get; set; } = CollabState.FirstTalks;
    public string UserId { get; set; }
}

public class Script
{
    public string Content { get; set; }
    public Script(string content) => Content = content;
}

public class Partner
{
    public string CompanyName { get; set; }
    public string Industry { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string CustomerNumber { get; set; }

    public Partner(string name, string email, string phone, string companyName, string industry, string customerNumber)
    {
        Name = name;
        Email = email;
        Phone = phone;
        CompanyName = companyName;
        Industry = industry;
        CustomerNumber = customerNumber;
    }
}

public class Fee
{
    public double Amount { get; set; }
    public string Currency { get; set; }
    public Fee(double amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
}

public class Deadline
{
    public DateTime Date { get; set; }
    public bool SendNotification { get; set; }
    public int? NotifyDaysBefore { get; set; }

    public Deadline(DateTime date, bool sendNotification, int? notifyDaysBefore = 1)
    {
        Date = date;
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
