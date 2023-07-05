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
            armyKashkania.Enqueue(Kashkania.CreateScout());
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
            while (AttackingSolder.IsDead() == false && DefendingSolder.IsDead() == false)
            {
                AttackingSolder.Attack(DefendingSolder);

                if(DefendingSolder.IsDead() == false)
                    DefendingSolder.Attack(AttackingSolder);
            }

            if (AttackingSolder.IsDead() == true && DefendingSolder.IsDead() == false)
                _defendingCountry.Enqueue(DefendingSolder);
            else if (DefendingSolder.IsDead() == true && AttackingSolder.IsDead() == false)
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
            return new Scout(this, _baseHealthScout, _baseDamageScout, ScoutRank);
        }

        public IEntity CreateHeavySoldier()
        {
            return new HeavySoldier(this, _baseHealthHeavySolder, _baseDamageHeavySolder, HeavySoldierRank);
        }
        public IEntity CreateDemolitionWorker()
        {
            return new DemolitionWorker(this, _baseHealthSolder, _baseDamageSolder, DemolitionWorkerRank);
        }
    }

    public interface IEntity
    {
        void TakeDamage(int damage);

        void Attack(IEntity enemy);

        bool IsDead();

        string Rank();

        string Country();
    }

    public class Soldier : IEntity
    {
        private string _rank;
        private int _health;
        private int _damage;

        private Country _country;

        public bool IsDead() => _health <= 0;
        public string Rank() => _rank;
        public string Country() => _country.Name;

        public Soldier(Country country, int health, int damage, string rank)
        {
            _damage = damage;
            _health = health;
            _rank = rank;
            _country = country;
        }

        public void Attack(IEntity enemy)
        {
            enemy.TakeDamage(CalculateAttackDamage(_damage));
        }

        public void Died()
        {
            Console.WriteLine(_rank + " убит.");
        }

        public void TakeDamage(int damage)
        {
            _health -= CalculateArmorDamage(damage);

            if (_health < 0)
                _health = 0;

            if (IsDead() == true)
                Died();
        }

        protected virtual int CalculateAttackDamage(int damage)
        {
            return damage;
        }

        protected virtual int CalculateArmorDamage(int damage)
        {
            return damage;
        }
    }

    public class HeavySoldier : Soldier
    {
        private int _armor;
        private int _armorDebuff = 1;

        public HeavySoldier(Country country, int health, int damage, string rank, int armor = 4) : base(country, health, damage, rank)
        {
            _armor = armor;
        }

        protected override int CalculateAttackDamage(int damage)
        {
            return damage -= _armorDebuff;
        }

        protected override int CalculateArmorDamage(int damage)
        {
            return damage -= _armor;
        }
    }

    public class Scout : Soldier
    {
        private int _agility;
        private int _damageFactor;

        public Scout(Country country, int health, int damage, string rank, int agility = 4, int damageFactor = 2) 
            : base(country, health, damage, rank)
        {
            _agility = agility;
            _damageFactor = damageFactor;
        }

        protected override int CalculateAttackDamage(int damage)
        {
            return damage *= _damageFactor;
        }

        protected override int CalculateArmorDamage(int damage)
        {
            damage -= damage / _agility;

            return damage;
        }
    }

    public class DemolitionWorker : Soldier
    {
        private int _damageFactor;

        public DemolitionWorker(Country country, int health, int damage, string rank, int damageFactor = 4) 
            : base(country, health, damage, rank)
        {
            _damageFactor = damageFactor;
        }

        protected override int CalculateAttackDamage(int damage)
        {
            return damage *= _damageFactor;
        }

        protected override int CalculateArmorDamage(int damage)
        {
            return damage *= _damageFactor;
        }
    }
}