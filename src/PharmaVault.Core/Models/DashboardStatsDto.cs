namespace PharmaVault.Core.Models;

public class DashboardStatsDto
{
    public int TotalStock { get; set; }
    public int ExpiredStock { get; set; }
    public int ExpiringSoonStock { get; set; } 
    public int GoodStock { get; set; }
}