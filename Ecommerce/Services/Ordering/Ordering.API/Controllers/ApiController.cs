using Microsoft.AspNetCore.Mvc;

namespace Ordering.API.Controllers;

[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
[ApiController]
public class ApiController : ControllerBase;