# Million Real Estate API

API desarrollada en **.NET 8** con **SQL Server**, orientada a gestionar propiedades inmobiliarias (crear, actualizar, cambiar precio, agregar imágenes, listar con filtros).  

## 🚀 Tecnologías usadas
- ASP.NET Core 8 (Web API)
- Entity Framework Core 8
- SQL Server
- JWT (Autenticación)
- NUnit (Unit Testing)
- FluentAssertions

## 🔐 Seguridad
- Autenticación JWT.
- Middleware para validar tokens.
- Cada request necesita enviar el **Bearer Token** en `Authorization`.

## 📂 Arquitectura

API_Million.RealEstate
│
├── Million.Api
│   ├── Connected Services
│   ├── Dependencias
│   ├── Properties
│   ├── wwwroot
│   ├── Controllers
│   │   ├── AuthController.cs
│   │   └── PropertiesController.cs
│   ├── UploadedFiles
│   ├── appsettings.json
│   ├── Program.cs
│   └── README.md
│
├── Million.Application
│   ├── Dependencias
│   ├── DTOs
│   │   ├── AddImageRequest.cs
│   │   ├── ChangePriceRequest.cs
│   │   ├── CreateOwnerRequest.cs
│   │   ├── CreatePropertyRequest.cs
│   │   ├── JwtSecretKey.cs
│   │   ├── PropertyDto.cs
│   │   ├── PropertyFilter.cs
│   │   └── UpdatePropertyRequest.cs
│   ├── Interfaces
│   │   ├── IAuthentication.cs
│   │   ├── IFileStorage.cs
│   │   └── IPropertyService.cs
│   └── README.md
│
├── Million.Domain
│   ├── Dependencias
│   ├── Analizadores
│   ├── Marcos de trabajo
│   ├── Entities
│   │   ├── LoginRequest.cs
│   │   ├── Owner.cs
│   │   ├── Property.cs
│   │   ├── PropertyImage.cs
│   │   └── PropertyTrace.cs
│   └── README.md
│
├── Million.Infrastructure
│   ├── Dependencias
│   ├── Persistence
│   │   └── RealEstateDbContext.cs
│   ├── Services
│   │   ├── AuthenticationService.cs
│   │   ├── FileStorage.cs
│   │   └── PropertyService.cs
│   └── README.md
│
└── Million.Tests
    ├── Dependencias
    ├── PropertyServiceTests.cs
    └── README.md