using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Million.Application.DTOs;
using Million.Application.Interfaces;
using Million.Domain.Entities;
using Million.Infrastructure.Persistence;
using System.Data;
using System.Text;

namespace Million.Infrastructure.Services
{
    /// <summary>
    /// Servicio encargado de administrar operaciones sobre Propiedades y Propietarios
    /// en el sistema inmobiliario Million.
    /// </summary>
    public class PropertyService : IPropertyService
    {
        private readonly IFileStorage _storage;
        private readonly string _connectionString;
        private IDbConnection @object;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="PropertyService"/>.
        /// </summary>
        /// <param name="storage">Servicio de almacenamiento de archivos.</param>
        /// <param name="configuration">Configuración de la aplicación (connection string).</param>
        public PropertyService(IFileStorage storage, IConfiguration configuration)
        {
            _storage = storage;
            _connectionString = configuration.GetConnectionString("RealEstateDb");
        }

        /// <summary>
        /// Crea un nuevo propietario en la base de datos.
        /// </summary>
        /// <param name="req">Datos del propietario.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>Id del propietario creado.</returns>
        /// <exception cref="InvalidOperationException">Si el propietario ya existe.</exception>
        public async Task<ActionResult<int>> CreateOwnerAsync(CreateOwnerRequest req, CancellationToken ct = default)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(ct);

                var existingOwner = await connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM Owner WHERE Name = @Name",
                    new { req.Name });

                if (existingOwner > 0)
                    throw new InvalidOperationException("Owner already exists");

                var url = await _storage.SaveFileAsync(req.Photo, "photo"+req.Name+".jpg", ct);

                var insertQuery = @"
                INSERT INTO Owner (Name, Address, Photo, Birthday)
                VALUES (@Name, @Address, @Photo, @Birthday);
                SELECT CAST(SCOPE_IDENTITY() AS INT);"; 

                var ownerId = await connection.QuerySingleAsync<int>(insertQuery, new
                {
                    req.Name,
                    req.Address,
                    Photo = url,
                    req.Birthday
                });

                return ownerId;
            }
        }

        /// <summary>
        /// Crea una nueva propiedad en la base de datos.
        /// </summary>
        /// <param name="req">Datos de la propiedad.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>Id de la propiedad creada.</returns>
        /// <exception cref="InvalidOperationException">Si ya existe un CodeInternal duplicado.</exception>
        public async Task<int> CreateAsync(CreatePropertyRequest req, CancellationToken ct = default)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(ct);

                var existingProperty = await connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM Property WHERE CodeInternal = @CodeInternal",
                    new { req.CodeInternal });

                if (existingProperty > 0)
                    throw new InvalidOperationException("CodeInternal already exists");

                var insertQuery = @"
                INSERT INTO Property (Name, Address, Price, CodeInternal, Year, IdOwner)
                VALUES (@Name, @Address, @Price, @CodeInternal, @Year, @IdOwner);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var propertyId = await connection.QuerySingleAsync<int>(insertQuery, new
                {
                    req.Name,
                    req.Address,
                    req.Price,
                    req.CodeInternal,
                    req.Year,
                    req.IdOwner
                });

                return propertyId;
            }
        }

        /// <summary>
        /// Agrega una imagen a una propiedad existente.
        /// </summary>
        /// <param name="idProperty">Id de la propiedad.</param>
        /// <param name="fileBase64">Archivo en base64.</param>
        /// <param name="fileName">Nombre del archivo.</param>
        /// <param name="ct">Token de cancelación.</param>
        /// <returns>Id de la imagen creada.</returns>
        /// <exception cref="KeyNotFoundException">Si la propiedad no existe.</exception>
        public async Task<int> AddImageAsync(int idProperty, string fileBase64, string fileName, CancellationToken ct = default)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(ct);

                var property = await connection.QueryFirstOrDefaultAsync<Property>(
                    "SELECT * FROM Property WHERE IdProperty = @IdProperty",
                    new { IdProperty = idProperty });

                if (property == null)
                    throw new KeyNotFoundException("Property not found");

                var url = await _storage.SaveFileAsync(fileBase64, fileName, ct);

                var query = @"
                INSERT INTO PropertyImage (IdProperty, [File], Enabled)
                VALUES (@IdProperty, @File, @Enabled);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var imageId = await connection.QuerySingleAsync<int>(query, new
                {
                    IdProperty = idProperty,
                    File = url,
                    Enabled = true
                });

                return imageId;
            }
        }

        /// <summary>
        /// Cambia el precio de una propiedad existente y registra el cambio en la tabla <c>PropertyTrace</c>.
        /// </summary>
        /// <param name="idProperty">Identificador único de la propiedad a modificar.</param>
        /// <param name="newPrice">Nuevo valor del precio que se desea asignar.</param>
        /// <param name="ct">Token de cancelación para abortar la operación de manera controlada.</param>
        /// <exception cref="KeyNotFoundException">
        /// Se lanza cuando no se encuentra ninguna propiedad con el <paramref name="idProperty"/> especificado.
        /// </exception>
        /// <remarks>
        /// Flujo del método:
        /// 1. Se obtiene la propiedad por su <paramref name="idProperty"/>.
        /// 2. Si no existe, se lanza una excepción.
        /// 3. Si el precio actual es igual al nuevo, no se realiza ningún cambio.
        /// 4. Si el precio es diferente, se actualiza en la tabla <c>Property</c>.
        /// 5. Se registra el cambio en la tabla <c>PropertyTrace</c> con el evento <c>PRICE_CHANGE</c>.
        /// </remarks>
        public async Task ChangePriceAsync(int idProperty, decimal newPrice, CancellationToken ct = default)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(ct);

                var property = await connection.QueryFirstOrDefaultAsync<Property>(
                    "SELECT * FROM Property WHERE IdProperty = @IdProperty",
                    new { IdProperty = idProperty });

                if (property == null)
                    throw new KeyNotFoundException("Property not found");

                if (property.Price == newPrice)
                    return;

                await connection.ExecuteAsync(
                    "UPDATE Property SET Price = @Price WHERE IdProperty = @IdProperty",
                    new { Price = newPrice, IdProperty = idProperty });

                var traceQuery = @"
                INSERT INTO PropertyTrace (IdProperty, Name, Value, Tax)
                VALUES (@IdProperty, @Name, @Value, @Tax)";

                await connection.ExecuteAsync(traceQuery, new
                {
                    IdProperty = idProperty,
                    Name = "PRICE_CHANGE",
                    Value = newPrice,
                    Tax = 0m
                });
            }
        }

        /// <summary>
        /// Actualiza la información básica de una propiedad existente.
        /// </summary>
        /// <param name="idProperty">Identificador único de la propiedad a actualizar.</param>
        /// <param name="req">Objeto con los nuevos datos de la propiedad.
        /// Si un campo no se envía, se mantiene el valor actual en base de datos.</param>
        /// <param name="ct">Token de cancelación para abortar la operación de manera controlada.</param>
        /// <exception cref="KeyNotFoundException">
        /// Se lanza si no existe ninguna propiedad con el <paramref name="idProperty"/> especificado.
        /// </exception>
        /// <remarks>
        /// - Campos actualizables: <c>Name</c>, <c>Address</c>, <c>Year</c>, <c>IdOwner</c>.  
        /// - El método aplica la lógica de "partial update":
        ///   - Si <c>req.Name</c> viene vacío, conserva el valor actual.
        ///   - Lo mismo aplica para <c>Address</c>, <c>Year</c> e <c>IdOwner</c>.
        /// </remarks>
        public async Task UpdateAsync(int idProperty, UpdatePropertyRequest req, CancellationToken ct = default)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(ct);

                  var property = await connection.QueryFirstOrDefaultAsync<Property>(
                    "SELECT * FROM Property WHERE IdProperty = @IdProperty",
                    new { IdProperty = idProperty });

                if (property == null)
                    throw new KeyNotFoundException("Property not found");

                var updateQuery = @"
                UPDATE Property
                SET Name = @Name,
                    Address = @Address,
                    Year = @Year,
                    IdOwner = @IdOwner
                WHERE IdProperty = @IdProperty";

                await connection.ExecuteAsync(updateQuery, new
                {
                    Name = !string.IsNullOrWhiteSpace(req.Name) ? req.Name : property.Name,
                    Address = !string.IsNullOrWhiteSpace(req.Address) ? req.Address : property.Address,
                    Year = req.Year.HasValue ? req.Year.Value : property.Year,
                    IdOwner = req.IdOwner.HasValue ? req.IdOwner.Value : property.IdOwner,
                    IdProperty = idProperty
                });
            }
        }

        /// <summary>
        /// Obtiene una lista paginada de propiedades aplicando filtros dinámicos.
        /// </summary>
        /// <param name="filter">Objeto que contiene los criterios de búsqueda:
        /// texto libre, rango de precios, año de construcción, propietario y paginación.</param>
        /// <param name="ct">Token de cancelación para abortar la operación de manera controlada.</param>
        /// <returns>
        /// Una tupla con:
        /// <list type="bullet">
        /// <item><description><c>Items</c>: Lista de propiedades que cumplen con los filtros.</description></item>
        /// <item><description><c>Total</c>: Número total de propiedades encontradas (antes de aplicar paginación).</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// Filtros aplicables:
        /// - <c>Text</c>: Busca coincidencias en <c>Name</c>, <c>Address</c> y <c>CodeInternal</c>.  
        /// - <c>OwnerId</c>: Filtra por propietario.  
        /// - <c>MinPrice</c> y <c>MaxPrice</c>: Rango de precios.  
        /// - <c>Year</c>: Año exacto de construcción.  
        ///
        /// El método también soporta:
        /// - Paginación con <c>Page</c> y <c>PageSize</c>.  
        /// - Orden descendente por <c>IdProperty</c>.  
        /// </remarks>
        public async Task<(IEnumerable<PropertyDto> Items, int Total)> ListAsync(PropertyFilter filter, CancellationToken ct = default)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(ct);

                // Construir la consulta de filtrado
                var query = new StringBuilder("SELECT p.IdProperty, p.Name, p.Address, p.Price, p.CodeInternal, p.Year, p.IdOwner, i.[File] AS ImageUrls " +
                                     "FROM Property p " +
                                     "LEFT JOIN PropertyImage i ON i.IdProperty = p.IdProperty AND i.Enabled = 1 " +
                                     "WHERE 1 = 1");

                if (!string.IsNullOrWhiteSpace(filter.Text))
                    query.Append(" AND (Name LIKE @Text OR Address LIKE @Text OR CodeInternal LIKE @Text)");

                if (filter.OwnerId.HasValue)
                    query.Append(" AND IdOwner = @OwnerId");

                if (filter.MinPrice.HasValue)
                    query.Append(" AND Price >= @MinPrice");

                if (filter.MaxPrice.HasValue)
                    query.Append(" AND Price <= @MaxPrice");

                if (filter.Year.HasValue)
                    query.Append(" AND Year = @Year");

                // Contar el total de propiedades que coinciden con el filtro
                var total = await connection.ExecuteScalarAsync<int>(query.ToString(), filter);

                // Paginación
                query.Append(" ORDER BY IdProperty DESC")
                     .Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");

                var items = await connection.QueryAsync<PropertyDto>(query.ToString(), new
                {
                    Text = $"%{filter.Text}%",
                    OwnerId = filter.OwnerId,
                    MinPrice = filter.MinPrice,
                    MaxPrice = filter.MaxPrice,
                    Year = filter.Year,
                    Offset = (filter.Page - 1) * filter.PageSize,
                    PageSize = filter.PageSize
                });

                return (items, total);
            }
        }
    }
}
