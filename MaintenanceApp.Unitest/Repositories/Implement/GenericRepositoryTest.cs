using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SmartParking.Data;
using SmartParking.Infrastructure;
using SmartParking.Models;
using SmartParking.Repositories.Implement;

namespace SmartParking.Unitest.Repositories.Implement;

[TestClass]
[TestSubject(typeof(GenericRepository<Vehicle>))]
public class GenericRepositoryTest
{
    private ServiceProvider _serviceProvider;
    private AppDbContext _context;

    [TestInitialize]
    public async Task Setup()
    {
    }

    [TestMethod]
    public void METHOD()
    {
        
    }
}