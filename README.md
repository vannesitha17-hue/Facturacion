# Sistema de Gestión de Facturación y Recordatorios

## 1. Descripción del proyecto

Aplicación Fullstack desarrollada para consultar las facturas de los clientes y procesar recordatorios de pago. El sistema identifica las facturas según su estado, simula el envío de notificaciones por correo electrónico y actualiza los estados correspondientes en MongoDB.

El proyecto aplica separación de responsabilidades mediante servicios y repositorios, inyección de dependencias y pruebas unitarias de la lógica de negocio.

## 2. Tecnologías utilizadas

- **Backend:** C# y ASP.NET Core con .NET 10.
- **Frontend:** Angular.
- **Base de datos:** MongoDB.
- **Pruebas unitarias:** xUnit y Moq.
- **Arquitectura:** servicios, repositorios e inyección de dependencias.
- **Control de versiones:** Git y GitHub.

## 3. Funcionalidades

- Consultar las facturas registradas en MongoDB.
- Filtrar las facturas por estado.
- Visualizar el número de factura, cliente, correo electrónico, valor, fecha de vencimiento y estado.
- Procesar recordatorios de pago pendientes.
- Simular el envío de correos electrónicos.
- Actualizar el estado de las facturas después de procesar correctamente la notificación.
- Manejar errores durante el procesamiento de las facturas.
- Ejecutar pruebas unitarias de la lógica de negocio.

## 4. Reglas de negocio

| Estado inicial | Acción | Estado final |
|---|---|---|
| `primerrecordatorio` | Simular el envío del segundo recordatorio | `segundorecordatorio` |
| `segundorecordatorio` | Simular el aviso de desactivación | `desactivado` |
| `pagada` | No requiere procesamiento | `pagada` |

El sistema consulta las facturas pendientes de recordatorio y procesa cada una según su estado.

El estado se actualiza en MongoDB después de que el servicio de correo simulado finaliza correctamente. Si ocurre un error durante el envío, la factura no debe avanzar al siguiente estado.

**Nota:** el envío de correos electrónicos está simulado mediante registros de la aplicación. No se envían mensajes reales.

## 5. Estructura del proyecto

```text
Facturacion/
├── Facturacion.ApiService/
│   ├── Constants/
│   ├── Models/
│   ├── Repositories/
│   ├── Services/
│   ├── Program.cs
│   └── appsettings.json
├── Facturacion.AppHost/
├── Facturacion.ServiceDefaults/
├── Facturacion.Tests/
│   └── Services/
├── facturacion-frontend/
├── global.json
└── README.md
```

- **Facturacion.ApiService:** contiene la API, los modelos, los servicios, los repositorios y los endpoints.
- **Facturacion.AppHost:** contiene la configuración de ejecución de la aplicación distribuida.
- **Facturacion.ServiceDefaults:** contiene configuraciones compartidas de los servicios.
- **Facturacion.Tests:** contiene las pruebas unitarias.
- **facturacion-frontend:** contiene la interfaz de usuario desarrollada en Angular.
- **global.json:** permite seleccionar la versión del SDK de .NET configurada para el proyecto.
- **README.md:** documenta el proyecto y los pasos para ejecutarlo.

## 6. Requisitos previos

Antes de ejecutar la aplicación, se requiere:

1. SDK de .NET 10 compatible con la versión configurada en `global.json`.
2. Node.js y npm compatibles con la versión de Angular del proyecto.
3. Una instancia de MongoDB local o en la nube.
4. Acceso a la base de datos y una colección de facturas con datos de prueba.

## 7. Configuración de MongoDB

La API requiere los siguientes parámetros de configuración:

- `MongoDb:ConnectionString`: cadena de conexión a MongoDB.
- `MongoDb:DatabaseName`: nombre de la base de datos.

En el entorno de desarrollo, se pueden configurar mediante User Secrets de .NET.

Desde la carpeta raíz de la solución, ejecutar:

```powershell
dotnet user-secrets set "MongoDb:ConnectionString" "TU_CADENA_DE_CONEXION" --project .\Facturacion.ApiService\Facturacion.ApiService.csproj

dotnet user-secrets set "MongoDb:DatabaseName" "TU_BASE_DE_DATOS" --project .\Facturacion.ApiService\Facturacion.ApiService.csproj
```

Reemplazar los valores de ejemplo por la información correspondiente al entorno local.

La colección utilizada por el repositorio es `Facturas`.

**Seguridad:** no publicar contraseñas, cadenas de conexión con credenciales ni otros secretos en el repositorio. Para otros entornos, configurar las variables de entorno o el mecanismo de gestión de secretos correspondiente.

## 8. Ejecución del backend

Desde la carpeta raíz del proyecto, ejecutar:

```powershell
dotnet run --project .\Facturacion.ApiService\Facturacion.ApiService.csproj
```

La API utilizará la dirección y el puerto indicados por su configuración de inicio.

### Endpoints principales

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/facturas` | Consulta las facturas registradas. |
| GET | `/api/facturas/estado/{estado}` | Consulta las facturas por estado. |
| POST | `/api/facturas/procesar-recordatorios` | Procesa los recordatorios pendientes. |
| GET | `/mongodb-test` | Comprueba la conexión con MongoDB. |

Para probar los endpoints, utilizar la dirección base que muestre la terminal al iniciar la API.

## 9. Ejecución del frontend

Abrir una segunda terminal y ejecutar:

```powershell
cd .\facturacion-frontend
npm install
npm.cmd start
```

La interfaz estará disponible en:

http://localhost:4200/

Verificar que la dirección de la API configurada en el servicio Angular coincida con el puerto en el que se ejecuta el backend.

## 10. Ejecución de las pruebas unitarias

Desde la carpeta raíz de la solución, ejecutar:

```powershell
dotnet test .\Facturacion.Tests\Facturacion.Tests.csproj
```

Las pruebas cubren escenarios de la lógica de negocio, incluyendo las transiciones de estado, las facturas pagadas y el manejo de errores durante el procesamiento de los recordatorios.

## 11. Estado de las pruebas

Durante la validación del proyecto se ejecutaron correctamente **6 pruebas unitarias, con 0 fallidas**.

También se verificó el funcionamiento de la interfaz Angular, el procesamiento de los recordatorios y la persistencia de los cambios en MongoDB después de recargar la página.

## 12. Consideraciones y mejoras futuras

- Integrar un proveedor real de correo electrónico.
- Fortalecer el manejo de reintentos y la prevención de notificaciones duplicadas.
- Ajustar las direcciones de la API y la configuración de CORS para cada entorno.
- Incorporar validaciones adicionales y pruebas de integración.
- Configurar adecuadamente los secretos y las credenciales antes de desplegar el sistema en producción.