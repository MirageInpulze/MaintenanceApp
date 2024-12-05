using Microsoft.AspNetCore.Mvc;

namespace MaintenanceApp.Controllers;
[ApiController]
[Route("api/[controller]/[action]")]
public class TransactionHistoryController: ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTransactionHistory()
    {
        return Ok();
    }
}