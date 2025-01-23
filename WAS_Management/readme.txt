Migration Command

dotnet ef dbcontext scaffold Name=DefaultConnection Pomelo.EntityFrameworkCore.MySql --output-dir Models --context-dir Data --namespace WAS_Management.Models --context-namespace WAS_Management.Data --context WAS_ManagementContext -f