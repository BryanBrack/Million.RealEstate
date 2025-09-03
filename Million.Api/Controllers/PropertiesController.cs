using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Million.Application.DTOs;
using Million.Application.Interfaces;

namespace Million.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/properties")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _svc;
        public PropertiesController(IPropertyService svc) => _svc = svc;

        /// <summary>
        /// Crea un nuevo propietario.
        /// </summary>
        /// <param name="req">Información del propietario a crear.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>Id del nuevo propietario creado.</returns>
        [HttpPost("createOwner")]
        public async Task<ActionResult<int>> CreateOwner(CreateOwnerRequest req, CancellationToken ct)
        {
            var id = await _svc.CreateOwnerAsync(req, ct);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        /// <summary>
        /// Crea una nueva propiedad.
        /// </summary>
        /// <param name="req">Información de la propiedad a crear.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>Id de la nueva propiedad creada.</returns>
        [HttpPost("createProperty")]
        public async Task<ActionResult<int>> CreateProperty(CreatePropertyRequest req, CancellationToken ct)
        {
            var id = await _svc.CreateAsync(req, ct);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        /// <summary>
        /// Agrega una imagen a una propiedad.
        /// </summary>
        /// <param name="id">Id de la propiedad.</param>
        /// <param name="req">Datos de la imagen a agregar.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost("{id:int}/images")]
        public async Task<IActionResult> AddImage(int id, AddImageRequest req, CancellationToken ct)
        {
            await _svc.AddImageAsync(id, req.FileBase64, req.FileName, ct);
            return NoContent();
        }

        /// <summary>
        /// Cambia el precio de una propiedad.
        /// </summary>
        /// <param name="id">Id de la propiedad.</param>
        /// <param name="body">Nuevo precio de la propiedad.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost("{id:int}/price")]
        public async Task<IActionResult> ChangePrice(int id, ChangePriceRequest body, CancellationToken ct)
        {
            await _svc.ChangePriceAsync(id, body.NewPrice, ct);
            return NoContent();
        }

        /// <summary>
        /// Actualiza la información de una propiedad.
        /// </summary>
        /// <param name="id">Id de la propiedad a actualizar.</param>
        /// <param name="req">Nuevos datos de la propiedad.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPatch("{id:int}/updateProperty")]
        public async Task<IActionResult> Update(int id, UpdatePropertyRequest req, CancellationToken ct)
        {
            await _svc.UpdateAsync(id, req, ct);
            return NoContent();
        }

        /// <summary>
        /// Lista todas las propiedades con filtros opcionales.
        /// </summary>
        /// <param name="q">Filtro de búsqueda de propiedades.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>Lista de propiedades filtradas.</returns>
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PropertyFilter q, CancellationToken ct)
        {
            var filter = q.ToFilter();
            var (items, total) = await _svc.ListAsync(filter, ct);
            return Ok(new { items, total, page = filter.Page, pageSize = filter.PageSize });
        }

        /// <summary>
        /// Obtiene una propiedad por el ID del propietario.
        /// </summary>
        /// <param name="IdOwner">Id del propietario.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>Detalles de la propiedad.</returns>
        [HttpGet("{IdOwner:int}")]
        public async Task<IActionResult> GetById(int IdOwner, CancellationToken ct)
        {
            var (items, _) = await _svc.ListAsync(new PropertyFilter(null, IdOwner, null, null, null, 1, 1, null), ct);
            var dto = items.FirstOrDefault();
            if (dto is null) return NotFound();
            return Ok(dto);
        }
    }
}
