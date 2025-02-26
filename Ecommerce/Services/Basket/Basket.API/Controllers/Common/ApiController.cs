using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers.Common;

[Route("api/v{version:apiVersion}/[controller]/[action]")]
[ApiController]
public class ApiController : ControllerBase;