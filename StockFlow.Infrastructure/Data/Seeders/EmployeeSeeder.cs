using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class EmployeeSeeder
    {
        public static void UseEmployeeSeeder(this DbContext context)
        {
            if (!context.Set<Employee>().Any())
            {
                // All passwords are "StrongPassword1!".
                IEnumerable<Employee> Employees = new List<Employee>()
                {
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1dfb57"),
                        EmployeeId = "000001",
                        FullName = "Admin",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.Admin,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb57"),
                        EmployeeId = "000002",
                        FullName = "Manager",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.Manager,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb56"),
                        EmployeeId = "000003",
                        FullName = "ShiftLeader One",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.ShiftLeader,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb55"),
                        EmployeeId = "000004",
                        FullName = "ShiftLeader Two",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.ShiftLeader,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb54"),
                        EmployeeId = "000005",
                        FullName = "ShiftLeader Three",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.ShiftLeader,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb53"),
                        EmployeeId = "000006",
                        FullName = "ServiceDesk One",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.ServiceDesk,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb52"),
                        EmployeeId = "000007",
                        FullName = "ServiceDesk Two",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.ServiceDesk,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb53"),
                        EmployeeId = "000008",
                        FullName = "ServiceDesk Three",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.ServiceDesk,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb52"),
                        EmployeeId = "000009",
                        FullName = "ServiceDesk Four",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.ServiceDesk,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb51"),
                        EmployeeId = "000010",
                        FullName = "Cashier One",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.Cashier,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb47"),
                        EmployeeId = "000011",
                        FullName = "Cashier Two",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.Cashier,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb46"),
                        EmployeeId = "000012",
                        FullName = "Cashier Three",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.Cashier,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb45"),
                        EmployeeId = "000013",
                        FullName = "FloorWorker One",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.FloorWorker,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb44"),
                        EmployeeId = "000014",
                        FullName = "FloorWorker Two",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.FloorWorker,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb43"),
                        EmployeeId = "000015",
                        FullName = "FloorWorker Three",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.FloorWorker,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb42"),
                        EmployeeId = "000016",
                        FullName = "FloorWorker Four",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.FloorWorker,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb41"),
                        EmployeeId = "000017",
                        FullName = "FloorWorker Five",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.FloorWorker,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb37"),
                        EmployeeId = "000018",
                        FullName = "FloorWorker Six",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.FloorWorker,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb36"),
                        EmployeeId = "000019",
                        FullName = "FloorWorker Seven",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.FloorWorker,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb35"),
                        EmployeeId = "000020",
                        FullName = "FloorWorker Eight",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.FloorWorker,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb34"),
                        EmployeeId = "000021",
                        FullName = "FloorWorker Nine",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.FloorWorker,
                    },
                    new Employee()
                    {
                        Id = new Guid("7be9b779-ac76-440f-ad76-1251ff1efb33"),
                        EmployeeId = "000022",
                        FullName = "FloorWorker Ten",
                        HashedPassword = "$2a$12$H9wz9isUk6/mBBi5nWpBAeZhV.lXmIz/qP7w43.jwm9CSUDA83kt2",
                        Role = EmployeeRole.FloorWorker,
                    },
                };
                context.Set<Employee>().AddRange(Employees);
                context.SaveChanges();
            }
        }
    }
}