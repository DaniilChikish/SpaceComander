using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceCommander
{
    public enum Languages { English, Russian, temp };
    public enum UnitClass { Scout, Recon, ECM, Figther, Bomber, Command, LR_Corvette, Guard_Corvette, Support_Corvette, Turret, Drone, Torpedo };
    public enum Army { Green, Red, Blue };
    public enum UnitStateType { MoveAI, UnderControl, Waiting };
    public enum SquadStatus { Free, InSquad, SquadMaster }
    public enum TacticSituation { SectorСlear, Attack, Defense, Retreat, ExitTheBattle }
    public enum TargetStateType { BehindABarrier, InPrimaryRange, InSecondaryRange, Captured, NotFinded };
    public enum WeaponType { Cannon, Autocannon, ShootCannon, Railgun, Railmortar, Laser, Plazma, MagnetohydrodynamicGun, Missile, Torpedo };
    public enum SmallShellType { SemiShell, Solid, APShell, Incendiary, BuckShot };
    public enum MediumShellType { Camorous, CamorousAP, Subcaliber };
    public enum BigShellType { WolframIngot, UraniumIngot, HigExplosive };
    public enum ShellLineType { Solid, ArmorPenetration, ShildOwerheat, Incendiary, Universal }
    public enum EnergyType { RedRay, GreenRay, BlueRay, Plazma }
    public enum MissileType { Hunter, Bombardier, Metheor, Interceptor }
    public enum TorpedoType { Unitary, Nuke, Sprute, ShieldsBreaker }
    public enum BlastType { UnitaryTorpedo, Missile, NukeTorpedo, SmallShip, MediumShip, Corvette, Shell, ExplosiveShell }
}
