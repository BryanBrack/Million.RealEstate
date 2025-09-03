# Million Real Estate API

API desarrollada en **.NET 8** con **SQL Server**, orientada a gestionar propiedades inmobiliarias (crear, actualizar, cambiar precio, agregar imágenes, listar con filtros).  

## 🚀 Tecnologías usadas
- ASP.NET Core 8 (Web API)
- Dapper
- SQL Server
- JWT (Autenticación)
- NUnit (Unit Testing)
- FluentAssertions

## 🔐 Seguridad
- Autenticación JWT.
- Middleware para validar tokens.
- Cada request necesita enviar el **Bearer Token** en `Authorization`.

## 📂 Arquitectura

| **Carpeta**               | **Contenido**                                                                 |
|---------------------------|-------------------------------------------------------------------------------|
| `Million.Api`              | Contiene los controladores y configuraciones principales.                     |
|                           | - `AuthController.cs`                                                         |
|                           | - `PropertiesController.cs                                                   |
|                           | - `appsettings.json`                                                          |
|                           | - `Program.cs`                                                                |
|                           | - `README.md`                                                                |
| `Million.Application`      | Contiene los DTOs, interfaces y lógica de negocio.                             |
|                           | - `DTOs/`                                                                     |
|                           | - `Interfaces/`                                                               |
|                           | - `README.md`                                                                |
| `Million.Domain`           | Contiene las entidades del dominio.                                           |
|                           | - `Entities/`                                                                 |
|                           | - `README.md`                                                                |
| `Million.Infrastructure`   | Contiene la infraestructura de acceso a datos y otros servicios.              |
|                           | - `Persistence/`                                                              |
|                           | - `Services/`                                                                 |
|                           | - `README.md`                                                                |
| `Million.Tests`            | Contiene las pruebas unitarias del proyecto.                                  |
|                           | - `PropertyServiceTests.cs`                                                   |
|                           | - `README.md`                                                                |

