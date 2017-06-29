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
    public enum WeaponType { Cannon, Laser, Plazma, Missile, Torpedo }
    public enum ShellType { Solid, SolidMedium, SolidBig, SolidAP, Subcaliber, SubcaliberMedium, SubcaliberBig, HightExplosive, Camorous, CamorousBig, CamorousAP, Uranium, Сumulative, Railgun }
    public enum ShellLineType { Solid, Camorus, ArmorPenetration, ShildOwerheat, QuickShell, Explosive, Universal }
    public enum EnergyType { RedRay, GreenRay, BlueRay, Plazma }
    public enum MissileType { Hunter, Bombardier, Metheor, Interceptor }
    public enum TorpedoType { Unitary, Nuke, Sprute }
    public enum BlastType { UnitaryTorpedo, Missile, NukeTorpedo, SmallShip, MediumShip, Corvette, Shell, ExplosiveShell }
}
