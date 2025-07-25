using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IUnitImagesRepository : IGenericRepository<UnitImages>
    {
        /// <summary>
        /// Retrieves a list of unit images associated with a specific unit ID.
        /// </summary>
        /// <param name="unitId">The ID of the unit for which to retrieve images.</param>
        /// <returns>A task that represents the asynchronous operation, containing a list of unit images.</returns>
        Task<IEnumerable<UnitImages>> GetUnitImagesByUnitIdAsync(int unitId);
        
        /// <summary>
        /// Deletes all images associated with a specific unit ID.
        /// </summary>
        /// <param name="unitId">The ID of the unit whose images are to be deleted.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        void DeleteAllImagesByUnitIdAsync(int unitId);
    }
}
