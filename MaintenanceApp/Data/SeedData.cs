using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using MaintenanceApp.Constant;
using MaintenanceApp.Infrastructure;
using MaintenanceApp.Models;
using MaintenanceApp.Services.Implement;

namespace MaintenanceApp.Data;

public class SeedData
{
    public static async Task Initialize(IServiceProvider services)
    {
        var environment = services.GetRequiredService<IHostEnvironment>();

        await EnsureRoles(services);
        await EnsureUsers(services);

        if (environment.IsDevelopment())
        {
            //await EnsureVehicleType(services);
            // await EnsureVehicle(services);
            //await EnsureParkingLot(services);
            // await EnsureParkingTicket(services);
        }
    }

    public static async Task EnsureRoles(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<Role>>();

        var roles = new List<Role>
        {
            new Role { Name = "admin", NormalizedName = "ADMIN" },
            new Role { Name = "supervisor", NormalizedName = "SUPERVISOR" },
            new Role { Name = "staff", NormalizedName = "STAFF" },
            new Role { Name = "reporter", NormalizedName = "REPORTER" }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name))
            {
                await roleManager.CreateAsync(role);
            }
        }
    }

    public static async Task EnsureUsers(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<User>>();

        // Thêm user với vai trò "admin"
        if (await userManager.FindByEmailAsync("admin@gmail.com") == null)
        {
            var adminUser = new User
            {
                UserName = "admin",
                Email = "admin@gmail.com",
            };

            var result = await userManager.CreateAsync(adminUser, "Test@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "admin");
            }
            else
            {
                throw new Exception("Tạo người dùng admin thất bại: " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Thêm user với vai trò "supervisor"
        if (await userManager.FindByEmailAsync("supervisor@gmail.com") == null)
        {
            var reporterUser = new User
            {
                UserName = "supervisor",
                Email = "supervisor@gmail.com",
            };

            var result = await userManager.CreateAsync(reporterUser, "Test@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(reporterUser, "supervisor");
            }
            else
            {
                throw new Exception("Tạo người dùng supervisor thất bại: " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }


        // Thêm user với vai trò "staff"
        if (await userManager.FindByEmailAsync("staff@gmail.com") == null)
        {
            var staffUser = new User
            {
                UserName = "staff",
                Email = "staff@gmail.com",
            };

            var result = await userManager.CreateAsync(staffUser, "Test@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(staffUser, "staff");
            }
            else
            {
                throw new Exception("Tạo người dùng staff thất bại: " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Thêm user với vai trò "reporter"
        if (await userManager.FindByEmailAsync("reporter@gmail.com") == null)
        {
            var reporterUser = new User
            {
                UserName = "reporter",
                Email = "reporter@gmail.com",
            };

            var result = await userManager.CreateAsync(reporterUser, "Test@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(reporterUser, "reporter");
            }
            else
            {
                throw new Exception("Tạo người dùng reporter thất bại: " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    // public static async Task EnsureVehicleType(IServiceProvider services)
    // {
    //     var unitOfWork = services.GetRequiredService<IUnitOfWork>();
    //     var vehicleTypesRepository = unitOfWork.GetRepository<VehicleType>();
    //
    //     List<VehicleType> vehicleTypes = new List<VehicleType>
    //     {
    //         new VehicleType
    //         {
    //             VehicleTypeName = "Xe máy",
    //             SeatCount = 2,
    //             MonthlyParkingFee = 120000.00m,
    //             DailyParkingFee = 12000.00m,
    //             NightParkingFee = 8000.00m,
    //             MorningParkingFee = 5000.00m,
    //         },
    //         new VehicleType
    //         {
    //             VehicleTypeName = "Ô tô dưới 9 chỗ",
    //             SeatCount = 8,
    //             MonthlyParkingFee = 4000000.00m,
    //             DailyParkingFee = 400000.00m,
    //             NightParkingFee = 250000.00m,
    //             MorningParkingFee = 200000.00m,
    //         },
    //         new VehicleType
    //         {
    //             VehicleTypeName = "Ô tô dưới 24 chỗ",
    //             SeatCount = 23,
    //             MonthlyParkingFee = 5000000.00m,
    //             DailyParkingFee = 500.50m,
    //             NightParkingFee = 300000.00m,
    //             MorningParkingFee = 250000.00m,
    //         },
    //     };
    //
    //     foreach (var vehicleType in vehicleTypes)
    //     {
    //         Expression<Func<VehicleType, bool>> filter = v => (
    //             v.VehicleTypeName == vehicleType.VehicleTypeName
    //         );
    //
    //         if (!vehicleTypesRepository.GetQuery(filter).Any())
    //         {
    //             // Nếu không tồn tại, thêm loại phương tiện vào repository
    //             vehicleTypesRepository.Add(vehicleType);
    //         }
    //     }
    //
    //
    //     await unitOfWork.SaveChangesAsync();
    // }
    //
    // public static async Task EnsureVehicle(IServiceProvider services)
    // {
    //     var userManager = services.GetRequiredService<UserManager<User>>();
    //     var unitOfWork = services.GetRequiredService<IUnitOfWork>();
    //     var vehicleRepository = unitOfWork.GetRepository<Vehicle>();
    //     var vehicleTypeRepository = unitOfWork.GetRepository<VehicleType>();
    //
    //     // Lấy thông tin người dùng "reporter"
    //     var reporterUser = await userManager.FindByEmailAsync("reporter@gmail.com");
    //     if (reporterUser == null)
    //     {
    //         throw new Exception("Không tìm thấy người dùng reporter");
    //     }
    //
    //     // Lấy các loại phương tiện (VehicleType) từ cơ sở dữ liệu
    //     var vehicleTypes = vehicleTypeRepository
    //         .GetAll()
    //         .Where(type => type.IsActive == true)
    //         .ToList();
    //
    //     var vehicles = new List<Vehicle>
    //     {
    //         new Vehicle
    //         {
    //             UserId = reporterUser.Id,
    //             VehicleTypeId = vehicleTypes.ElementAt(0).Id,
    //             LicensePlate = "ABC123",
    //             Color = "Red",
    //             VehicleName = "Airblade"
    //         },
    //         new Vehicle
    //         {
    //             UserId = reporterUser.Id,
    //             VehicleTypeId = vehicleTypes.ElementAt(1).Id,
    //             LicensePlate = "XYZ789",
    //             Color = "Blue",
    //             VehicleName = "Airblade 2"
    //         },
    //         new Vehicle
    //         {
    //             UserId = reporterUser.Id,
    //             VehicleTypeId = vehicleTypes.ElementAt(2).Id,
    //             LicensePlate = "DEF456",
    //             Color = "Black",
    //             VehicleName = "Airblade 3"
    //         }
    //     };
    //
    //
    //     foreach (var vehicle in vehicles)
    //     {
    //         Expression<Func<Vehicle, bool>> filter = v => v.LicensePlate.ToUpper() == vehicle.LicensePlate.ToUpper();
    //
    //         if (!vehicleRepository.GetQuery(filter).Any())
    //         {
    //             // Nếu không tồn tại, thêm phương tiện vào repository
    //             vehicleRepository.Add(vehicle);
    //         }
    //     }
    //
    //     await unitOfWork.SaveChangesAsync();
    // }
    //
    // public static async Task EnsureParkingLot(IServiceProvider services)
    // {
    //     var unitOfWork = services.GetRequiredService<IUnitOfWork>();
    //     var parkingLotRepository = unitOfWork.GetRepository<ParkingLot>();
    //     var vehicleTypeRepository = unitOfWork.GetRepository<VehicleType>();
    //
    //     var vehicleTypes = vehicleTypeRepository.GetAll().ToList();
    //
    //     var parkingLots = new List<ParkingLot>
    //     {
    //         new ParkingLot
    //         {
    //             ParkingLotCode = "PL001",
    //             VehicleTypeId = vehicleTypes[0].Id,
    //             Description = "Lot for motorbikes",
    //             Status = ParkingLotStatus.Empty,
    //             IsActive = true
    //         },
    //         new ParkingLot
    //         {
    //             ParkingLotCode = "PL002",
    //             VehicleTypeId = vehicleTypes[1].Id,
    //             Description = "Lot for cars under 9 seats",
    //             Status = ParkingLotStatus.Empty,
    //             IsActive = true
    //         },
    //         new ParkingLot
    //         {
    //             ParkingLotCode = "PL003",
    //             VehicleTypeId = vehicleTypes[2].Id,
    //             Description = "Lot for larger vehicles",
    //             Status = ParkingLotStatus.Empty,
    //             IsActive = true
    //         },
    //         new ParkingLot
    //         {
    //             ParkingLotCode = "PL004",
    //             VehicleTypeId = vehicleTypes[0].Id,
    //             Description = "Additional motorbike lot",
    //             Status = ParkingLotStatus.Empty,
    //             IsActive = true
    //         },
    //         new ParkingLot
    //         {
    //             ParkingLotCode = "PL005",
    //             VehicleTypeId = vehicleTypes[1].Id,
    //             Description = "Overflow car lot",
    //             Status = ParkingLotStatus.Empty,
    //             IsActive = true
    //         }
    //     };
    //
    //     foreach (var parkingLot in parkingLots)
    //     {
    //         if (!parkingLotRepository.GetQuery(p => p.ParkingLotCode == parkingLot.ParkingLotCode).Any())
    //         {
    //             parkingLotRepository.Add(parkingLot);
    //         }
    //     }
    //
    //     await unitOfWork.SaveChangesAsync();
    // }
    //
    // public static async Task EnsureParkingTicket(IServiceProvider services)
    // {
    //     var unitOfWork = services.GetRequiredService<IUnitOfWork>();
    //     var parkingTicketRepository = unitOfWork.GetRepository<ParkingTicket>();
    //     var vehicleRepository = unitOfWork.GetRepository<Vehicle>();
    //     var parkingLotRepository = unitOfWork.GetRepository<ParkingLot>();
    //     var monthlyTickets = unitOfWork.GetRepository<MonthlyTicket>().GetAll().ToList();
    //     var vehicles = vehicleRepository.GetAll().ToList();
    //     var parkingLots = parkingLotRepository.GetAll().ToList();
    //
    //     var parkingTickets = new List<ParkingTicket>
    //     {
    //         new ParkingTicket
    //         {
    //             VehicleId = vehicles[0].Id,
    //             ParkingLotId = parkingLots[0].Id,
    //             ParkingLotCode = parkingLots[0].ParkingLotCode,
    //             ParkingTicketType = ParkingTicketType.SingleTicket,
    //             ParkingSpot = "A1",
    //             LicensePlate = vehicles[0].LicensePlate,
    //             EntryTime = DateTime.UtcNow,
    //             VehicleColor = vehicles[0].Color,
    //             IsActive = true
    //         },
    //         new ParkingTicket
    //         {
    //             MonthlyTicketId = monthlyTickets[0].Id,
    //             ParkingLotId = parkingLots[0].Id,
    //             ParkingLotCode = parkingLots[0].ParkingLotCode,
    //             ParkingTicketType = ParkingTicketType.MonthlyTicket,
    //             ParkingSpot = "A2",
    //             LicensePlate = vehicles[1].LicensePlate,
    //             EntryTime = DateTime.UtcNow,
    //             VehicleColor = vehicles[1].Color,
    //             IsActive = true
    //         }
    //     };
    //
    //     foreach (var parkingTicket in parkingTickets)
    //     {
    //         if (!parkingTicketRepository.GetQuery(pt =>
    //                     pt.LicensePlate == parkingTicket.LicensePlate && pt.ParkingLotId == parkingTicket.ParkingLotId)
    //                 .Any())
    //         {
    //             parkingTicketRepository.Add(parkingTicket);
    //         }
    //     }
    //
    //     await unitOfWork.SaveChangesAsync();
    // }
}