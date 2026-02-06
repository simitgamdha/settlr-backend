namespace Settlr.Common.Helper;

public static class ApiRoutes
{
    public const string Base = "api";
    public const string Auth = Base + "/auth";
    public const string AuthRegister = Auth + "/register";
    public const string AuthLogin = Auth + "/login";
    public const string Groups = Base + "/groups";
    public const string GroupMembers = Base + "/groups/{groupId}/members";
    public const string GroupBalances = Base + "/groups/{groupId}/balances";
    public const string GroupExpenses = Base + "/groups/{groupId}/expenses";
    public const string Expenses = Base + "/expenses";
    public const string Dashboard = Base + "/dashboard";
    public const string DashboardSummary = Base + "/dashboard/summary";
}
