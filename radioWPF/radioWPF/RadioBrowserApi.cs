using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace radioWPF
{
    public interface IRadioBrowserApi
    {
        [Get("/json/stations/bycountry/{country}?limit={limit}")]
        Task<List<Data_Structure>> GetByCountry(int limit, string country);

        [Get("/json/stations/byname/{name}?limit={limit}")]
        Task<List<Data_Structure>> GetByName(int limit, string name);

        [Get("/json/stations/broken")]
        Task<List<Data_Structure>> GetBrokenStations();

        [Get("/json/stations")]
        Task<List<Data_Structure>> GetAllStations();

    }
}
