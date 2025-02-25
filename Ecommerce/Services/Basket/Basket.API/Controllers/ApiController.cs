using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[Route("api/v{version:apiVersion}/[controller]/[action]")]
[ApiController]
public class ApiController : ControllerBase;