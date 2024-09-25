using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class MstUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public decimal? Balance { get; set; }

    public List<MstLoans> MstLoans { get; set; } = new List<MstLoans>();
    public List<TrnFunding> TrnFunding { get; set; } = new List<TrnFunding>();
}
