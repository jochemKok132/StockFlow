using System.Security.Claims;

namespace StockFlow.UnitTests.Common
{
    public class RoleHierarchyTests
    {
        [Theory]
        [InlineData(EmployeeRoleDto.Admin, EmployeeRoleDto.Manager, true)]
        [InlineData(EmployeeRoleDto.Admin, EmployeeRoleDto.Tester, true)]
        [InlineData(EmployeeRoleDto.Admin, EmployeeRoleDto.Admin, false)]
        [InlineData(EmployeeRoleDto.Manager, EmployeeRoleDto.ShiftLeader, true)]
        [InlineData(EmployeeRoleDto.Manager, EmployeeRoleDto.FloorWorker, true)]
        [InlineData(EmployeeRoleDto.Manager, EmployeeRoleDto.Manager, false)]
        [InlineData(EmployeeRoleDto.Manager, EmployeeRoleDto.Tester, false)]
        [InlineData(EmployeeRoleDto.Manager, EmployeeRoleDto.Admin, false)]
        [InlineData(EmployeeRoleDto.ShiftLeader, EmployeeRoleDto.Cashier, false)]
        [InlineData(EmployeeRoleDto.Tester, EmployeeRoleDto.Cashier, false)]
        [InlineData(EmployeeRoleDto.Admin, EmployeeRoleDto.None, false)]
        public void CanManage_OnlyManagersAndOnlyBelowThemselves(EmployeeRoleDto actor, EmployeeRoleDto target, bool expected)
        {
            Assert.Equal(expected, RoleHierarchy.CanManage(actor, target));
        }

        [Fact]
        public void AssignableRoles_Manager_AreTheRolesBelowManager()
        {
            var result = RoleHierarchy.AssignableRoles(EmployeeRoleDto.Manager);

            Assert.Equal([EmployeeRoleDto.ShiftLeader, EmployeeRoleDto.ServiceDesk, EmployeeRoleDto.Cashier, EmployeeRoleDto.FloorWorker], result);
        }

        [Fact]
        public void HiddenFrom_NonAdmin_HidesAdminAndTester()
        {
            Assert.Equal([EmployeeRoleDto.Admin, EmployeeRoleDto.Tester], RoleHierarchy.HiddenFrom(EmployeeRoleDto.Manager));
            Assert.Empty(RoleHierarchy.HiddenFrom(EmployeeRoleDto.Admin));
        }

        [Fact]
        public void EmployeeRole_ReadsTheRoleClaim()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, "ShiftLeader")], "test"));

            Assert.Equal(EmployeeRoleDto.ShiftLeader, user.EmployeeRole());
            Assert.Equal(EmployeeRoleDto.None, new ClaimsPrincipal(new ClaimsIdentity()).EmployeeRole());
        }

        [Fact]
        public void Permissions_Allows_ChecksEveryListedRole()
        {
            var cashier = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, "Cashier")], "test"));
            var tester = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, "Tester")], "test"));

            Assert.True(cashier.Allows(Permissions.CashRegister));
            Assert.False(cashier.Allows(Permissions.ManageStock));
            Assert.False(tester.Allows(Permissions.ManageStock));
            Assert.True(tester.CanOpen(Permissions.ManageStock));
        }
    }
}
