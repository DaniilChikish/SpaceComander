using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using SpaceCommander.Units;

namespace SpaceCommander
{
    public interface IUnit
    {
        UnitClass Type { get; }
        Army Team { get; }
        Transform ObjectTransform { get; }
        Vector3 Velocity { get; }
        void MakeImpact(IImpact impact);
        void MakeDamage(float damage);
        void Die();
    }
    public interface IDriver
    {
        Vector3 Velocity { get; }
        void Update();
        void UpdateSpeed();
        bool MoveTo(Vector3 destination);
        bool MoveToQueue(Vector3 destination);
        void ClearQueue();
        int PathPoints { get; }
        Vector3 NextPoint { get; }
    }
    public interface IGunner
    {
        void Update();
        bool SetAim(IUnit target);
        bool ShootHim(SpaceShip target, int slot);
        bool Volley(int slot);
        bool ResetAim();
        void ReloadWeapons();
        float GetRange(int slot);
    }
    public interface IWeapon
    {
        IUnit Target { set; get; }
        float Range { get; }
        float RoundSpeed { get; }
        int Ammo { get; }
        float Cooldown { get; }
        float Dispersion { get; }
        float CoolingTime { get; }
        float ShildBlink { get; }
        void StatUp();
        void Reset();
        void InstantCool();
        bool Fire();
    }
    public interface IShell
    {
        float Speed { get; }
        float Damage { get; }
        float ArmorPiersing { get; }
        void StatUp(ShellType type);
    }
    public interface IEnergy
    {
        float Speed { get; }
        float Damage { get; }
        float ArmorPiersing { get; }
        void StatUp(EnergyType type);
        float GetEnergy();
    }
    public interface IImpact
    {
        string Name { get; }
        void ActImpact();
        void CompleteImpact();
        string ToString();
    }
}
