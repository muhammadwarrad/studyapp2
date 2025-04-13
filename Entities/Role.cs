using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace StudyApp.Entities;

public class Role : IdentityRole<int>
{
    public List<UserRole> Users { get; set; } = new();
}