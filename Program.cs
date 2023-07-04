using System;
using System.Collections.Generic;
using System.Linq;

namespace War
{
    class Program
    {
        static void Main(string[] args)
        {
            Country Kashkania = new Country("Кашкания");
            Country Dauniya = new Country("Дауния");

            Queue<IEntity> armyKashkania = new Queue<IEntity>();
            armyKashkania.Enqueue(Kashkania.CreateHeavySoldier());
            armyKashkania.Enqueue(Kashkania.CreateSolder());
            armyKashkania.Enqueue(Kashkania.CreateDemolitionWorker());

            Queue<IEntity> armyDauniya = new Queue<IEntity>();
            armyDauniya.Enqueue(Dauniya.CreateScout());
            armyDauniya.Enqueue(Dauniya.CreateHeavySoldier());
            armyDauniya.Enqueue(Dauniya.CreateScout());

            Battlefield battlefield = new Battlefield(armyKashkania, armyDauniya);
            battlefield.Battle();
        }
    }

    class Battlefield
    {
        private Queue<IEntity> _attackingCountry;
        private Queue<IEntity> _defendingCountry;

        public Battlefield(Queue<IEntity> AttackingCountry, Queue<IEntity> DefendingCountry)
        {
            _attackingCountry = AttackingCountry;
            _defendingCountry = DefendingCountry;
        }

        public void Battle()
        {
            while(_attackingCountry.Count > 0 && _defendingCountry.Count > 0)
            {
                FightSoldier(_attackingCountry.Dequeue(), _defendingCountry.Dequeue());
            }

            if(_attackingCountry.Count > 0)
                ShowWinner(_attackingCountry);
            else if(_defendingCountry.Count > 0)
                ShowWinner(_defendingCountry);
        }

        private void FightSoldier(IEntity AttackingSolder,IEntity DefendingSolder)
        {
            while (AttackingSolder.Die() == false && DefendingSolder.Die() == false)
            {
                DefendingSolder.TakeDamage(AttackingSolder.Attack());

                if(DefendingSolder.Die() == false)
                    AttackingSolder.TakeDamage(DefendingSolder.Attack());
            }

            if (AttackingSolder.Die() == true && DefendingSolder.Die() == false)
                _defendingCountry.Enqueue(DefendingSolder);
            else if (DefendingSolder.Die() == true && AttackingSolder.Die() == false)
                _attackingCountry.Enqueue(AttackingSolder);
        }

        private void ShowWinner(Queue<IEntity> winner)
        {
            Console.WriteLine($"Победила - {winner.First().Country()}, в живых осталось {winner.Count()} солдат.");
        }
    }

    public class Country
    {
        #region RankToString
        private const string ScoutRank = "Скаут";
        private const string SolderRank = "Солдат";
        private const string HeavySoldierRank = "Танк";
        private const string DemolitionWorkerRank = "Камикадзе";
        #endregion

        #region BaseParams
        private int _baseHealthScout = 30;
        private int _baseDamageScout = 15;
        private int _baseHealthHeavySolder = 60;
        private int _baseDamageHeavySolder = 10;
        private int _baseHealthSolder = 50;
        private int _baseDamageSolder = 10;
        #endregion

        public string Name { get; private set; }

        public Country(string name)
        {
            Name = name;
        }

        public IEntity CreateSolder()
        {
            return new Soldier(this, _baseHealthSolder, _baseDamageSolder, SolderRank);
        }

        public IEntity CreateScout()
        {
            return new Scout(new Soldier(this, _baseHealthScout, _baseDamageScout, ScoutRank));
        }

        public IEntity CreateHeavySoldier()
        {
            return new HeavySoldier(new Soldier(this, _baseHealthHeavySolder, _baseDamageHeavySolder, HeavySoldierRank));
        }
        public IEntity CreateDemolitionWorker()
        {
            return new DemolitionWorker(new Soldier(this, _baseHealthSolder, _baseDamageSolder, DemolitionWorkerRank));
        }
    }

    public interface IEntity
    {
        void TakeDamage(int damage);

        int Attack();

        bool Die();

        string Rank();

        string Country();
    }

    public class Soldier : IEntity
    {
        private string _rank;
        private int _health;
        private int _damage;
        private Country _country;

        public bool Die()
        {
            if(_health <= 0)
            {
                return true;
            }

            return false;
        }

        public Soldier(Country country, int health, int damage, string rank)
        {
            _damage = damage;
            _health = health;
            _rank = rank;
            _country = country;
        }

        public int Attack()
        {
            return _damage;
        }

        public void Died()
        {
            Console.WriteLine(_rank + " убит.");
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;

            if (_health < 0)
                _health = 0;

            if (_health == 0)
                Died();
        }

        public string Rank()
        {
            return _rank;
        }

        public string Country()
        {
            return _country.Name;
        }
    }

    public class HeavySoldier : IEntity
    {
        private IEntity _entity;
        private int _armor;
        private int _armorDebuff = 1;

        public HeavySoldier(IEntity entity, int armor = 4)
        {
            _entity = entity;
            _armor = armor;
        }

        public int Attack()
        {
            return _entity.Attack() - _armorDebuff;
        }

        public bool Die()
        {
            return _entity.Die();
        }

        public void TakeDamage(int damage)
        {
            _entity.TakeDamage(damage - _armor);
        }

        public string Rank()
        {
            return _entity.Rank();
        }

        public string Country()
        {
            return _entity.Country();
        }
    }

    public class Scout : IEntity
    {
        private IEntity _entity;
        private int _agility;
        private int _damageFactor;

        public Scout(IEntity entity, int agility = 4, int damageFactor = 2)
        {
            _entity = entity;
            _agility = agility;
            _damageFactor = damageFactor;
        }

        public int Attack()
        {
            return _entity.Attack() * _damageFactor;
        }

        public void TakeDamage(int damage)
        {
            damage -= damage / _agility;

            _entity.TakeDamage(damage);
        }

        public bool Die()
        {
            return _entity.Die();
        }

        public string Rank()
        {
            return _entity.Rank();
        }

        public string Country()
        {
            return _entity.Country();
        }
    }

    public class DemolitionWorker : IEntity
    {
        private IEntity _entity;
        private int _damageFactor;

        public DemolitionWorker(IEntity entity, int damageFactor = 4)
        {
            _entity = entity;
            _damageFactor = damageFactor;
        }

        public int Attack()
        {
            return _entity.Attack() * _damageFactor;
        }

        public void TakeDamage(int damage)
        {
            _entity.TakeDamage(damage * _damageFactor);
        }

        public bool Die()
        {
            return _entity.Die();
        }

        public string Rank()
        {
            return _entity.Rank();
        }

        public string Country()
        {
            return _entity.Country();
        }
    }
}
