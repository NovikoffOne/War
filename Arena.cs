using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace War
{
    class Arena
    {
        private Dictionary<IEntity, IEntity> _solders;
        private Country _country1;
        private Country _country2;

        public Arena(Country country1, Country country2)
        {
            _country1 = country1;
            _country2 = country2;
        }

        public void PrepareWariors()
        {
            _solders[_country1.CreateSolder()] = _country2.CreateDemolitionWorker();
            _solders[_country1.CreateDemolitionWorker()] = _country2.CreateScout();
            _solders[_country1.CreateScout()] = _country2.CreateHeavySoldier();
            _solders[_country1.CreateHeavySoldier()] = _country2.CreateSolder();
        }

        public void Figth()
        {
            while()
        }

        private bool CheckEveryoneIsDead()
        {
            foreach (var solder in _solders)
            {
                if (solder.Key != null || solder.Value != null)
                    return true;
            }
        }
    }
}
