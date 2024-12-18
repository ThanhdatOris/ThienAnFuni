using System.Collections.Generic;
using ThienAnFuni.Models;

public class DashboardViewModel
{
    public List<SalesData> RecentSalesData { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int LowStockProducts { get; set; }
    public List<OrderViewModel> RecentOrders { get; set; }
    public List<Customer> NewCustomers { get; set; }
    public List<User> CustomerUsers { get; set; }
}

public class OrderViewModel
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public double TotalAmount { get; set; }
    public int? Status { get; set; }
    public DateTime Date { get; set; }
}

public class SalesData
{
    public DateTime Date { get; set; }
    public double Amount { get; set; }
}