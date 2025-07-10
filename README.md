
# 📋 TaskManager API

> **API REST profesional para gestión de tareas y productividad personal**

Una API completa desarrollada en **C# .NET 8** que demuestra las mejores prácticas de desarrollo backend, incluyendo autenticación JWT, Entity Framework Core, arquitectura en capas y documentación interactiva.

---

## 🚀 **Características Principales**

### 🔐 **Autenticación y Seguridad**
- **JWT (JSON Web Tokens)** para autenticación stateless
- **BCrypt** para hashing seguro de contraseñas
- **Autorización por usuario** - cada usuario solo ve sus propios datos
- **Validaciones robustas** en todos los endpoints

### 📊 **Funcionalidades de Negocio**
- **CRUD completo** para Usuarios, Tareas y Categorías
- **Dashboard con estadísticas** en tiempo real
- **Filtros avanzados** (tareas vencidas, por prioridad)
- **Sistema de categorías** con colores personalizables
- **Prioridades de tareas** (Baja, Media, Alta)
- **Fechas de vencimiento** con control de tareas atrasadas

### 🏗️ **Arquitectura y Patrones**
- **Arquitectura en capas** (Controllers → Services → Data)
- **Repository Pattern** con Entity Framework Core
- **Dependency Injection** nativo de .NET
- **DTOs** para transferencia segura de datos
- **Manejo centralizado de errores** con `ApiResponse<T>`

### 🛠️ **Tecnologías y Herramientas**
- **.NET 8** - Framework principal
- **Entity Framework Core 8.0** - ORM y migraciones
- **SQL Server LocalDB** - Base de datos relacional
- **Swagger/OpenAPI** - Documentación interactiva
- **CORS** configurado para desarrollo frontend

---

## 📋 **Endpoints Disponibles**

### 🔐 **Autenticación**

POST   /api/auth/register     # Registro de usuarios
POST   /api/auth/login        # Inicio de sesión
GET    /api/auth/me           # Información del usuario actual


### ✅ **Gestión de Tareas**

GET    /api/tasks             # Obtener todas las tareas del usuario
GET    /api/tasks/{id}        # Obtener tarea específica
POST   /api/tasks             # Crear nueva tarea
PUT    /api/tasks/{id}        # Actualizar tarea completa
PATCH  /api/tasks/{id}/toggle # Cambiar estado completado/pendiente
DELETE /api/tasks/{id}        # Eliminar tarea
GET    /api/tasks/overdue     # Tareas vencidas
GET    /api/tasks/priority/{priority} # Tareas por prioridad (1-3)


### 📂 **Categorías**

GET    /api/categories        # Obtener todas las categorías
GET    /api/categories/{id}   # Obtener categoría específica
POST   /api/categories        # Crear nueva categoría
PUT    /api/categories/{id}   # Actualizar categoría
DELETE /api/categories/{id}   # Eliminar categoría

### 👥 **Usuarios**
GET    /api/users             # Obtener todos los usuarios
GET    /api/users/{id}        # Obtener usuario específico
PUT    /api/users/{id}        # Actualizar perfil de usuario
DELETE /api/users/{id}        # Eliminar cuenta (soft delete)
GET    /api/users/statistics  # Dashboard con estadísticas del usuario



## 🛠️ **Instalación y Configuración**

### **Prerrequisitos**
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (Community o superior)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server LocalDB (incluido con Visual Studio)

### **Clonar el Repositorio**

git clone https://github.com/tu-usuario/taskmanager-api.git
cd taskmanager-api


### **Instalar Dependencias**
bash
# Restaurar paquetes NuGet
dotnet restore

### **Configurar Base de Datos**
bash
# Crear migración inicial
dotnet ef migrations add InitialCreate

# Aplicar migración
dotnet ef database update


### **Ejecutar la Aplicación**
bash
# Modo desarrollo
dotnet run

# O presionar F5 en Visual Studio


### **Acceder a la API**
- **Swagger UI:** https://localhost:7286
- **API Base URL:** https://localhost:7286/api

---

## 📊 **Estructura de la Base de Datos**

sql
Users
├── Id (PK)
├── Username (Unique)
├── Email (Unique)
├── Password (Hashed)
├── CreatedAt
└── IsActive

Categories
├── Id (PK)
├── Name
├── Description
├── Color (Hex)
└── CreatedAt

Tasks
├── Id (PK)
├── Title
├── Description
├── IsCompleted
├── Priority (1-3)
├── DueDate
├── CreatedAt
├── UpdatedAt
├── UserId (FK)
└── CategoryId (FK)


---

## 🎯 **Demo Rápido**

### 1. **Registrar Usuario**
json
POST /api/auth/register
{
  "username": "demo_user",
  "email": "demo@example.com",
  "password": "Demo123456"
}


### 2. **Iniciar Sesión**
json
POST /api/auth/login
{
  "username": "demo_user",
  "password": "Demo123456"
}


### 3. **Crear Categoría**
json
POST /api/categories
Authorization: Bearer {jwt-token}
{
  "name": "Desarrollo",
  "description": "Tareas de programación",
  "color": "#3498db"
}


### 4. **Crear Tarea**
json
POST /api/tasks
Authorization: Bearer {jwt-token}
{
  "title": "Completar API TaskManager",
  "description": "Finalizar todos los endpoints",
  "priority": 3,
  "dueDate": "2025-07-15T10:00:00Z",
  "categoryId": 1
}


### 5. **Ver Estadísticas**
http
GET /api/users/statistics
Authorization: Bearer {jwt-token}


---

## 🎨 **Características Destacadas**

### **Para Desarrolladores**
- ✅ **Código limpio** y bien documentado
- ✅ **Patrones de diseño** implementados correctamente
- ✅ **Separación de responsabilidades** clara
- ✅ **Validaciones** en múltiples capas
- ✅ **Manejo de errores** profesional

### **Para el Negocio**
- ✅ **Funcionalidad completa** de gestión de tareas
- ✅ **Dashboard informativo** con métricas
- ✅ **Filtros útiles** para productividad
- ✅ **Interfaz documented** con Swagger
- ✅ **Escalabilidad** pensada desde el diseño

---

## 📚 **Tecnologías Utilizadas**

| Tecnología | Versión | Propósito |
|------------|---------|-----------|
| **.NET** | 8.0 | Framework principal |
| **C#** | 12.0 | Lenguaje de programación |
| **Entity Framework Core** | 8.0 | ORM y acceso a datos |
| **SQL Server LocalDB** | - | Base de datos |
| **JWT Bearer** | 8.0 | Autenticación |
| **BCrypt.Net** | 4.0.3 | Hashing de contraseñas |
| **Swagger** | 6.5.0 | Documentación de API |

---

## 🔧 **Configuración Avanzada**

### **Variables de Entorno**
json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=TaskManagerDB;Trusted_Connection=true;TrustServerCertificate=true"
  },
  "JwtSettings": {
    "SecretKey": "tu-clave-secreta-aqui",
    "Issuer": "TaskManagerAPI",
    "Audience": "TaskManagerUsers",
    "ExpiryInMinutes": 60
  }
}


### **CORS para Frontend**
La API está configurada para aceptar requests desde cualquier origen en desarrollo. Para producción, configurar dominios específicos.

---

## 🤝 **Contribuciones**

¡Las contribuciones son bienvenidas! Si quieres mejorar este proyecto:

1. Fork el repositorio
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

---


---

## 👨‍💻 **Autor**

**Tu Nombre**
- LinkedIn: [tu-perfil-linkedin](https://www.linkedin.com/in/josue-barboza-250566267/)
- Email: jabs9606@gmail.com
- Portfolio: [tu-portfolio.com](https://josuebarboza.netlify.app/)
