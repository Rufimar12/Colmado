using Ardalis.ApiEndpoints;
using Colmado.Server.Context;
using Colmado.Server.Models;
using Colmado.Shared.Request;
using Colmado.Shared.Routes;
using Colmado.Shared.Wrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Colmado.Server.Endpoints.UsuariosRoles;

using Request = UsuarioRolCreateRequest;
using Respuesta = Result<int>;

public class Create : EndpointBaseAsync.WithRequest<Request>.WithActionResult<Respuesta>
{
    private readonly IMyDbContext dbContext;

    public Create(IMyDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    [HttpPost(UsuarioRolRouteManager.BASE)]
    public override async Task<ActionResult<Respuesta>> HandleAsync(Request request, CancellationToken cancellationToken = default)
    {
        try
        {
            #region Validaciones
            var rol = await dbContext.UsuariosRoles.FirstOrDefaultAsync(r => r.Nombre.ToLower() == request.Nombre.ToLower(),cancellationToken);
            if (rol != null)
              return Respuesta.Fail($"Ya existe un rol con el nombre dado'({request.Nombre})'");
            #endregion
            rol = UsuarioRol.Crear(request);
            dbContext.UsuariosRoles.Add(rol);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Respuesta.Success(rol.Id);
        }
        catch(Exception e){
            return Respuesta.Fail(e.Message);
        }
}
}