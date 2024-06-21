﻿namespace PettyCashPrototype.Models;

public partial class User: IdentityUser
{
    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Idnumber { get; set; } = null!;

    public int DepartmentId { get; set; }

    public int OfficeId { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual Department Department { get; set; } = null!;

    public virtual Office Office { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Requisition> Applicants { get; set; } = new List<Requisition>();

    [JsonIgnore]
    public virtual ICollection<Requisition> FinanceOfficers { get; set; } = new List<Requisition>();

    [JsonIgnore]
    public virtual ICollection<Requisition> Issuers { get; set; } = new List<Requisition>();

    [JsonIgnore]
    public virtual ICollection<Requisition> Managers { get; set; } = new List<Requisition>();
    
    [JsonIgnore]
    public IList<UserRole>? UserRoles { get; set; }

    public string FullName
    {
        get
        {
            return $"{Firstname} {Lastname}";
        }
    }
}
