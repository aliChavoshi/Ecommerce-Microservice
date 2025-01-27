using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
[ApiController]
public class ApiController : ControllerBase;