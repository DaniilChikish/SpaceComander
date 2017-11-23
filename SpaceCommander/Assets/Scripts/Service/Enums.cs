using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SpaceCommander.General
{
    public enum Languages { English, Russian, temp };
}
namespace SpaceCommander.Mechanics
{
    public enum Army { Green, Red, Blue, Civil };
    public enum RelationshipType { Allies, Enemys, Neutral};
    public enum UnitClass { Scout, Recon, ECM, Figther, Bomber, Command, LR_Corvette, Guard_Corvette, Support_Corvette, Turret, Drone, Torpedo, Civil };
    public enum UnitStateType { MoveAI, UnderControl, Waiting };
    public enum SquadStatus { Free, InSquad, SquadMaster }
    public enum TacticSituation { SectorСlear, Attack, Defense, Retreat, ExitTheBattle }
    public enum TargetStateType { BehindABarrier, InPrimaryRange, InSecondaryRange, Captured, NotFinded };
    public enum WeaponType { Cannon, MachineCannon, ShootCannon, Railgun, Railmortar, Laser, Plazma, MagnetohydrodynamicGun, Missile, Torpedo, Chaingun };
    public enum SmallShellType { SemiShell, Solid, APShell, Incendiary, BuckShot };
    public enum MediumShellType { Camorous, CamorousAP, Subcaliber };
    public enum BigShellType { WolframIngot, UraniumIngot, HigExplosive };
    public enum ShellLineType { Solid, ArmorPenetration, ShildOwerheat, Incendiary, Universal }
    public enum EnergyType { RedRay, GreenRay, BlueRay, Plazma }
    public enum MissileType { Hunter, Bombardier, Metheor, Interceptor}
    public enum TorpedoType { Unitary, Nuke, Sprute, ShieldsBreaker, Thunderbolth, DragonTooth}
    public enum BlastType { UnitaryTorpedo, Missile, NukeTorpedo, SmallShip, MediumShip, Corvette, Shell, ExplosiveShell }
}
