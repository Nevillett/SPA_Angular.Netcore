using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SPA_Angular.NETCore.Core;
using SPA_Angular.NETCore.Core.Models;
using System.Linq;
using System.Linq.Expressions;

namespace SPA_Angular.NETCore.Persistence
{
    public class VehicleRepository: IVehicleRepository
    {
        private readonly SpaDbContext context;
        public VehicleRepository(SpaDbContext context)
        {
            this.context = context;
        }

        public async Task<Vehicle> GetVehicle(int id, bool includeRelated = true)
        {
            if(!includeRelated)
                return await context.Vehicles.FindAsync(id);

            return await context.Vehicles
            .Include(v => v.Features)
                .ThenInclude(vf => vf.Feature)
            .Include(v => v.Model)
                .ThenInclude(md => md.Make)
            .SingleOrDefaultAsync(v => v.Id == id);
        }

        public void Add(Vehicle vehicle)
        {
            context.Vehicles.Add(vehicle);
        }

        public void Remove(Vehicle vehicle)
        {
            context.Remove(vehicle);
        }

        public async Task<IEnumerable<Vehicle>> GetVehicles(VehicleQuery queryObj)
        {
            // return await context.Vehicles
            // .Include(v => v.Model)
            //     .ThenInclude(md => md.Make)
            // .Include(v => v.Features)
            //     .ThenInclude(vf => vf.Feature)
            // .ToListAsync();

            //filter, using dynamically query
            var query = context.Vehicles
                        .Include(v => v.Model)
                            .ThenInclude(md => md.Make)
                        .Include(v => v.Features)
                            .ThenInclude(vf => vf.Feature)
                        .AsQueryable();

            if (queryObj.MakeId.HasValue)
                query = query.Where(v => v.Model.MakeId == queryObj.MakeId.Value);
            // sorting     
            if (queryObj.SortBy == "make")
               query = (queryObj.IsSortAscending) ? query.OrderBy(v => v.Model.Make.Name) : query.OrderByDescending(v => v.Model.Make.Name);
   
            //Expression<Func<Vehicle, Object>>

        
            return await query.ToListAsync();
        }
    }
}